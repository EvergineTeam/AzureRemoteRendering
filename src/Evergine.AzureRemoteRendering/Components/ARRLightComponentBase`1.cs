// Copyright © Evergine S.L. All rights reserved. Use is subject to license terms.

using Microsoft.Azure.RemoteRendering;
using Evergine.Common.Graphics;
using Evergine.Framework;
using Evergine.Mathematics;

namespace Evergine.AzureRemoteRendering.Components
{
    /// <summary>
    ///  Wraps a <see cref="LightComponentBase"/> in a proxy <see cref="Component"/> for interoperability.
    ///
    /// <para>
    /// Base class for all light components. Lights are added to the scene as components of respective
    /// type attached to a game object. Lights are considered as dynamic lights that contribute to the scene's
    /// lighting conditions on top of the global sky light. The game object serves as the spatial transform of the
    /// light source.
    /// This base class cannot be instantiated directly, instead one of the specific light types
    /// (Point/Spot/Directional) has to be used.
    /// </para>
    /// </summary>
    /// <typeparam name="TRemoteComponent">The ARR remote light component type.</typeparam>
    /// <remarks>
    /// This component uses the bind mode <see cref="ARRComponentBindMode.FromProxyToRemote"/> by default.
    /// </remarks>
    public abstract class ARRLightComponentBase<TRemoteComponent> : ARRTypedComponentBase<TRemoteComponent>
        where TRemoteComponent : LightComponentBase
    {
        /// <summary>
        /// The default fade color value set by the ARR API.
        /// </summary>
        protected static readonly Color DefaultFadeColor = new Color(255, 255, 255, 255);

        /// <summary>
        /// The default intensity value set by the ARR API.
        /// </summary>
        protected static readonly float DefaultIntensity = 10;

        /// <summary>
        /// The default attenuation cutoff value set by the ARR API.
        /// </summary>
        protected static readonly Vector2 DefaultAttenuationCutoff = Vector2.Zero;

        /// <summary>
        /// The default radius value set by the ARR API.
        /// </summary>
        protected static readonly float DefaultRadius = 0f;

        private Color? colorOverride;

        /// <summary>
        /// Gets or sets the color of the light in gamma color space. The alpha portion is ignored.
        /// </summary>
        public Color Color
        {
            get => this.GetProperty(this.colorOverride, DefaultFadeColor, () => this.RemoteComponent.Color.ToEvergine());
            set => this.SetProperty(value, ref this.colorOverride, DefaultFadeColor, (x) => this.RemoteComponent.Color = x.ToRemoteColor4Ub());
        }

        private float? intensityOverride;

        /// <summary>
        /// Gets or sets the intensity of the light. This value has no physical measure however it can
        /// be considered to be proportional to the physical power of the light source.
        /// If the light has a fall-off (point and spotlight) this value also defines the maximum range of light influence.
        /// An intensity of 1000 roughly has a range of 100 world units, but note this does not scale linearly.
        /// </summary>
        public float Intensity
        {
            get => this.GetProperty(this.intensityOverride, DefaultIntensity, () => this.RemoteComponent.Intensity);
            set => this.SetProperty(value, ref this.intensityOverride, DefaultIntensity, (x) => this.RemoteComponent.Intensity = x);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ARRLightComponentBase{TRemoteComponent}"/> class.
        /// </summary>
        /// <param name="type">The ARR light component type.</param>
        protected ARRLightComponentBase(ObjectType type)
            : base(type)
        {
            this.BindMode = ARRComponentBindMode.FromProxyToRemote;
        }

        /// <inheritdoc />
        protected override void OnRemoteComponentCreated()
        {
            this.OverrideRemoteProperty(this.colorOverride, (value) => this.Color = value);
            this.OverrideRemoteProperty(this.intensityOverride, (value) => this.Intensity = value);

            base.OnRemoteComponentCreated();
        }
    }
}
