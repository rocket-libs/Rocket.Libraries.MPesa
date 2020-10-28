using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Rocket.Libraries.MPesa.HttpClients;
using Rocket.Libraries.MPesa.Logging;

namespace Rocket.Libraries.MPesa.AccessToken
{
    public interface ITokenFetcher
    {
        Task<TokenResponse> FetchAsync ();
    }

    public class TokenFetcher : ITokenFetcher
    {
        private readonly ICustomHttpClientProvider customHttpClientProvider;
        private readonly IHttpCaller httpCaller;
        private readonly ILoggedExceptionFetcher loggedExceptionFetcher;
        private readonly MPesaSettings mPesaKeys;

        public TokenFetcher (
            IOptions<MPesaSettings> mPesaKeysOptions,
            IHttpCaller httpCaller,
            ILoggedExceptionFetcher loggedExceptionFetcher)
        {
            
            this.httpCaller = httpCaller;
            this.loggedExceptionFetcher = loggedExceptionFetcher;
            mPesaKeys = mPesaKeysOptions.Value;
        }

        public async Task<TokenResponse> FetchAsync ()
        {

            return await httpCaller.CallEndpoint<TokenResponse> (
                HttpClientTypes.TokenFetcher,
                relativePath: "oauth/v1/generate?grant_type=client_credentials",
                HttpMethod.Get,
                payload: default,
                (request) =>
                {
                    var keyBytes = Convert.ToBase64String (Encoding.UTF8.GetBytes ($"{mPesaKeys.ConsumerKey}:{mPesaKeys.ConsumerSecret}"));
                    request.Headers.Authorization = new AuthenticationHeaderValue ("Basic", keyBytes);
                }
            );
        }
    }
}