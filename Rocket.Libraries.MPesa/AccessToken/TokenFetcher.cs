using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Rocket.Libraries.MPesa.ApiCredentials;
using Rocket.Libraries.MPesa.HttpClients;
using Rocket.Libraries.MPesa.Logging;

namespace Rocket.Libraries.MPesa.AccessToken
{
    public interface ITokenFetcher
    {
        Task<string> FetchAsync ();
    }

    public class TokenFetcher : ITokenFetcher
    {
        private const string accessTokenCacheKey = "AccessToken";
        private readonly IHttpCaller httpCaller;
        private readonly ILoggedExceptionFetcher loggedExceptionFetcher;
        private readonly IMemoryCache memoryCache;
        private readonly ICredentialEncryptor credentialEncryptor;

        public TokenFetcher (
            IOptions<MPesaSettings> mPesaKeysOptions,
            IHttpCaller httpCaller,
            ILoggedExceptionFetcher loggedExceptionFetcher,
            IMemoryCache memoryCache,
            ICredentialEncryptor credentialEncryptor
            )
        {
            
            this.httpCaller = httpCaller;
            this.loggedExceptionFetcher = loggedExceptionFetcher;
            this.memoryCache = memoryCache;
            this.credentialEncryptor = credentialEncryptor;
        }

        public async Task<string> FetchAsync ()
        {
            var accessToken = string.Empty;
            if(memoryCache.TryGetValue<string>(accessTokenCacheKey,out accessToken))
            {
                return accessToken;
            }
            else
            {
                accessToken = await FetchTokenFromThirdPartyAsync();
                CacheToken(accessToken);
                return accessToken;
            }
        }

        private void CacheToken(string accessToken)
        {
            const byte staleMinutes = 30;
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(staleMinutes));
            memoryCache.Set(accessTokenCacheKey, accessToken,cacheEntryOptions);
        }

        private async Task<string> FetchTokenFromThirdPartyAsync()
        {
            var encryptedCredentials = await credentialEncryptor.GetEncryptedCredentialsAsync();
            var tokenResponse = await httpCaller.CallEndpoint<TokenResponse> (
                HttpClientTypes.TokenFetcher,
                relativePath: "oauth/v1/generate?grant_type=client_credentials",
                HttpMethod.Get,
                payload: default,
                (request) =>
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue ("Basic", encryptedCredentials);
                }
            );
            return tokenResponse.AccessToken;
        }
    }
}