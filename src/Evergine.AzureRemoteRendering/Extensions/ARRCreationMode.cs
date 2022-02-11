// Copyright © Evergine S.L. All rights reserved. Use is subject to license terms.
using Evergine.AzureRemoteRendering.Components;

namespace Evergine.AzureRemoteRendering
{
    /// <summary>
    /// When calling <see cref="EntityExtensions.FindOrCreateProxyEntity"/>, creations of the Components for
    /// ARR Components (<see cref="ARRCutPlaneComponent"/>, <see cref="ARRDirectionalLightComponent"/>, etc.)
    /// can be toggled through the <see cref="ARRCreationMode"/> enum.
    /// Creating the components will create additional performance overhead and, if the components
    /// will not be accessed, is not recommended.
    /// </summary>
    public enum ARRCreationMode
    {
        /// <summary>
        /// Create ARR Components associated with the remote object.
        /// </summary>
        CreateProxyComponents = 0,

        /// <summary>
        /// Do not create ARR Components associated with the remote object.
        /// </summary>
        DoNotCreateProxyComponents = 1,
    }
}
