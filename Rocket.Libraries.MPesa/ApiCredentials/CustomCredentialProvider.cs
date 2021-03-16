using System;
using System.Threading.Tasks;

namespace Rocket.Libraries.MPesa.ApiCredentials
{
    public interface ICustomCredentialProvider
    {
        Task<Credential> GetAsync ();
    }

    /// <summary>
    /// For multi-tenant applications customize this class to
    /// return each tenants specific credentials.
    /// 
    /// Please note. The implementation used below is for the developer's machine and is completely useless for you.
    /// You MUST create your own implementation.
    /// </summary>
    public class CustomCredentialProvider : ICustomCredentialProvider
    {
        public async Task<Credential> GetAsync ()
        {
            return await Task.Run (() =>
            {
                return new Credential
                {
                    ConsumerKey = Environment.GetEnvironmentVariable ("MPesa_ConsumerKey"),
                    ConsumerSecret = Environment.GetEnvironmentVariable ("MPesa_ConsumerSecret"),
                    PassKey = Environment.GetEnvironmentVariable ("MPesa_PassKey")
                };
            });
        }
    }
}