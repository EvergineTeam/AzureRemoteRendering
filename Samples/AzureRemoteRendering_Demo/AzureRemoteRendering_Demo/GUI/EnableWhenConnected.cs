using WaveEngine.AzureRemoteRendering;
using WaveEngine.Framework;

namespace AzureRemoteRendering_Demo.GUI
{
    /// <summary>
    /// This component enables the owner entity when <see cref="AzureRemoteRenderingService.IsCurrentSessionConnected"/>
    /// </summary>
    public class EnableWhenConnected : Component
    {
        [BindService]
        protected AzureRemoteRenderingService arrService;

        /// <inheritdoc />
        protected override bool OnAttached()
        {
            if (!base.OnAttached())
            {
                return false;
            }

            this.arrService.ConnectionStatusChanged += this.ArrService_ConnectionStatusChanged;
            this.RefreshEnabledState();

            return true;
        }

        /// <inheritdoc />
        protected override void OnDetach()
        {
            base.OnDetach();
            this.arrService.ConnectionStatusChanged -= this.ArrService_ConnectionStatusChanged;
        }

        private void ArrService_ConnectionStatusChanged(object sender, ARRConnectionStatus e)
        {
            this.RefreshEnabledState();
        }

        private void RefreshEnabledState()
        {
            if (Application.Current.IsEditor)
            {
                return;
            }

            this.Owner.IsEnabled = this.arrService.IsCurrentSessionConnected;
        }
    }
}
