// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using Microsoft.Azure.RemoteRendering;
using WaveEngine.Framework;
using WaveEngine.Mathematics;

namespace WaveEngine.AzureRemoteRendering.Components
{
    /// <summary>
    ///  Wraps a <see cref="SpotLightComponent"/> in a proxy <see cref="Component"/> for interoperability.
    ///
    /// <para>
    /// Component that represents a dynamic spot light. In contrast to a point light, the light is not emitted
    /// in all directions but instead constrained to the shape of a cone. The orientation of the cone is defined
    /// by the owner entity's forward vector. Typical use cases for spotlights are flashlights.
    /// </para>
    /// </summary>
    /// <remarks>
    /// This component uses the bind mode <see cref="ARRComponentBindMode.FromProxyToRemote"/> by default.
    /// </remarks>
    public class ARRSpotLightComponent : ARRLightComponentBase<SpotLightComponent>
    {
        /// <summary>
        /// The default falloff exponent value set by the ARR API.
        /// </summary>
        protected static readonly float DefaultFalloffExponent = 1f;

        /// <summary>
        /// The default spot angle vector in degrees value set by the ARR API.
        /// </summary>
        protected static readonly Vector2 DefaultSpotAngleDeg = new Vector2(25, 35);

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

        private float? falloffExponentOverride;

        /// <summary>
        /// Gets or sets a value that defines the characteristic of the falloff between the inner and the outer cone angle.
        /// A higher value results in a sharper transition between inner and outer cone angle.
        /// The default of 1.0 defines a linear falloff.
        /// </summary>
        public float FalloffExponent
        {
            get => this.GetProperty(this.falloffExponentOverride, DefaultFalloffExponent, () => this.RemoteComponent.FalloffExponent);
            set => this.SetProperty(value, ref this.falloffExponentOverride, DefaultFalloffExponent, (x) => this.RemoteComponent.FalloffExponent = x);
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

        private Texture projected2dTextureOverride;

        /// <summary>
        /// Gets or sets a projected texture by this lights. The texture's color is modulated with the light's color.
        /// </summary>
        public Texture Projected2dTexture
        {
            get => this.GetProperty(this.projected2dTextureOverride, () => this.RemoteComponent.Projected2dTexture);
            set => this.SetProperty(value, ref this.projected2dTextureOverride, (x) => this.RemoteComponent.Projected2dTexture = x);
        }

        private Vector2? spotAngleDegOverride;

        /// <summary>
        /// Gets or sets an interval that defines the inner and outer angle of the spot light cone both measured in degree.
        /// Everything within the inner angle is illuminated by the full brightness of the spot light source and a falloff
        /// is applied towards the outer angle that generates a penumbra-like effect.
        /// </summary>
        public Vector2 SpotAngleDeg
        {
            get => this.GetProperty(this.spotAngleDegOverride, DefaultSpotAngleDeg, () => this.RemoteComponent.SpotAngleDeg.ToWave());
            set => this.SetProperty(value, ref this.spotAngleDegOverride, DefaultSpotAngleDeg, (x) => this.RemoteComponent.SpotAngleDeg = x.ToRemote());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ARRSpotLightComponent"/> class.
        /// </summary>
        public ARRSpotLightComponent()
            : base(ObjectType.SpotLightComponent)
        {
        }

        /// <inheritdoc />
        protected override void OnRemoteComponentCreated()
        {
            this.OverrideRemoteProperty(this.attenuationCutoffOverride, (value) => this.AttenuationCutoff = value);
            this.OverrideRemoteProperty(this.falloffExponentOverride, (value) => this.FalloffExponent = value);
            this.OverrideRemoteProperty(this.radiusOverride, (value) => this.Radius = value);
            this.OverrideRemoteProperty(this.projected2dTextureOverride, (value) => this.Projected2dTexture = value);
            this.OverrideRemoteProperty(this.spotAngleDegOverride, (value) => this.SpotAngleDeg = value);

            base.OnRemoteComponentCreated();
        }
    }
}
