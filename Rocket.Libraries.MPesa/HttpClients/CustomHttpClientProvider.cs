using System;
using System.Net.Http;
using Rocket.Libraries.MPesa.Logging;

namespace Rocket.Libraries.MPesa.HttpClients
{
    public interface ICustomHttpClientProvider
    {
        HttpClient GetHttpClient (HttpClientTypes httpClientType);
    }

    public class CustomHttpClientProvider : ICustomHttpClientProvider
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILoggedExceptionFetcher loggedExceptionFetcher;

        public CustomHttpClientProvider (
            IHttpClientFactory httpClientFactory,
            ILoggedExceptionFetcher loggedExceptionFetcher)
        {
            this.httpClientFactory = httpClientFactory;
            this.loggedExceptionFetcher = loggedExceptionFetcher;
        }

        public HttpClient GetHttpClient (HttpClientTypes httpClientType)
        {
            if (Enum.IsDefined (typeof (HttpClientTypes), httpClientType))
            {
                return httpClientFactory.CreateClient (httpClientType.ToString ());
            }
            else
            {
                throw loggedExceptionFetcher.GetLoggedException ($"Unsupported httpclient '{httpClientType}'", "Failed to return http client");
            }
        }
    }
}