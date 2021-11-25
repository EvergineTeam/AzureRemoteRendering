using Evergine.AzureRemoteRendering.Components;
using Evergine.Framework;
using Evergine.Framework.Physics3D;
using Evergine.MRTK.SDK.Features.UX.Components.BoundingBox;

namespace AzureRemoteRendering_Demo.GUI
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
                this.boxCollider3D.Size = Evergine.Mathematics.Vector3.Zero;
                this.boxCollider3D.Offset = Evergine.Mathematics.Vector3.Zero;
            }

            this.boundingBox.AutoCalculate = false;
            boundingBox.RotationHandlePrefab.Refresh();
        }
    }
}
