using System;
using System.Text.RegularExpressions;
using Evergine.AzureRemoteRendering;
using Evergine.AzureRemoteRendering.Components;
using Evergine.Framework;
using Evergine.Components.Fonts;

namespace AzureRemoteRendering_Demo.GUI
{
    public class StatusComponent : Component
    {
        [BindService]
        protected AzureRemoteRenderingService arrService;

        [BindComponent(source: BindComponentSource.Children)]
        protected Text3DMesh text3D;

        [BindComponent(source: BindComponentSource.Scene)]
        protected ARRModelLoader modelLoader;

        protected override void OnActivated()
        {
            base.OnActivated();
            this.arrService.ConnectionStatusChanged += this.ArrService_ConnectionStatusChanged;
            this.modelLoader.ProgressChanged += this.ModelLoader_ProgressChanged;
            this.modelLoader.Loaded += this.ModelLoader_Loaded;
            this.RefreshStatusText();
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            this.arrService.ConnectionStatusChanged -= this.ArrService_ConnectionStatusChanged;
            this.modelLoader.ProgressChanged -= this.ModelLoader_ProgressChanged;
            this.modelLoader.Loaded -= this.ModelLoader_Loaded;
        }

        private void ArrService_ConnectionStatusChanged(object sender, ARRConnectionStatus e)
        {
            this.RefreshStatusText();
        }

        private void ModelLoader_ProgressChanged(object sender, float e)
        {
            this.RefreshStatusText();
        }

        private void ModelLoader_Loaded(object sender, EventArgs e)
        {
            this.RefreshStatusText();
        }

        private void RefreshStatusText()
        {
            var isModelLoaded = this.modelLoader.IsModelLoaded;
            var isConnected = this.arrService.IsCurrentSessionConnected;

            var showStatusText = !isModelLoaded || !isConnected;
            this.text3D.Owner.IsEnabled = showStatusText;

            if (!showStatusText)
            {
                return;
            }

            if (!isConnected)
            {
                this.text3D.Text = Regex.Replace($"{this.arrService.ConnectionStatus}", "(\\B[A-Z])", " $1");
            }
            else
            {
                this.text3D.Text = $"Loading model: {this.modelLoader.Progress:F}%";
            }
        }
    }
}
