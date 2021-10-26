// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace WaveEngine.AzureRemoteRendering.Components
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
        /// The <see cref="Camera3D"/> component dependency that will be used to render ARR remote frame.
        /// </summary>
        [BindComponent]
        protected Camera3D camera3D;

        private bool localUpdateDone;

        private bool isProxyCameraActive;

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
                this.camera3D.DrawContext.OnPostRender += this.DrawContext_OnPostRender;
            }
        }

        private void DeactivateProxyCamera()
        {
            if (this.isProxyCameraActive)
            {
                this.isProxyCameraActive = false;
                this.camera3D.DrawContext.OnCollect -= this.DrawContext_OnCollect;
                this.camera3D.DrawContext.OnPreRender -= this.DrawContext_OnPreRender;
                this.camera3D.DrawContext.OnPostRender -= this.DrawContext_OnPostRender;
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

        private void DrawContext_OnPostRender(DrawContext drawContext, CommandBuffer commandBuffer)
        {
            if (this.localUpdateDone)
            {
                this.arrService.Reproject();
            }
        }
    }
}
