// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using Microsoft.Azure.RemoteRendering;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework;

namespace WaveEngine.AzureRemoteRendering.Components
{
    /// <summary>
    /// Wraps a <see cref="CutPlaneComponent"/> in a proxy <see cref="Component"/> for interoperability.
    /// </summary>
    /// <remarks>
    /// This component uses the bind mode <see cref="ARRComponentBindMode.FromProxyToRemote"/> by default.
    /// </remarks>
    public class ARRCutPlaneComponent : ARRTypedComponentBase<CutPlaneComponent>
    {
        private static readonly Color DefaultFadeColor = new Color(13, 26, 255, 255);

        private static readonly float DefaultFadeLength = 0.3f;

        private Color? fadeColorOverride;

        /// <summary>
        /// Gets or sets the color towards which pixels closer to the cut plane will be faded.
        /// </summary>
        public Color FadeColor
        {
            get => this.GetProperty(this.fadeColorOverride, DefaultFadeColor, () => this.RemoteComponent.FadeColor.ToWave());
            set => this.SetProperty(value, ref this.fadeColorOverride, DefaultFadeColor, (x) => this.RemoteComponent.FadeColor = x.ToRemoteColor4Ub());
        }

        private float? fadeLengthOverride;

        /// <summary>
        /// Gets or sets the distance in local units over which the cut plane will fade
        /// the original pixel color.
        /// </summary>
        public float FadeLength
        {
            get => this.GetProperty(this.fadeLengthOverride, DefaultFadeLength, () => this.RemoteComponent.FadeLength);
            set => this.SetProperty(value, ref this.fadeLengthOverride, DefaultFadeLength, (x) => this.RemoteComponent.FadeLength = x);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ARRCutPlaneComponent"/> class.
        /// </summary>
        public ARRCutPlaneComponent()
        : base(ObjectType.CutPlaneComponent)
        {
            this.BindMode = ARRComponentBindMode.FromProxyToRemote;
        }

        /// <inheritdoc />
        protected override void OnRemoteComponentCreated()
        {
            this.OverrideRemoteProperty(this.fadeColorOverride, (value) => this.FadeColor = value);
            this.OverrideRemoteProperty(this.fadeLengthOverride, (value) => this.FadeLength = value);

            base.OnRemoteComponentCreated();
        }
    }
}
