using System.Net.Http;
using System.Threading.Tasks;
using Rocket.Libraries.MPesa.AccessToken;
using Rocket.Libraries.MPesa.ApiCredentials;
using Rocket.Libraries.MPesa.HttpClients;

namespace Rocket.Libraries.MPesa.ApiCalling
{
    public interface ITokenizedApiCaller
    {
        Task<TResult> CallEndpoint<TResult>(HttpClientTypes httpClientType, string relativePath, HttpMethod method, object payload);
    }

    public class TokenizedApiCaller : ITokenizedApiCaller
    {
        private readonly IHttpCaller httpCaller;
        private readonly ITokenFetcher tokenFetcher;

        public TokenizedApiCaller(
            IHttpCaller httpCaller,
            ITokenFetcher tokenFetcher
        )
        {
            this.httpCaller = httpCaller;
            this.tokenFetcher = tokenFetcher;
        }

        public async Task<TResult> CallEndpoint<TResult>(
            HttpClientTypes httpClientType,
            string relativePath,
            HttpMethod method,
            object payload)
        {
            var accessToken = await tokenFetcher.FetchAsync();
            return await httpCaller.CallEndpoint<TResult>(
                httpClientType,
                relativePath,
                method,
                payload,
                onBeforeRequest: (httpRequestMessage) =>
                {
                    httpRequestMessage.Headers.Add("Authorization", $"Bearer {accessToken}");
                }
            );
        }
    }
}