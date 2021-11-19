using System;
using System.Reflection;
using WaveEngine.AzureRemoteRendering;
using WaveEngine.Editor.Extension;
using WaveEngine.Editor.Extension.Attributes;

namespace AzureRemoteRendering_Demo.Editor
{
    [CustomPropertyEditor(typeof(ARRSessionConfiguration))]
    public class SessionConfigurationEditor : PropertyEditor<ARRSessionConfiguration>
    {
        private ARRSessionConfiguration property;

        private Type propertyType;

        protected override void Loaded()
        {
            this.property = this.GetMemberValue();
            this.propertyType = typeof(ARRSessionConfiguration);
        }

        public override void GenerateUI()
        {
            this.propertyPanelContainer.AddLabel("Title", "RemoteRenderingClientAccount");
            this.AddProperty(this.propertyType.GetProperty(nameof(ARRSessionConfiguration.AccountDomain)));
            this.AddProperty(this.propertyType.GetProperty(nameof(ARRSessionConfiguration.RemoteRenderingDomain)));
            this.AddProperty(this.propertyType.GetProperty(nameof(ARRSessionConfiguration.AccountId)));
            this.AddProperty(this.propertyType.GetProperty(nameof(ARRSessionConfiguration.AccountKey)));
            this.AddProperty(this.propertyType.GetProperty(nameof(ARRSessionConfiguration.AccessToken)));
            this.AddProperty(this.propertyType.GetProperty(nameof(ARRSessionConfiguration.AuthenticationToken)));
        }

        private void AddProperty(MemberInfo memberInfo)
        {
            this.propertyPanelContainer.Add(memberInfo, this.property);
        }
    }
}
