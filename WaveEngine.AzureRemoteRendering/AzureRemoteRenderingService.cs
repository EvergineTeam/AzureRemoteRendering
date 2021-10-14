// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using Microsoft.Azure.RemoteRendering;
using SharpDX.Direct3D11;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.DirectX11;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Platform;
using ARREntity = Microsoft.Azure.RemoteRendering.Entity;
using ARRMaterial = Microsoft.Azure.RemoteRendering.Material;
using ARRTexture = Microsoft.Azure.RemoteRendering.Texture;
using ARRTextureType = Microsoft.Azure.RemoteRendering.TextureType;

namespace WaveEngine.AzureRemoteRendering
{
    /// <summary>
    /// Service for the Azure Remote Rendering extension. This service provides functionality
    /// for <see cref="RemoteRenderingClient"/> and <see cref="RenderingSession"/> management. It has a concept
    /// of the <see cref="CurrentSession"/>. There can only be a single active session at a time.
    /// </summary>
    /// <remarks>
    /// This service must be initialized before <see cref="XRPlatform"/> service in Mixed Reality platform.
    /// Once attached, the method <see cref="Initialize(ARRSessionConfiguration)"/> must be called to perform
    /// authentication with the <see cref="RemoteRenderingClient"/>.
    /// Currently the extension only works with <see cref="GraphicsBackend.DirectX11"/>.
    /// </remarks>
    public class AzureRemoteRenderingService : UpdatableService
    {
        /// <summary>
        /// The <see cref="GraphicsPresenter"/> dependency used to retrieve the swapchain.
        /// </summary>
        [BindService]
        protected GraphicsPresenter graphicsPresenter;

        /// <summary>
        /// The <see cref="GraphicsContext"/> dependency used to retrieve the swapchain.
        /// Currently the extension only works with <see cref="GraphicsBackend.DirectX11"/>.
        /// </summary>
        [BindService]
        protected GraphicsContext graphicsContext;

        /// <summary>
        /// The <see cref="XRPlatform"/> dependency used in Mixed Reality graphics binding mode.
        /// </summary>
        [BindService(false)]
        protected XRPlatform xrPlatform;

        private ARRConnectionStatus connectionStatus = ARRConnectionStatus.Disconnected;

        private RemoteRenderingClient frontEnd;

        // Activate Deactivate
        private bool wasConnected;
        private RendererInitOptions rendererInitOptions;

        // Simulation
        private SimulationUpdateParameters update;
        private Mathematics.Matrix4x4 preUpdateCameraWorldTransform;

        // Mixed Reality
        private IntPtr userCoordinateSystem;

        /// <summary>
        /// Gets a value indicating the current session connection status of the service.
        /// </summary>
        public ARRConnectionStatus ConnectionStatus
        {
            get => this.connectionStatus;
            private set
            {
                if (this.connectionStatus != value)
                {
                    this.connectionStatus = value;
                    this.ConnectionStatusChanged?.Invoke(this, this.connectionStatus);
                }
            }
        }

        /// <summary>
        /// Gets the frontend associated with this service. Frontends are used for authentication and
        /// will be created through <see cref="Initialize(ARRSessionConfiguration)"/>.
        /// </summary>
        public RemoteRenderingClient FrontEnd
        {
            get
            {
                if (this.frontEnd == null)
                {
                    throw new InvalidOperationException($"{nameof(this.FrontEnd)} not available. {nameof(this.Initialize)} method must be called first.");
                }

                return this.frontEnd;
            }
        }

        /// <summary>
        /// Gets the active <see cref="RenderingSession"/>.
        /// </summary>
        public RenderingSession CurrentSession { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="CurrentSession"/> is connected.
        /// </summary>
        public bool IsCurrentSessionConnected => this.CurrentSession?.IsConnected == true;

        /// <summary>
        /// Occurs when <see cref="ConnectionStatus"/> is changed
        /// </summary>
        public event EventHandler<ARRConnectionStatus> ConnectionStatusChanged;

        /// <inheritdoc />
        protected override bool OnAttached()
        {
            if (!base.OnAttached())
            {
                return false;
            }

            if (Application.Current.IsEditor)
            {
                return true;
            }

            if (this.graphicsContext.BackendType != GraphicsBackend.DirectX11)
            {
                throw new InvalidOperationException(
                    $"Invalid {nameof(GraphicsContext)} backend type: {this.graphicsContext.BackendType}." +
                    $" Currently the extension only works with {GraphicsBackend.DirectX11}.");
            }

            if (!RenderingConnectionStatic.IsInitialized)
            {
                // 1. One time initialization
                var isWMRPlatform = DeviceInfo.PlatformType == PlatformType.UWP && this.xrPlatform != null;
                if (isWMRPlatform && this.xrPlatform.IsAttached)
                {
                    throw new InvalidOperationException($"In Windows Mixed Reality mode, {nameof(XRPlatform)} service must be registered after {nameof(AzureRemoteRenderingService)}.");
                }

                var clientInit = new RemoteRenderingInitialization();
                clientInit.ConnectionType = ConnectionType.General;
                clientInit.GraphicsApi = isWMRPlatform ? GraphicsApiType.WmrD3D11 : GraphicsApiType.SimD3D11;
                clientInit.ToolId = "Wave Engine";
                clientInit.UnitsPerMeter = 1.0f;
                clientInit.Forward = Axis.NegativeZ;
                clientInit.Right = Axis.X;
                clientInit.Up = Axis.Y;
                RenderingConnectionStatic.StartupRemoteRendering(clientInit);
            }

            return true;
        }

        /// <inheritdoc />
        protected override async void OnActivated()
        {
            base.OnActivated();

            if (this.wasConnected)
            {
                await this.CurrentSession.ConnectAsync(this.rendererInitOptions);
            }
        }

        /// <inheritdoc />
        protected override void OnDeactivated()
        {
            base.OnDeactivated();

            this.wasConnected = this.IsCurrentSessionConnected;
            if (this.wasConnected)
            {
                this.rendererInitOptions = this.CurrentSession.RendererInitOptions;
                this.CurrentSession.Disconnect();
            }
        }

        /// <inheritdoc />
        protected override void OnDestroy()
        {
            base.OnDestroy();

            this.UnsetCurrentSession();

            this.frontEnd?.Dispose();
            this.frontEnd = null;

            RenderingConnectionStatic.ShutdownRemoteRendering();
        }

        /// <inheritdoc />
        public override void Update(TimeSpan gameTime)
        {
            // Tick the client to receive messages
            this.CurrentSession?.Connection.Update();
        }

        internal bool UpdateLocal(Camera camera)
        {
            if (!this.IsCurrentSessionConnected)
            {
                return false;
            }

            var cameraSettings = this.CurrentSession.Connection.CameraSettings;
            cameraSettings.SetNearAndFarPlane(camera.NearPlane, camera.FarPlane);

            if (this.CurrentSession.GraphicsBinding is GraphicsBindingSimD3d11 simulationBinding)
            {
                this.update.FrameId++;
                ////this.update.nearPlaneDistance = camera.NearPlane;
                ////this.update.farPlaneDistance = camera.FarPlane;
                this.update.FieldOfView.Left.FromProjectionMatrix(camera.Projection.ToRemote());
                this.update.ViewTransform.Left = camera.Transform.WorldToLocalTransform.ToRemote();

                ////camera.Projection.ToRemote(out this.update.projection);
                ////camera.View.ToRemote(out this.update.viewTransform);
                var updateResult = simulationBinding.Update(this.update, out var proxyUpdate);
                if (updateResult != Result.Success && updateResult != Result.NoConnection)
                {
                    Debug.WriteLine($"ERROR: Simulation update failed: {updateResult}");
                }

                if (proxyUpdate.FrameId != 0)
                {
                    this.preUpdateCameraWorldTransform = camera.Transform.WorldTransform;

                    // Wave expects the smaller of the plane values to be the near plane
                    // and the larger of the two to be the far plane.
                    camera.NearPlane = Math.Min(proxyUpdate.NearPlaneDistance, proxyUpdate.FarPlaneDistance);
                    camera.FarPlane = Math.Max(proxyUpdate.NearPlaneDistance, proxyUpdate.FarPlaneDistance);

                    // Getting a projection matrix out of the proxyUpdate data. Note: Despite rendering with an inverse
                    // z depth buffer internally, Wave has a normal z range matrix on the cameras. Therefore, we need
                    // to request a matrix in a conventional [-1;1] OpenGL space.
                    ////proxyUpdate.projection.ToWave(out var remoteProjection);
                    ////proxyUpdate.viewTransform.ToWave(out var remoteView);
                    ////Mathematics.Matrix4x4.Invert(ref remoteView, out var remoteTransfom);
                    ////camera.SetCustomProjection(remoteProjection);
                    ////camera.Transform.WorldTransform = remoteTransfom;
                    if (proxyUpdate.FieldOfView.Left.ToProjectionMatrix(proxyUpdate.NearPlaneDistance, proxyUpdate.FarPlaneDistance, DepthConvention.MinusOneToOne, out Matrix4x4 projection) == Result.Success)
                    {
                        camera.SetCustomProjection(projection.ToWave());
                    }

                    var remoteTransform = proxyUpdate.ViewTransform.Left.ToWave();
                    remoteTransform.Invert();
                    camera.Transform.WorldTransform = remoteTransform;
                    return true;
                }
            }
            else if (this.CurrentSession.GraphicsBinding is GraphicsBindingWmrD3d11 wmrBinding)
            {
                if (this.xrPlatform.GetNativePointer("SpatialCoordinateSystem", out var ptr) &&
                     ptr != IntPtr.Zero &&
                     ptr != this.userCoordinateSystem &&
                     wmrBinding.UpdateUserCoordinateSystem(ptr) == Result.Success)
                {
                    this.userCoordinateSystem = ptr;
                }

                return true;
            }

            return false;
        }

        internal bool BlitRemoteFrame(Camera camera)
        {
            bool blitSuccess = false;

            if (this.CurrentSession?.GraphicsBinding is GraphicsBindingSimD3d11 simulationBinding)
            {
                camera.ResetCustomProjection();
                camera.Transform.WorldTransform = this.preUpdateCameraWorldTransform;

                var drawContext = camera.DrawContext;
                var frameBuffer = drawContext.IntermediateFrameBuffer ?? drawContext.FrameBuffer;
                var dx11FrameBuffer = (DX11FrameBuffer)frameBuffer;
                var colorDestination = dx11FrameBuffer.ColorTargetViews[0];
                var depthDestination = dx11FrameBuffer.DepthTargetview;
                var dxContext = (this.graphicsContext as DX11GraphicsContext).DXDeviceContext;
                dxContext.Rasterizer.SetViewport(0, 0, frameBuffer.Width, frameBuffer.Height);
                dxContext.OutputMerger.SetRenderTargets(depthDestination, colorDestination);
                blitSuccess = simulationBinding.BlitRemoteFrameToProxy() == Result.Success;
            }
            else if (this.CurrentSession.GraphicsBinding is GraphicsBindingWmrD3d11 wmrBinding)
            {
                var dxContext = (this.graphicsContext as DX11GraphicsContext).DXDeviceContext;
                var frameBuffer = camera.DrawContext.FrameBuffer as DX11FrameBuffer;
                dxContext.Rasterizer.SetViewport(0, 0, frameBuffer.Width, frameBuffer.Height);
                dxContext.OutputMerger.SetRenderTargets(frameBuffer.DepthTargetview, frameBuffer.ColorTargetViews);
                blitSuccess = wmrBinding.BlitRemoteFrame() == Result.Success;
            }

            if (blitSuccess)
            {
                camera.ClearFlags &= ~(ClearFlags.Depth | ClearFlags.Target);
            }

            return blitSuccess;
        }

        /// <summary>
        /// Initializes the service creating the <see cref="FrontEnd"/>.
        /// </summary>
        /// <param name="sessionConfig">The Azure Frontend credentials.</param>
        public void Initialize(ARRSessionConfiguration sessionConfig)
        {
            if (sessionConfig == null)
            {
                throw new ArgumentNullException(nameof(sessionConfig));
            }

            if (this.frontEnd != null)
            {
                throw new InvalidOperationException($"{nameof(AzureRemoteRenderingService)} is already initialized.");
            }

            if (!this.IsAttached)
            {
                throw new InvalidOperationException($"{nameof(AzureRemoteRenderingService)} is not attached.");
            }

            if (!sessionConfig.HasRequiredInfo)
            {
                throw new ArgumentException($"{nameof(sessionConfig)} has not all required info.");
            }

            this.frontEnd = new RemoteRenderingClient(sessionConfig.Convert());
        }

        /// <summary>
        /// Query the full set of existing rendering sessions for the account associated with the frontend. Since the underlying call is a REST call,
        /// there should be sufficient delay (5-10s) between subsequent calls to avoid server throttling. In case of throttling, the function will fail
        /// and the HttpResponseCode reports code 429 ("too many requests").
        /// </summary>
        /// <returns>
        /// A task with an array of <see cref="RenderingSessionProperties"/>.
        /// </returns>
        public Task<RenderingSessionPropertiesArrayResult> GetCurrentRenderingSessionsAsync()
        {
            return this.FrontEnd.GetCurrentRenderingSessionsAsync();
        }

        /// <summary>
        /// Create a new rendering session on the cloud. Once created, it will set as active <see cref="CurrentSession"/>.
        /// </summary>
        /// <param name="maxLease">
        /// A timeout value when the VM will be decommissioned automatically. The expiration time is VM start time + MaxLease.
        /// </param>
        /// <param name="size">The VM size.</param>
        /// <returns><c>true</c> if the rendering session is created and ready; otherwise, <c>false</c>.</returns>
        public async Task<bool> CreateNewRenderingSessionAsync(TimeSpan maxLease, RenderingSessionVmSize size = RenderingSessionVmSize.Standard)
        {
            // create a new session
            var renderingSessionParams = new RenderingSessionCreationOptions();
            renderingSessionParams.MaxLeaseInMinutes = (int)maxLease.TotalMinutes;
            renderingSessionParams.Size = size;
            try
            {
                this.ConnectionStatus = ARRConnectionStatus.CreatingSession;
                var sessionRes = await this.FrontEnd.CreateNewRenderingSessionAsync(renderingSessionParams);
                return await this.SetNewSessionAsync(sessionRes.Session);
            }
            catch (RRSessionException)
            {
                this.ConnectionStatus = ARRConnectionStatus.ConnectionFailed;
                return false;
            }
        }

        /// <summary>
        /// Open an existing rendering session. Once opened, it will set as active <see cref="CurrentSession"/>.
        /// </summary>
        /// <param name="sessionId">
        /// Rendering Session UUID.
        /// </param>
        /// <returns><c>true</c> if the rendering session is found and ready; otherwise, <c>false</c>.</returns>
        public async Task<bool> OpenRenderingSessionAsync(string sessionId)
        {
            var openSessionRes = await this.FrontEnd.OpenRenderingSessionAsync(sessionId);
            if (openSessionRes == null)
            {
                return false;
            }

            return await this.SetNewSessionAsync(openSessionRes.Session);
        }

        /// <summary>
        /// Renew an existing rendering session. This will update the lease of the VM to a new time. Time is absolute since starting time of the VM: expiration = start_time + lease_time.
        /// </summary>
        /// <param name="leaseTime">
        /// A timeout value when the VM will be decommissioned automatically. Time is absolute since starting time of the VM: expiration = start_time + <paramref name="leaseTime"/>.
        /// </param>
        /// <param name="sessionId">
        /// Rendering Session UUID. When not specified, this method will update the <see cref="CurrentSession"/>.
        /// </param>
        /// <returns><c>true</c> if the rendering session is found and updated; otherwise, <c>false</c>.</returns>
        public async Task<bool> RenewRenderingSessionAsync(TimeSpan leaseTime, string sessionId = null)
        {
            var isCurrentSession = string.IsNullOrEmpty(sessionId);
            var session = isCurrentSession ? this.CurrentSession : (await this.FrontEnd.OpenRenderingSessionAsync(sessionId))?.Session;
            if (session == null)
            {
                return false;
            }

            var stopAsync = session.RenewAsync(new RenderingSessionUpdateOptions(leaseTime.Hours, leaseTime.Minutes));
            var stopRes = await stopAsync;

            return stopRes.ErrorCode == Result.Success;
        }

        /// <summary>
        /// Stop an existing rendering session. This will decommission the rendering session.
        /// </summary>
        /// <param name="sessionId">
        /// Rendering Session UUID. When not specified, this method will stop the <see cref="CurrentSession"/>.
        /// </param>
        /// <returns><c>true</c> if the rendering session is found and stopped; otherwise, <c>false</c>.</returns>
        public async Task<bool> StopRenderingSessionAsync(string sessionId = null)
        {
            var isCurrentSession = string.IsNullOrEmpty(sessionId);
            var session = isCurrentSession ? this.CurrentSession : (await this.FrontEnd.OpenRenderingSessionAsync(sessionId)).Session;
            if (session == null)
            {
                return false;
            }

            var stopAsync = session.StopAsync();
            var stopRes = await stopAsync;

            var result = stopRes.ErrorCode == Result.Success;
            if (result && isCurrentSession)
            {
                this.UnsetCurrentSession();
            }

            return result;
        }

        /// <summary>
        /// Connect to the runtime on the virtual machine associated with the <see cref="CurrentSession"/>.
        /// </summary>
        /// <param name="renderMode">Mode for the rendering session.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="CurrentSession"/> has been connected; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> ConnectAsync(ServiceRenderMode renderMode = ServiceRenderMode.Default)
        {
            if (this.CurrentSession == null)
            {
                return false;
            }

            if (this.IsCurrentSessionConnected)
            {
                return true;
            }

            await this.CurrentSession.ConnectAsync(new RendererInitOptions(renderMode));

            return this.IsCurrentSessionConnected;
        }

        /// <summary>
        /// Get a file path pointing to a HTML file to connect to the ARR inspector website.
        /// The ARR inspector provides introspection and service status updates for the current
        /// session.
        /// </summary>
        /// <returns>
        /// A task with the file path pointing to the generated HTML file.
        /// </returns>
        public async Task<string> ConnectToArrInspectorAsync()
        {
            if (!this.IsCurrentSessionConnected)
            {
                return null;
            }

            var sessionPropertiesRes = await this.CurrentSession.GetPropertiesAsync();
            return await this.CurrentSession.ConnectToArrInspectorAsync(/*sessionPropertiesRes.SessionProperties.Hostname*/);
        }

        /// <summary>
        /// Disconnect the <see cref="CurrentSession"/> if connected to the runtime.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the <see cref="CurrentSession"/> has been disconnected; otherwise, <c>false</c>.
        /// </returns>
        public bool Disconnect()
        {
            if (!this.IsCurrentSessionConnected)
            {
                return false;
            }

            var result = this.CurrentSession.Disconnect();
            return result == Result.Success;
        }

        /// <summary>
        /// Asynchronously perform a raycast query on the remote scene.
        /// The raycast will be performed on the server against the state of the world on the frame
        /// that the raycast was issued on. Results will be sorted by distance, with the closest intersection
        /// to the user being the first item in the array.
        /// </summary>
        /// <param name="rayCast">Outgoing <see cref="RayCast"/>.</param>
        /// <returns>
        /// A task with an array of <see cref="RayCastHit"/>.
        /// </returns>
        public Task<RayCastQueryResult> RayCastQueryAsync(RayCast rayCast)
        {
            if (!this.IsCurrentSessionConnected)
            {
                return Task.FromResult<RayCastQueryResult>(null);
            }

            return this.CurrentSession.Connection.RayCastQueryAsync(rayCast);
        }

        /// <summary>
        /// Tries to create a new ARR material on the server. The new material can be set to ARR Mesh components.
        /// </summary>
        /// <param name="type">
        /// Type of created material.
        /// </param>
        /// <param name="material">
        /// The created ARR material.
        /// </param>
        /// <returns>
        /// <c>true</c> if the <see cref="CurrentSession"/> is connected and the <see cref="ARRMaterial"/> has
        /// been created; otherwise, <c>false</c>.
        /// </returns>
        public bool TryCreateMaterial(MaterialType type, out ARRMaterial material)
        {
            if (!this.IsCurrentSessionConnected)
            {
                material = null;
                return false;
            }

            material = this.CurrentSession.Connection.CreateMaterial(type);
            return true;
        }

        /// <summary>
        /// Asynchronously load a texture from blob store.
        /// </summary>
        /// <param name="url">
        /// URL for the texture. Either builtin:// or a URL pointing at a converted texture. Both raw (public)
        /// URIs to blob store and URIs with embedded SAS tokens to blob store are supported.
        /// </param>
        /// <param name="textureType">Type of the texture.</param>
        /// <returns>
        /// A task with the loaded remote <see cref="ARRTexture"/>.
        /// </returns>
        public Task<ARRTexture> LoadTextureFromSASAsync(string url, ARRTextureType textureType)
        {
            if (!this.IsCurrentSessionConnected)
            {
                return Task.FromResult<ARRTexture>(default);
            }

            var parameters = new LoadTextureFromSasOptions(url, textureType);
            return this.CurrentSession.Connection.LoadTextureFromSasAsync(parameters);
        }

        /// <summary>
        /// Asynchronously load a texture from blob store.
        /// </summary>
        /// <param name="blobOptions">Blob store parameters for loading.</param>
        /// <param name="textureType">Type of the texture.</param>
        /// <returns>
        /// A task with the loaded remote <see cref="ARRTexture"/>.
        /// </returns>
        public Task<ARRTexture> LoadTextureAsync(LoadFromBlobOptions blobOptions, ARRTextureType textureType)
        {
            if (!this.IsCurrentSessionConnected)
            {
                return Task.FromResult<ARRTexture>(default);
            }

            var parameters = new LoadTextureOptions()
            {
                Blob = blobOptions,
                TextureType = textureType,
            };
            return this.CurrentSession.Connection.LoadTextureAsync(parameters);
        }

        /// <summary>
        /// Asynchronously load a model.
        /// </summary>
        /// <param name="url">
        /// URL for the model. Either builtin:// or a URL pointing at a converted model. Both raw (public) URIs
        /// to blob store and URIs with embedded SAS tokens to blob store are supported.
        /// </param>
        /// <param name="parent">Optional parent for the loaded model.</param>
        /// <param name="progress">
        /// A provider for model load progress updates. It will report a float with a loading percentage.
        /// </param>
        /// <returns>
        /// A task with a remote <see cref="ARREntity"/> that represents the root of the loaded model.
        /// </returns>
        public Task<LoadModelResult> LoadModelFromSASAsync(string url, ARREntity parent = null, IProgress<float> progress = null)
        {
            if (!this.IsCurrentSessionConnected)
            {
                return Task.FromResult<LoadModelResult>(default);
            }

            var options = new LoadModelFromSasOptions(url, parent);
            return this.CurrentSession.Connection.LoadModelFromSasAsync(options, null);
        }

        /// <summary>
        /// Asynchronously load a model from blob store.
        /// </summary>
        /// <param name="blobOptions">Blob store parameters for loading.</param>
        /// <param name="parent">Optional parent for the loaded model.</param>
        /// <param name="progress">
        /// A provider for model load progress updates. It will report a float with a loading percentage.
        /// </param>
        /// <returns>
        /// A task with a remote <see cref="ARREntity"/> that represents the root of the loaded model.
        /// </returns>
        public Task<LoadModelResult> LoadModelAsync(LoadFromBlobOptions blobOptions, ARREntity parent = null, IProgress<float> progress = null)
        {
            if (!this.IsCurrentSessionConnected)
            {
                return Task.FromResult<LoadModelResult>(default);
            }

            var options = new LoadModelOptions()
            {
                Blob = blobOptions,
                Parent = parent,
            };
            return this.CurrentSession.Connection.LoadModelAsync(options, null);
        }

        private async Task<bool> SetNewSessionAsync(RenderingSession session)
        {
            bool success = true;
            this.UnsetCurrentSession();

            this.CurrentSession = session;
            this.ConnectionStatus = ARRConnectionStatus.StartingSession;
            this.CurrentSession.ConnectionStatusChanged += this.Session_ConnectionStatusChanged;

            if (this.CurrentSession.GraphicsBinding is GraphicsBindingSimD3d11 simulationBinding)
            {
                var swapChainDescription = this.graphicsPresenter.FocusedDisplay.SwapChain.SwapChainDescription;
                var textureDescription = new TextureDescription()
                {
                    Type = Common.Graphics.TextureType.Texture2D,
                    Usage = Common.Graphics.ResourceUsage.Default,
                    Flags = TextureFlags.RenderTarget | TextureFlags.ShaderResource,
                    Format = swapChainDescription.ColorTargetFormat,
                    Width = swapChainDescription.Width,
                    Height = swapChainDescription.Height,
                    Depth = 1,
                    MipLevels = 1,
                    ArraySize = 1,
                    Faces = 1,
                    CpuAccess = ResourceCpuAccess.None,
                    SampleCount = TextureSampleCount.None,
                };
                var proxyColorTexture = this.graphicsContext.Factory.CreateTexture(ref textureDescription);
                var proxyColorPtr = proxyColorTexture.NativePointer;

                textureDescription.Format = swapChainDescription.DepthStencilTargetFormat;
                textureDescription.Flags = TextureFlags.DepthStencil;
                var proxyDepthTexture = this.graphicsContext.Factory.CreateTexture(ref textureDescription);
                var proxyDepthPtr = proxyDepthTexture.NativePointer;

                this.update = new SimulationUpdateParameters();
                ////{
                ////    renderTargetWidth = (int)textureDescription.Width,
                ////    renderTargetHeight = (int)textureDescription.Height,
                ////};

                // In spite of ARR BlitRemoteFrame invocation is performed using framebuffer's color and depth targets
                // (avoid the need of DX11.CopyResource), InitSimulation invocation need textures with specific description
                // even though they are disposed right after the invocation.
                var m_device = this.graphicsContext.NativeDevicePointer;
                var refreshRate = swapChainDescription.RefreshRate;
                var result = simulationBinding.InitSimulation(m_device, proxyDepthPtr, proxyColorPtr, refreshRate, false, false, false);
                success = result == Result.Success;
                proxyColorTexture.Dispose();
                proxyDepthTexture.Dispose();
            }

            if (!success)
            {
                this.UnsetCurrentSession();
            }

            return success && await this.CheckSessionIsReadyAsync(this.CurrentSession);
        }

        private void UnsetCurrentSession()
        {
            if (this.CurrentSession == null)
            {
                return;
            }

            if (this.CurrentSession.GraphicsBinding is GraphicsBindingSimD3d11 simulationSession)
            {
                simulationSession.DeinitSimulation();
            }

            this.CurrentSession.ConnectionStatusChanged -= this.Session_ConnectionStatusChanged;
            this.CurrentSession = null;
        }

        private async Task<bool> CheckSessionIsReadyAsync(RenderingSession session)
        {
            while (true)
            {
                try
                {
                    var properties = await session.GetPropertiesAsync();
                    switch (properties.SessionProperties.Status)
                    {
                        case RenderingSessionStatus.Ready:
                            return true;
                        case RenderingSessionStatus.Error:
                        case RenderingSessionStatus.Stopped:
                        case RenderingSessionStatus.Expired:
                            return false;
                    }

                    await Task.Delay(1000);
                }
                catch
                {
                    return false;
                }
            }
        }

        private void Session_ConnectionStatusChanged(ConnectionStatus status, Result error)
        {
            switch (status)
            {
                case Microsoft.Azure.RemoteRendering.ConnectionStatus.Connecting:
                    this.ConnectionStatus = ARRConnectionStatus.Connecting;
                    break;
                case Microsoft.Azure.RemoteRendering.ConnectionStatus.Connected:
                    this.ConnectionStatus = error == Result.Success ? ARRConnectionStatus.Connected : ARRConnectionStatus.ConnectionFailed;
                    break;
                case Microsoft.Azure.RemoteRendering.ConnectionStatus.Disconnected:
                    this.ConnectionStatus = error == Result.Success ? ARRConnectionStatus.Disconnected : ARRConnectionStatus.ConnectionFailed;
                    break;
            }
        }
    }
}
