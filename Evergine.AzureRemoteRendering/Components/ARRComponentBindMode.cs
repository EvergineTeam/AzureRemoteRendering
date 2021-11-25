// Copyright © Evergine S.L. All rights reserved. Use is subject to license terms.

namespace Evergine.AzureRemoteRendering.Components
{
    /// <summary>
    /// Indicate how a proxy component should be binding should be performed by the engine.
    /// </summary>
    public enum ARRComponentBindMode
    {
        /// <summary>
        /// The proxy component is binded to an existing remote component.
        /// <see cref="ARRComponentBase.Bind"/> must be called when the component is instanced without
        /// using the extension method <see cref="EntityExtensions.BindARRComponent"/>.
        /// </summary>
        FromRemoteToProxy = 0,

        /// <summary>
        /// The remote component is created and binded automatically by the proxy component.
        /// </summary>
        FromProxyToRemote,
    }
}
