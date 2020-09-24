using WaveEngine.AzureRemoteRendering.Components;
using WaveEngine.Framework;
using WaveEngine.Framework.Physics3D;
using WaveEngine.MRTK.SDK.Features.UX.Components.BoundingBox;

namespace AzureRemoteRendeging_Demo.GUI
{
    public class RemoteModelBoundingUpdater : Component
    {
        [BindComponent]
        protected ARRModelLoader modelLoader;

        [BindComponent]
        protected BoundingBox boundingBox;

        [BindComponent]
        protected BoxCollider3D boxCollider3D;

        /// <inheritdoc />
        protected override void OnActivated()
        {
            base.OnActivated();

            this.modelLoader.Loaded += this.ModelLoader_Loaded;
            this.RefreshBoundingBox();
        }

        /// <inheritdoc />
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            this.modelLoader.Loaded -= this.ModelLoader_Loaded;
        }

        private void ModelLoader_Loaded(object sender, System.EventArgs e)
        {
            this.RefreshBoundingBox();
        }

        private void RefreshBoundingBox()
        {
            if (Application.Current.IsEditor)
            {
                return;
            }

            if (this.modelLoader.LocalBounds.HasValue)
            {
                this.boxCollider3D.Size = this.modelLoader.LocalBounds.Value.HalfExtent * 2;
                this.boxCollider3D.Offset = this.modelLoader.LocalBounds.Value.Center;
            }
            else
            {
                this.boxCollider3D.Size = WaveEngine.Mathematics.Vector3.Zero;
                this.boxCollider3D.Offset = WaveEngine.Mathematics.Vector3.Zero;
            }

            this.boundingBox.AutoCalculate = false;
            this.boundingBox.CreateRig();
        }
    }
}
