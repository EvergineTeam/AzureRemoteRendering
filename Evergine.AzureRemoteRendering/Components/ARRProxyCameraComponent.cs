// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using Evergine.Common.Graphics;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Framework.Managers;

namespace Evergine.AzureRemoteRendering.Components
{
    /// <summary>
    /// A component that binds the Azure Remote Rendering proxy camera with an <see cref="Camera3D"/>.
    /// </summary>
    public class ARRProxyCameraComponent : Component
    {
        /// <summary>
        /// The <see cref="AzureRemoteRenderingService"/> dependency used by this component.
        /// </summary>
        [BindService]
        protected AzureRemoteRenderingService arrService;

        /// <summary>
        /// The RenderManager of the scene.
        /// </summary>
        [BindSceneManager]
        protected RenderManager renderManager;

        /// <summary>
        /// The <see cref="Camera3D"/> component dependency that will be used to render ARR remote frame.
        /// </summary>
        [BindComponent]
        protected Camera3D camera3D;

        private bool localUpdateDone;

        private bool isProxyCameraActive;
        private bool enableDepth = true;
        private bool inverseDepth = false;

        /// <summary>
        /// Gets or sets a value indicating whether the depth composition is enabled with locally rendered content.
        /// </summary>
        public bool EnableDepth
        {
            get => this.enableDepth;
            set
            {
                this.enableDepth = value;
                if (this.arrService != null)
                {
                    this.arrService.EnableDepth = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether you are using the inverse depth range of 1 (closest to the camera) to zero (farthest from the camera) instead of the standard [0;1] for your local depth buffer.
        /// </summary>
        public bool InverseDepth
        {
            get => this.inverseDepth;
            set
            {
                this.inverseDepth = value;
                if (this.arrService != null)
                {
                    this.arrService.InverseDepth = value;
                }
            }
        }

        /// <inheritdoc/>
        protected override bool OnAttached()
        {
            this.arrService.EnableDepth = this.enableDepth;
            this.arrService.InverseDepth = this.inverseDepth;
            return base.OnAttached();
        }

        /// <inheritdoc/>
        protected override void OnActivated()
        {
            base.OnActivated();

            this.camera3D.BackgroundColor = Color.Black;

            this.arrService.ConnectionStatusChanged += this.ArrService_ConnectionStatusChanged;
            this.ArrService_ConnectionStatusChanged(this, this.arrService.ConnectionStatus);
        }

        /// <inheritdoc/>
        protected override void OnDeactivated()
        {
            base.OnDeactivated();

            this.arrService.ConnectionStatusChanged -= this.ArrService_ConnectionStatusChanged;
            this.DeactivateProxyCamera();
        }

        private void ActivateProxyCamera()
        {
            if (!this.isProxyCameraActive)
            {
                this.isProxyCameraActive = true;
                this.camera3D.DrawContext.OnCollect += this.DrawContext_OnCollect;
                this.camera3D.DrawContext.OnPreRender += this.DrawContext_OnPreRender;
                this.renderManager.OnPostRender += this.RenderManager_OnPostRender;
            }
        }

        private void DeactivateProxyCamera()
        {
            if (this.isProxyCameraActive)
            {
                this.isProxyCameraActive = false;
                this.camera3D.DrawContext.OnCollect -= this.DrawContext_OnCollect;
                this.camera3D.DrawContext.OnPreRender -= this.DrawContext_OnPreRender;
                this.renderManager.OnPostRender -= this.RenderManager_OnPostRender;
            }
        }

        private void ArrService_ConnectionStatusChanged(object sender, ARRConnectionStatus status)
        {
            if (status == ARRConnectionStatus.Connected)
            {
                this.ActivateProxyCamera();
            }
            else
            {
                this.DeactivateProxyCamera();
            }
        }

        private void DrawContext_OnCollect(object sender, EventArgs e)
        {
            this.localUpdateDone = this.arrService.UpdateLocal(this.camera3D);
        }

        private void DrawContext_OnPreRender(DrawContext drawContext, CommandBuffer commandBuffer)
        {
            if (this.localUpdateDone)
            {
                this.arrService.BlitRemoteFrame(drawContext as CameraDrawContext, commandBuffer);
            }
        }

        private void RenderManager_OnPostRender(object sender, RenderManager e)
        {
            if (this.localUpdateDone)
            {
                this.arrService.Reproject();
            }
        }
    }
}
