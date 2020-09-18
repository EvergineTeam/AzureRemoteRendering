using System;
using System.Reflection;
using WaveEngine.AzureRemoteRendering;
using WaveEngine.Editor.Extension;
using WaveEngine.Editor.Extension.Attributes;

namespace AzureRemoteRendeging_Demo.Editor
{
    [CustomPropertyEditor(typeof(ARRFrontendAccountInfo))]
    public class AzureFrontendAccountInfoEditor : PropertyEditor<ARRFrontendAccountInfo>
    {
        private ARRFrontendAccountInfo property;

        private Type propertyType;

        protected override void Loaded()
        {
            this.property = this.GetMemberValue();
            this.propertyType = typeof(ARRFrontendAccountInfo);
        }

        public override void GenerateUI()
        {
            this.propertyPanelContainer.AddLabel("Title", "AzureFrontendAccount");
            this.AddProperty(this.propertyType.GetField(nameof(ARRFrontendAccountInfo.AccountDomain)));
            this.AddProperty(this.propertyType.GetField(nameof(ARRFrontendAccountInfo.AccountId)));
            this.AddProperty(this.propertyType.GetField(nameof(ARRFrontendAccountInfo.AccountKey)));
            this.AddProperty(this.propertyType.GetField(nameof(ARRFrontendAccountInfo.AccessToken)));
            this.AddProperty(this.propertyType.GetField(nameof(ARRFrontendAccountInfo.AuthenticationToken)));
        }

        private void AddProperty(FieldInfo fieldInfo)
        {
            this.propertyPanelContainer.Add(fieldInfo, this.property);
        }
    }
}
