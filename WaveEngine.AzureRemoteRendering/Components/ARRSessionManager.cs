// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using Microsoft.Azure.RemoteRendering;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WaveEngine.Framework;

namespace WaveEngine.AzureRemoteRendering.Components
{
    /// <summary>
    /// Component that handles the initialization of <see cref="AzureRemoteRenderingService"/> and the creation and
    /// connection of a <see cref="RenderingSession"/>.
    /// </summary>
    public class ARRSessionManager : Component
    {
        /// <summary>
        /// The <see cref="AzureRemoteRenderingService"/> dependency used by this component.
        /// </summary>
        [BindService]
        protected AzureRemoteRenderingService arrService;

        /// <summary>
        /// Gets or sets the account information and domain to associate an <see cref="RemoteRenderingClient"/> instance with.
        /// </summary>
        public ARRSessionConfiguration AccountInfo { get; set; } = new ARRSessionConfiguration();

        /// <summary>
        /// Gets or sets a value indicating whether a <see cref="RenderingSession"/> will be reused or created once this component is activated.
        /// </summary>
        public bool CreateAndConnectAutomatically { get; set; } = true;

        /// <summary>
        /// Gets or sets a timeout value when the VM will be decommissioned automatically. The expiration time is VM start time + MaxLease.
        /// </summary>
        public TimeSpan InitialLeaseTime { get; set; } = TimeSpan.FromHours(1);

        /// <summary>
        /// Gets or sets the session VM size.
        /// </summary>
        public RenderingSessionVmSize VMSize { get; set; } = RenderingSessionVmSize.Standard;

        /// <inheritdoc />
        protected async override void OnActivated()
        {
            base.OnActivated();

            if (Application.Current.IsEditor ||
                !this.arrService.IsAttached)
            {
                return;
            }

            this.arrService.Initialize(this.AccountInfo);

            if (this.CreateAndConnectAutomatically &&
                await this.ReuseOrCreateSessionAsync() &&
                !await this.arrService.ConnectAsync())
            {
                Trace.TraceError("[ARR] Error connecting to session");
            }
        }

        /// <inheritdoc />
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            this.arrService.Disconnect();
        }

        private async Task<bool> ReuseOrCreateSessionAsync()
        {
            var currentSessions = (await this.arrService.GetCurrentRenderingSessionsAsync()).SessionProperties;
            var existingSessionId = currentSessions?.FirstOrDefault(x => x.Size == this.VMSize).Id;

            var createNewSession = true;
            if (!string.IsNullOrEmpty(existingSessionId))
            {
                if (await this.arrService.OpenRenderingSessionAsync(existingSessionId))
                {
                    createNewSession = false;
                }
                else
                {
                    Trace.TraceWarning($"[{nameof(ARRSessionManager)}] Failed to open existing rendering session \"{existingSessionId}\"");
                }
            }

            if (createNewSession)
            {
                if (!await this.arrService.CreateNewRenderingSessionAsync(this.InitialLeaseTime, this.VMSize))
                {
                    Trace.TraceError($"[{nameof(ARRSessionManager)}] Failed to create rendering session");
                    return false;
                }
            }

            return true;
        }
    }
}
