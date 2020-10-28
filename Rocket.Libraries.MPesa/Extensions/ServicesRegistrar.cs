using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Rocket.Libraries.MPesa.AccessToken;
using Rocket.Libraries.MPesa.HttpClients;
using Rocket.Libraries.MPesa.Logging;
using Rocket.Libraries.MPesa.STKPush;

namespace Rocket.Libraries.MPesa.Extensions
{
    public static class ServicesRegistrar
    {
        public static void AddMPesaSupport (this IServiceCollection services)
        {
            services
                .AddMemoryCache()
                .RegisterHttpClients ()
                .AddTransient<IEnvironmentSpecificValues, EnvironmentSpecificValues> ()
                .AddTransient<ICustomHttpClientProvider, CustomHttpClientProvider> ()
                .AddTransient<ILogWriter, MPesaLogWriter> ()
                .AddTransient<ILoggedExceptionFetcher, LoggedExceptionFetcher> ()
                .AddTransient<ITokenFetcher, TokenFetcher> ()
                .AddTransient<IHttpCaller, HttpCaller> ()
                .AddTransient<ISTKPusher, STKPusher> ();
        }

        private static IServiceCollection RegisterHttpClients (this IServiceCollection services)
        {
            const byte defaultRetriesCount = 6;
            services
                .AddHttpClient (HttpClientTypes.TokenFetcher.ToString ())
                .AddPolicyHandler (GetRetryPolicy (totalRetries: defaultRetriesCount));
            services
                .AddHttpClient (HttpClientTypes.STKPusher.ToString ())
                .AddPolicyHandler (GetRetryPolicy (totalRetries: defaultRetriesCount));

            return services;
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy (byte totalRetries)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError ()
                .OrResult (msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .OrResult (msg => msg.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                .WaitAndRetryAsync (totalRetries, retryAttempt => TimeSpan.FromSeconds (Math.Pow (2, retryAttempt)));
        }
    }
}