// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using Microsoft.Azure.RemoteRendering;
using Evergine.Framework;
using Evergine.Mathematics;

namespace Evergine.AzureRemoteRendering.Components
{
    /// <summary>
    ///  Wraps a <see cref="PointLightComponent"/> in a proxy <see cref="Component"/> for interoperability.
    ///
    /// <para>
    /// A point light simulates light emitted equally in all directions form a point (or small sphere/tube) in space.
    /// Point light components can be used to create local light effects such as light from bulbs.
    /// </para>
    /// </summary>
    /// <remarks>
    /// This component uses the bind mode <see cref="ARRComponentBindMode.FromProxyToRemote"/> by default.
    /// </remarks>
    public class ARRPointLightComponent : ARRLightComponentBase<PointLightComponent>
    {
        /// <summary>
        /// The default length value set by the ARR API.
        /// </summary>
        protected static readonly float DefaultLength = 0f;

        private Vector2? attenuationCutoffOverride;

        /// <summary>
        /// Gets or sets a value that defines a custom interval of min/max distances over which the light's attenuated
        /// intensity is scaled linearly down to 0.
        /// This feature can be used to enforce a smaller range of influence of a specific light.
        /// If not defined (default), these values are implicitly derived from the light's intensity.
        /// </summary>
        public Vector2 AttenuationCutoff
        {
            get => this.GetProperty(this.attenuationCutoffOverride, DefaultAttenuationCutoff, () => this.RemoteComponent.AttenuationCutoff.ToWave());
            set => this.SetProperty(value, ref this.attenuationCutoffOverride, DefaultAttenuationCutoff, (x) => this.RemoteComponent.AttenuationCutoff = x.ToRemote());
        }

        private float? lengthOverride;

        /// <summary>
        /// Gets or sets a value that when >0 (and also <see cref="Radius"/> > 0) defines the length of a light emitting tube.
        /// Use case is a neon tube.
        /// </summary>
        public float Length
        {
            get => this.GetProperty(this.lengthOverride, DefaultLength, () => this.RemoteComponent.Length);
            set => this.SetProperty(value, ref this.lengthOverride, DefaultLength, (x) => this.RemoteComponent.Length = x);
        }

        private Texture projectedCubeMapOverride;

        /// <summary>
        /// Gets or sets a <see cref="TextureType.CubeMap"/> texture that is projected using the orientation of the light.
        /// The cubemap's color is modulated with the light's color.
        /// </summary>
        public Texture ProjectedCubeMap
        {
            get => this.GetProperty(this.projectedCubeMapOverride, () => this.RemoteComponent.ProjectedCubeMap);
            set => this.SetProperty(value, ref this.projectedCubeMapOverride, (x) => this.RemoteComponent.ProjectedCubeMap = x);
        }

        private float? radiusOverride;

        /// <summary>
        /// Gets or sets a value that when >0 makes the light emitting shape of the light source
        /// is a sphere of given radius as opposed to a point.
        /// This shape for instance affects the appearance of specular highlights.
        /// </summary>
        public float Radius
        {
            get => this.GetProperty(this.radiusOverride, DefaultRadius, () => this.RemoteComponent.Radius);
            set => this.SetProperty(value, ref this.radiusOverride, DefaultRadius, (x) => this.RemoteComponent.Radius = x);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ARRPointLightComponent"/> class.
        /// </summary>
        public ARRPointLightComponent()
            : base(ObjectType.PointLightComponent)
        {
        }

        /// <inheritdoc />
        protected override void OnRemoteComponentCreated()
        {
            this.OverrideRemoteProperty(this.attenuationCutoffOverride, (value) => this.AttenuationCutoff = value);
            this.OverrideRemoteProperty(this.lengthOverride, (value) => this.Length = value);
            this.OverrideRemoteProperty(this.projectedCubeMapOverride, (value) => this.ProjectedCubeMap = value);
            this.OverrideRemoteProperty(this.radiusOverride, (value) => this.Radius = value);

            base.OnRemoteComponentCreated();
        }
    }
}
