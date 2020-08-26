// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using Microsoft.Azure.RemoteRendering;
using WaveEngine.Framework;

namespace WaveEngine.AzureRemoteRendering.Components
{
    /// <summary>
    ///  Wraps a <see cref="DirectionalLightComponent"/> in a proxy <see cref="Component"/> for interoperability.
    ///
    /// <para>
    /// A directional light simulates a light source that is infinitely far away. Accordingly, unlike point lights
    /// and spot lights, the position of a directional light is ignored.
    /// The direction of the parallel light rays is defined by the negative z-axis of the owner object.
    /// There are no additional directional light-specific properties.
    /// </para>
    /// </summary>
    /// <remarks>
    /// This component uses the bind mode <see cref="ARRComponentBindMode.FromProxyToRemote"/> by default.
    /// </remarks>
    public class ARRDirectionalLightComponent : ARRLightComponentBase<DirectionalLightComponent>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ARRDirectionalLightComponent"/> class.
        /// </summary>
        public ARRDirectionalLightComponent()
            : base(ObjectType.DirectionalLightComponent)
        {
        }
    }
}
