// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using Microsoft.Azure.RemoteRendering;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework;

namespace WaveEngine.AzureRemoteRendering.Components
{
    /// <summary>
    ///  Wraps a <see cref="HierarchicalStateOverrideComponent"/> in a proxy <see cref="Component"/>
    ///  for interoperability.
    ///
    /// <para>
    /// Controls the visualization of sub-hierarchies of remote entities.
    /// </para>
    /// </summary>
    /// <remarks>
    /// This component uses the bind mode <see cref="ARRComponentBindMode.FromProxyToRemote"/> by default.
    /// </remarks>
    public class ARRHierarchicalStateOverrideComponent : ARRTypedComponentBase<HierarchicalStateOverrideComponent>
    {
        private static readonly HierarchicalEnableState DefaultEnableState = HierarchicalEnableState.InheritFromParent;

        private static readonly HierarchicalStates DefaulHierarchicalState = HierarchicalStates.None;

        private static readonly Color DefaultTintColor = new Color(255, 255, 255, 0);

        private HierarchicalEnableState? disableCollisionStateOverride;

        /// <summary>
        /// Gets or sets a value indicating how the <see cref="HierarchicalStates.DisableCollision"/> flag
        /// is set on this hierarchy level.
        /// <see cref="HierarchicalStates.DisableCollision"/> flag indicates that the geometry is exempt from
        /// spatial queries (<see cref="AzureRemoteRenderingService.RayCastQueryAsync(RayCast)"/>).
        /// The <see cref="HierarchicalStates.Hidden"/> flag doesn't affect the collision state flag,
        /// so these two flags are often set together.
        /// </summary>
        public HierarchicalEnableState DisableCollisionState
        {
            get => this.GetProperty(this.disableCollisionStateOverride, DefaultEnableState, () => this.RemoteComponent.DisableCollisionState);
            set => this.SetProperty(value, ref this.disableCollisionStateOverride, DefaultEnableState, (x) => this.RemoteComponent.DisableCollisionState = x);
        }

        private HierarchicalStates? flagsOverride;

        /// <summary>
        /// Gets or sets rendering flags set on this component.
        /// </summary>
        public HierarchicalStates Flags
        {
            get => this.GetProperty(this.flagsOverride, DefaulHierarchicalState, () => this.RemoteComponent.Flags);
            set => this.SetProperty(value, ref this.flagsOverride, DefaulHierarchicalState, (x) => this.RemoteComponent.Flags = x);
        }

        private HierarchicalEnableState? hiddenStateOverride;

        /// <summary>
        /// Gets or sets a value indicating if the respective meshes in the scene graph are hidden or shown.
        /// </summary>
        public HierarchicalEnableState HiddenState
        {
            get => this.GetProperty(this.hiddenStateOverride, DefaultEnableState, () => this.RemoteComponent.HiddenState);
            set => this.SetProperty(value, ref this.hiddenStateOverride, DefaultEnableState, (x) => this.RemoteComponent.HiddenState = x);
        }

        private HierarchicalStates? overrideFlagsOverride;

        /// <summary>
        /// Gets or sets the render property flags to override on this component.
        /// </summary>
        public HierarchicalStates OverrideFlags
        {
            get => this.GetProperty(this.overrideFlagsOverride, DefaulHierarchicalState, () => this.RemoteComponent.OverrideFlags);
            set => this.SetProperty(value, ref this.overrideFlagsOverride, DefaulHierarchicalState, (x) => this.RemoteComponent.OverrideFlags = x);
        }

        private HierarchicalEnableState? seeThroughStateOverride;

        /// <summary>
        /// Gets or sets a value indicating if the geometry is rendered semi-transparently, for example to reveal the inner parts of an object.
        /// </summary>
        public HierarchicalEnableState SeeThroughState
        {
            get => this.GetProperty(this.seeThroughStateOverride, DefaultEnableState, () => this.RemoteComponent.SeeThroughState);
            set => this.SetProperty(value, ref this.seeThroughStateOverride, DefaultEnableState, (x) => this.RemoteComponent.SeeThroughState = x);
        }

        private HierarchicalEnableState? selectedStateOverride;

        /// <summary>
        /// Gets or sets a value indicating if the geometry is rendered with a selection outline.
        /// </summary>
        public HierarchicalEnableState SelectedState
        {
            get => this.GetProperty(this.selectedStateOverride, DefaultEnableState, () => this.RemoteComponent.SelectedState);
            set => this.SetProperty(value, ref this.selectedStateOverride, DefaultEnableState, (x) => this.RemoteComponent.SelectedState = x);
        }

        private Color? tintColorOverride;

        /// <summary>
        /// Gets or sets a value indicating the tint color that will be applied by a rendered object.
        /// When overriding, the alpha portion of the tint color defines the weight of the tinting effect:
        /// If set to 0.0, no tint color is visible and if set to 1.0 the object will be rendered with pure tint color.
        /// For in-between values, the final color will be mixed with the tint color.
        /// The tint color can be changed on a per-frame basis to achieve a color animation.
        /// </summary>
        public Color TintColor
        {
            get => this.GetProperty(this.tintColorOverride, DefaultTintColor, () => this.RemoteComponent.TintColor.ToWave());
            set => this.SetProperty(value, ref this.tintColorOverride, DefaultTintColor, (x) => this.RemoteComponent.TintColor = x.ToRemoteColor4Ub());
        }

        private HierarchicalEnableState? useTintColorStateOverride;

        /// <summary>
        /// Gets or sets a value indicating how the <see cref="TintColor"/> property affects on this hierarchy level.
        /// </summary>
        public HierarchicalEnableState UseTintColorState
        {
            get => this.GetProperty(this.useTintColorStateOverride, DefaultEnableState, () => this.RemoteComponent.UseTintColorState);
            set => this.SetProperty(value, ref this.useTintColorStateOverride, DefaultEnableState, (x) => this.RemoteComponent.UseTintColorState = x);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ARRHierarchicalStateOverrideComponent"/> class.
        /// </summary>
        public ARRHierarchicalStateOverrideComponent()
            : base(ObjectType.HierarchicalStateOverrideComponent)
        {
            this.BindMode = ARRComponentBindMode.FromProxyToRemote;
        }

        /// <inheritdoc />
        protected override void OnRemoteComponentCreated()
        {
            this.OverrideRemoteProperty(this.disableCollisionStateOverride, (value) => this.DisableCollisionState = value);
            this.OverrideRemoteProperty(this.flagsOverride, (value) => this.Flags = value);
            this.OverrideRemoteProperty(this.hiddenStateOverride, (value) => this.HiddenState = value);
            this.OverrideRemoteProperty(this.overrideFlagsOverride, (value) => this.OverrideFlags = value);
            this.OverrideRemoteProperty(this.seeThroughStateOverride, (value) => this.SeeThroughState = value);
            this.OverrideRemoteProperty(this.selectedStateOverride, (value) => this.SelectedState = value);
            this.OverrideRemoteProperty(this.tintColorOverride, (value) => this.TintColor = value);
            this.OverrideRemoteProperty(this.useTintColorStateOverride, (value) => this.UseTintColorState = value);

            base.OnRemoteComponentCreated();
        }
    }
}
