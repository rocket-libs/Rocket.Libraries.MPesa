using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Rocket.Libraries.MPesa.ApiCredentials
{
    public interface ICredentialResolver
    {
        Task<Credential> GetCredentialsAsync ();
    }

    public class CredentialResolver : ICredentialResolver
    {
        private readonly ICustomCredentialProvider customCredentialProvider;
        public Credential singleTenantCredentials;
        public CredentialResolver (
            IOptions<Credential> singleTenantCredentialOptions,
            ICustomCredentialProvider customCredentialProvider
        )
        {
            singleTenantCredentials = singleTenantCredentialOptions.Value;
            this.customCredentialProvider = customCredentialProvider;
        }

        public async Task<Credential> GetCredentialsAsync ()
        {
            var customCredential = await customCredentialProvider.GetAsync ();
            var singleTentantCredentialsAreValid = CredentialIsValid (singleTenantCredentials);
            var customCredentialsAreValid = CredentialIsValid (customCredential);
            if (singleTentantCredentialsAreValid && customCredentialsAreValid)
            {
                throw new Exception ($"Both global credentials and custom credentials have been specified. Only a single type of credential can be used app-wide");
            }
            else if (customCredentialsAreValid)
            {
                return customCredential;
            }
            else if (singleTentantCredentialsAreValid)
            {
                return singleTenantCredentials;
            }
            else
            {
                throw new Exception ("No credentials to connect to the M-Pesa API have been provided");
            }
        }

        private bool CredentialIsValid (Credential credential)
        {
            if (credential == default)
            {
                return false;
            }
            else
            {
                return !string.IsNullOrEmpty (credential.ConsumerKey) &&
                    !string.IsNullOrEmpty (credential.ConsumerSecret);
            }
        }
    }
}