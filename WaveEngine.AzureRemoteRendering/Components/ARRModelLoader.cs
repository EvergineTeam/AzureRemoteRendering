// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Threading.Tasks;
using WaveEngine.Framework;
using WaveEngine.Mathematics;
using ARREntity = Microsoft.Azure.RemoteRendering.Entity;

namespace WaveEngine.AzureRemoteRendering.Components
{
    /// <summary>
    /// Component that handles the load process of a remote model. The remote model will be automatically loaded when the
    /// component <see cref="AttachableObject.IsEnabled"/> and <see cref="AzureRemoteRenderingService.IsCurrentSessionConnected"/>.
    /// <para/>
    /// </summary>
    /// <remarks>
    /// When combined with a <see cref="ARREntitySync"/> component,
    /// the loaded remote model entity will be automatically binded to the <see cref="Component.Owner"/> of this component.
    /// </remarks>
    public class ARRModelLoader : Component
    {
        /// <summary>
        /// The <see cref="AzureRemoteRenderingService"/> dependency used by this component.
        /// </summary>
        [BindService]
        protected AzureRemoteRenderingService arrService;

        /// <summary>
        /// The <see cref="ARREntitySync"/> component dependency that contains the remote entity
        /// containing the remote model.
        /// </summary>
        [BindComponent(isRequired: false)]
        protected ARREntitySync entitySync;

        private float progress;

        private Task loadingTask;

        /// <summary>
        /// Gets or sets the URL for the model. Either builtin:// or a URL pointing at a converted model.
        /// Both raw (public) URIs to blob store and URIs with embedded SAS tokens to blob store are supported.
        /// </summary>
        public string Url { get; set; } = "builtin://Engine";

        /// <summary>
        /// Gets a percentage of the loading progress.
        /// </summary>
        public float Progress
        {
            get => this.progress;
            private set
            {
                if (this.progress != value)
                {
                    this.progress = value;
                    this.ProgressChanged?.Invoke(this, value);
                }
            }
        }

        /// <summary>
        /// Gets the remote entity that represents the model.
        /// </summary>
        public ARREntity RemoteEntity { get; private set; }

        /// <summary>
        /// Gets the remote entity model local bounding box.
        /// <para>
        /// This property has a valid value once <see cref="IsModelLoaded"/> is <c>true</c>.
        /// </para>
        /// </summary>
        public BoundingBox? LocalBounds { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the model is loaded in <see cref="AzureRemoteRenderingService.CurrentSession"/>.
        /// </summary>
        public bool IsModelLoaded => this.RemoteEntity != null;

        /// <summary>
        /// Occurs whenever the loading <see cref="Progress"/> is changed.
        /// </summary>
        public event EventHandler<float> ProgressChanged;

        /// <summary>
        /// Occurs once the <see cref="RemoteEntity"/> has been loaded.
        /// </summary>
        public event EventHandler Loaded;

        /// <inheritdoc/>
        protected override void OnActivated()
        {
            base.OnActivated();

            this.arrService.ConnectionStatusChanged += this.ArrService_ConnectionStatusChanged;
            this.ArrService_ConnectionStatusChanged(this, this.arrService.ConnectionStatus);
        }

        /// <inheritdoc/>
        protected override void OnDeactivated()
        {
            base.OnDeactivated();

            this.arrService.ConnectionStatusChanged -= this.ArrService_ConnectionStatusChanged;
        }

        /// <inheritdoc/>
        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.DestroyRemoteEntity();
        }

        private async void ArrService_ConnectionStatusChanged(object sender, ARRConnectionStatus status)
        {
            if (status != ARRConnectionStatus.Connected)
            {
                this.DestroyRemoteEntity();
                return;
            }

            if (this.RemoteEntity != null || this.loadingTask != null)
            {
                return;
            }

            this.loadingTask = this.LoadRemoteEntity();
            await this.loadingTask;
            this.loadingTask = null;
        }

        private void DestroyRemoteEntity()
        {
            if (this.entitySync == null ||
                !this.entitySync.IsRemoteEntityValid ||
                this.entitySync.RemoteEntity != this.RemoteEntity)
            {
                // Otherwise ARREntitySync will care about the remote entity destruction.
                this.RemoteEntity?.Destroy();
            }

            this.RemoteEntity = null;
            this.LocalBounds = null;
            this.Progress = 0f;
        }

        private async Task LoadRemoteEntity()
        {
            const string errorMessageHeader = "An error occurred while loading the remote model: ";
            if (!Uri.IsWellFormedUriString(this.Url, UriKind.Absolute))
            {
                throw new InvalidOperationException($"{errorMessageHeader}The URL {this.Url} is not valid.");
            }

            try
            {
                var loadModelProgress = new Progress<float>((progress) => this.Progress = progress);
                var parentEntitySync = this.Owner.FindComponentInParents<ARREntitySync>(skipOwner: true);
                var parentRemoteEntity = parentEntitySync?.IsRemoteEntityValid == true ? parentEntitySync.RemoteEntity : null;
                var remoteEntity = await this.arrService.LoadModelFromSASAsync(this.Url, parentRemoteEntity, loadModelProgress);
                this.entitySync?.Bind(remoteEntity, false);
                this.entitySync?.SyncToRemote();

                var localBounds = await remoteEntity.QueryLocalBoundsAsync().AsTask();
                this.LocalBounds = localBounds.ToWave();

                this.RemoteEntity = remoteEntity;
                this.Loaded?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"{errorMessageHeader}{ex}", ex);
            }
        }
    }
}
