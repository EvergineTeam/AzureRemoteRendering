// Copyright © Evergine S.L. All rights reserved. Use is subject to license terms.

using System;

namespace Evergine.AzureRemoteRendering
{
    /// <summary>
    /// Flags used by the extension method <see cref="EntityExtensions.RemoveProxyEntity"/>
    /// to indicate how to proceed with parent entities of the proxy entity.
    /// </summary>
    [Flags]
    public enum ARRRemoveProxyEntityFlags
    {
        /// <summary>
        /// None of the options.
        /// </summary>
        None = 0,

        /// <summary>
        /// Destroys all game objects up the hierarchy that would be left without children.
        /// </summary>
        DestroyEmptyParents = 1,

        /// <summary>
        /// Keeps the last parent with bound remote entity.
        /// </summary>
        KeepRemoteRoot = 2,
    }
}
