// Copyright © Evergine S.L. All rights reserved. Use is subject to license terms.

using Microsoft.Azure.RemoteRendering;

namespace Evergine.AzureRemoteRendering
{
    /// <summary>
    /// Object to store Azure Frontend account info.
    /// </summary>
    public class ARRSessionConfiguration
    {
        /// <summary>
        /// Gets or sets the domain that will be used to generate sessions for the Azure Remote Rendering service.
        /// </summary>
        /// <remarks>
        /// The domain is of the form [region].mixedreality.azure.com.
        /// [region] should be selected based on the region closest to the user. For example, westus2.mixedreality.azure.com
        /// or westeurope.mixedreality.azure.com.
        /// </remarks>
        public string AccountDomain { get; set; }

        /// <summary>
        /// Gets or sets the remote domain that will be used to generate sessions for the Azure Remote Rendering service.
        /// </summary>
        /// <remarks>
        /// The remote domain is of the form [region].mixedreality.azure.com.
        /// [region] should be selected based on the region closest to the user. For example, westus2.mixedreality.azure.com
        /// or westeurope.mixedreality.azure.com.
        /// </remarks>
        public string RemoteRenderingDomain { get; set; }

        /// <summary>
        /// Gets or sets the ID of the account that's being used with the Azure Remote Rendering service.
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// Gets or sets the key of the account that's being used with the Azure Remote Rendering service.
        /// </summary>
        /// <remarks>
        /// Only one of <see cref="AccountKey"/>, <see cref="AccessToken"/> or <see cref="AuthenticationToken"/> needs to be set.
        /// </remarks>
        public string AccountKey { get; set; }

        /// <summary>
        /// Gets or sets the authentication token for Azure Active Directory (AAD).
        /// </summary>
        /// <remarks>
        /// Only one of <see cref="AccountKey"/>, <see cref="AccessToken"/> or <see cref="AuthenticationToken"/> needs to be set.
        /// </remarks>
        public string AuthenticationToken { get; set; }

        /// <summary>
        /// Gets or sets the access token for the account that's being used with the Azure Remote Rendering service.
        /// </summary>
        /// <remarks>
        /// Only one of <see cref="AccountKey"/>, <see cref="AccessToken"/> or <see cref="AuthenticationToken"/> needs to be set.
        /// </remarks>
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets a value indicating whether all the required information is available.
        /// </summary>
        public bool HasRequiredInfo
        {
            get =>
                !string.IsNullOrEmpty(this.AccountId) &&
                !string.IsNullOrEmpty(this.RemoteRenderingDomain) &&
                !string.IsNullOrEmpty(this.AccountDomain) &&
                (!string.IsNullOrEmpty(this.AccountKey) || !string.IsNullOrEmpty(this.AccessToken) || !string.IsNullOrEmpty(this.AuthenticationToken));
        }

        internal SessionConfiguration Convert()
        {
            var result = new SessionConfiguration();

            result.AccountDomain = this.AccountDomain;
            result.RemoteRenderingDomain = this.RemoteRenderingDomain;
            result.AccountId = this.AccountId;
            result.AccountKey = !string.IsNullOrEmpty(this.AccountKey) ? this.AccountKey : null;
            result.AccessToken = !string.IsNullOrEmpty(this.AccessToken) ? this.AccessToken : null;
            result.AuthenticationToken = !string.IsNullOrEmpty(this.AuthenticationToken) ? this.AuthenticationToken : null;

            return result;
        }
    }
}
