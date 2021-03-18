using System;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Libraries.MPesa.ApiCredentials
{
    public interface ICredentialEncryptor
    {
        Task<string> GetEncryptedCredentialsAsync();
    }

    public class CredentialEncryptor : ICredentialEncryptor
    {
        private readonly ICredentialResolver credentialResolver;


        public CredentialEncryptor(
            ICredentialResolver credentialResolver)
        {
            this.credentialResolver = credentialResolver;
        }

        public async Task<string> GetEncryptedCredentialsAsync()
        {
            var credentials = await credentialResolver.GetCredentialsAsync();
            var plainText = $"{credentials.ConsumerKey}:{credentials.ConsumerSecret}";
            var bytes = Encoding.UTF8.GetBytes(plainText);
            var base64String =  Convert.ToBase64String(bytes);
            return base64String;
        }
    }
}