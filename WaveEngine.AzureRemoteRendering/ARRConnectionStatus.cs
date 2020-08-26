// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

namespace WaveEngine.AzureRemoteRendering
{
    /// <summary>
    /// Indicates the rendering session connection status for <see cref="AzureRemoteRenderingService"/>.
    /// </summary>
    public enum ARRConnectionStatus
    {
        /// <summary>
        /// The service is not connected to any rendering session.
        /// </summary>
        Disconnected,

        /// <summary>
        /// The service is creating a new rendering session.
        /// </summary>
        CreatingSession,

        /// <summary>
        /// The service is waiting until the current rendering session is ready to connect.
        /// </summary>
        StartingSession,

        /// <summary>
        /// The service is connecting to the current rendering session.
        /// </summary>
        Connecting,

        /// <summary>
        /// The service is connected to the current rendering session.
        /// </summary>
        Connected,

        /// <summary>
        /// The service found a problem while connected to the current rendering session.
        /// </summary>
        ConnectionFailed,
    }
}
