using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Rocket.Libraries.MPesa.HttpClients;
using Rocket.Libraries.MPesa.Logging;

namespace Rocket.Libraries.MPesa.AccessToken
{
    public interface ITokenFetcher
    {
        Task<string> FetchAsync (Credentials credentials);
    }

    public class TokenFetcher : ITokenFetcher
    {
        private const string accessTokenCacheKey = "AccessToken";
        private readonly IHttpCaller httpCaller;
        private readonly ILoggedExceptionFetcher loggedExceptionFetcher;
        private readonly IMemoryCache memoryCache;
        public TokenFetcher (
            IOptions<MPesaSettings> mPesaKeysOptions,
            IHttpCaller httpCaller,
            ILoggedExceptionFetcher loggedExceptionFetcher,
            IMemoryCache memoryCache)
        {
            
            this.httpCaller = httpCaller;
            this.loggedExceptionFetcher = loggedExceptionFetcher;
            this.memoryCache = memoryCache;
        }

        public async Task<string> FetchAsync (Credentials credentials)
        {
            var accessToken = string.Empty;
            if(memoryCache.TryGetValue<string>(accessTokenCacheKey,out accessToken))
            {
                return accessToken;
            }
            else
            {
                accessToken = await FetchTokenFromThirdPartyAsync(credentials);
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

        private async Task<string> FetchTokenFromThirdPartyAsync(Credentials credentials)
        {
            var tokenResponse = await httpCaller.CallEndpoint<TokenResponse> (
                HttpClientTypes.TokenFetcher,
                relativePath: "oauth/v1/generate?grant_type=client_credentials",
                HttpMethod.Get,
                payload: default,
                (request) =>
                {
                    var keyBytes = Convert.ToBase64String (Encoding.UTF8.GetBytes ($"{credentials.ConsumerKey}:{credentials.ConsumerSecret}"));
                    request.Headers.Authorization = new AuthenticationHeaderValue ("Basic", keyBytes);
                }
            );
            return tokenResponse.AccessToken;
        }
    }
}