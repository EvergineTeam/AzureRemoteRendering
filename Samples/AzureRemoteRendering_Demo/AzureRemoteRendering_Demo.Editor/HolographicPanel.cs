﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Evergine.Editor.Extension;
using Evergine.Editor.Extension.Attributes;
using Evergine.MRTK.Effects;

namespace Evergine_MRTK_Demo.Editor
{
    [CustomPanelEditor(typeof(HoloGraphic))]
    public class HolographicPanel : PanelEditor
    {
        private Dictionary<string, MemberInfo> members;

        public new HoloGraphic Instance => (HoloGraphic)base.Instance;

        protected override void Loaded()
        {
            base.Loaded();

            this.Instance.Material.MaterialStateChanged += this.Material_MaterialStateChanged;
            this.members = PanelEditor.GetMembersForType(this.Instance.GetType());
        }

        private void Material_MaterialStateChanged(object sender, EventArgs e)
        {
            this.propertyPanelContainer.InvalidateLayout();
        }

        public override void GenerateUI()
        {
            this.AddMember(nameof(HoloGraphic.Albedo));

            if (this.AddDirectiveCheckbox("Albedo Map", HoloGraphic.AlbedoMapDirective))
            {
                this.AddMember(nameof(HoloGraphic.Texture));
                this.AddMember(nameof(HoloGraphic.Sampler));
                this.AddMember(nameof(HoloGraphic.Parameters_Tiling));
                this.AddMember(nameof(HoloGraphic.Parameters_Offset));
            }

            // Rendering Options
            if (this.AddDirectiveCheckbox("Directional Light", HoloGraphic.DirectionalLightDirective))
            {
                this.AddMember(nameof(HoloGraphic.Metallic));
                this.AddMember(nameof(HoloGraphic.Smoothness));
            }

            // Fluent Options
            var hoverLight = this.AddDirectiveCheckbox("Hover Light", HoloGraphic.HoverLightDirective);
            if (hoverLight &&
                this.AddDirectiveCheckbox("Override Color", HoloGraphic.HoverColorOverrideDirective))
            {
                this.AddMember(nameof(HoloGraphic.HoverColorOverride));
            }

            var proximityLight = this.AddDirectiveCheckbox("Proximity Light", HoloGraphic.ProximityLightDirective);
            if (proximityLight)
            {
                this.AddDirectiveCheckbox("Two Sided", HoloGraphic.ProximityLightTwoSidedDirective);
                this.AddDirectiveCheckbox("Substractive", HoloGraphic.ProximityLightSubtractiveDirective);

                if (this.AddDirectiveCheckbox("Override Color", HoloGraphic.ProximityLightColorOverrideDirective))
                {
                    this.AddMember(nameof(HoloGraphic.ProximityLightCenterColorOverride));
                    this.AddMember(nameof(HoloGraphic.ProximityLightMiddleColorOverride));
                    this.AddMember(nameof(HoloGraphic.ProximityLightOuterColorOverride));
                }
            }

            var borderLight = this.AddDirectiveCheckbox("Border Light", HoloGraphic.BorderLightDirective);
            if (borderLight)
            {
                this.AddMember(nameof(HoloGraphic.BorderWidth));
                this.AddMember(nameof(HoloGraphic.BorderMinValue));
            }

            if (hoverLight || proximityLight || borderLight)
            {
                this.AddMember(nameof(HoloGraphic.FluentLightIntensity));
            }

            var roundCorners = this.AddDirectiveCheckbox("Round Corners", HoloGraphic.RoundCornersDirective);
            if (roundCorners)
            {
                if (this.AddDirectiveCheckbox("Independent Corners", HoloGraphic.IndependentCornersDirective))
                {
                    this.AddMember(nameof(HoloGraphic.Parameters_RoundCornersRadious));
                }
                else
                {
                    this.AddMember(nameof(HoloGraphic.RoundCornerRadius));
                }

                this.AddMember(nameof(HoloGraphic.RoundCornerMargin));

                // TODO: Cutoff property is at different position in Unity. For now, we put Cutoff here because
                // it is only used by ROUND_CORNERS directive
                this.AddMember(nameof(HoloGraphic.AlphaCutoff));
            }

            if (roundCorners || borderLight)
            {
                this.AddMember(nameof(HoloGraphic.EdgeSmoothingValue));
            }

            if (this.AddDirectiveCheckbox("Inner Glow", HoloGraphic.InnerGlowDirective))
            {
                this.AddMember(nameof(HoloGraphic.InnerGlowColor));
                this.AddMember(nameof(HoloGraphic.InnerGlowPower));
            }

            // TODO: In Unity this is related to nearPlaneFade.
            // if (this.AddDirectiveCheckbox("Near Plane Fade", "NEAR_PLANE_FADE"))
            if (this.AddDirectiveCheckbox("Near Light Fade", HoloGraphic.NearLightFadeDirective))
            {
                // if (this.AddDirectiveCheckbox("Near Light Fade", "NEAR_LIGHT_FADE"))
                this.AddMember(nameof(HoloGraphic.FadeBeginDistance));
                this.AddMember(nameof(HoloGraphic.FadeCompleteDistance));
                this.AddMember(nameof(HoloGraphic.FadeMinValue));
            }


            foreach (var item in this.propertyPanelContainer.Properties)
            {
                item.Name = item.Name.Replace("Parameters_", string.Empty);
            }

            this.AddMember(nameof(HoloGraphic.LayerDescription));
        }

        private void AddMember(string memberName)
        {
            if (this.members.TryGetValue(memberName, out var memberInfo))
            {
                this.propertyPanelContainer.Add(memberInfo);
            }
        }

        private bool AddDirectiveCheckbox(string name, string directiveOn)
        {
            var directiveOff = directiveOn + "_OFF";
            this.propertyPanelContainer.AddBoolean(
                            directiveOn,
                            name,
                            false,
                            () => this.Instance.ActiveDirectivesNames.Contains(directiveOn),
                            (val) =>
                            {
                                var currentDirectives = this.Instance.ActiveDirectivesNames.ToList();
                                currentDirectives.Remove(directiveOn);
                                currentDirectives.Remove(directiveOff);
                                currentDirectives.Add(val ? directiveOn : directiveOff);
                                this.Instance.ActiveDirectivesNames = currentDirectives.ToArray();
                            });

            return this.Instance.ActiveDirectivesNames.Contains(directiveOn);
        }
    }
}
