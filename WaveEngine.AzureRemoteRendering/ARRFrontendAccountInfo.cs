// Copyright © Wave Engine S.L. All rights reserved. Use is subject to license terms.

using Microsoft.Azure.RemoteRendering;

namespace WaveEngine.AzureRemoteRendering
{
    /// <summary>
    /// Object to store Azure Frontend account info.
    /// </summary>
    public class ARRFrontendAccountInfo
    {
        /// <summary>
        /// Gets or sets the domain that will be used to generate sessions for the Azure Remote Rendering service.
        /// The domain is of the form [region].mixedreality.azure.com. Region should be selected based on the region currently closest to the user.
        /// For example, westus2.mixedreality.azure.com or westeurope.mixedreality.azure.com.
        /// </summary>
        public string AccountDomain { get; set; }

        /// <summary>
        /// Gets or sets the account-level ID for the Azure Remote Rendering service.
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// Gets or sets the account-level key for the Azure Remote Rendering service.
        /// Among AccountKey, AccessToken and AuthenticationToken, only one needs to be set.
        /// </summary>
        public string AccountKey { get; set; }

        /// <summary>
        /// Gets or sets the authentication token for Azure Active Directory (AAD).
        /// Among AccountKey, AccessToken and AuthenticationToken, only one needs to be set.
        /// </summary>
        public string AuthenticationToken { get; set; }

        /// <summary>
        /// Gets or sets the access token for the Azure Remote Rendering service.
        /// Among AccountKey, AccessToken and AuthenticationToken, only one needs to be set.
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets a value indicating whether all the requiredInformation is available.
        /// </summary>
        public bool HasRequiredInfo
        {
            get => !string.IsNullOrEmpty(this.AccountId) &&
                   !string.IsNullOrEmpty(this.AccountDomain) &&
                  (!string.IsNullOrEmpty(this.AccountKey) || !string.IsNullOrEmpty(this.AccessToken) || !string.IsNullOrEmpty(this.AuthenticationToken));
        }

        internal AzureFrontendAccountInfo Convert()
        {
            var result = new AzureFrontendAccountInfo();

            result.AccountDomain = this.AccountDomain;
            result.AccountId = this.AccountId;
            result.AccountKey = !string.IsNullOrEmpty(this.AccountKey) ? this.AccountKey : null;
            result.AuthenticationToken = !string.IsNullOrEmpty(this.AuthenticationToken) ? this.AuthenticationToken : null;
            result.AccessToken = !string.IsNullOrEmpty(this.AccessToken) ? this.AccessToken : null;

            return result;
        }
    }
}
