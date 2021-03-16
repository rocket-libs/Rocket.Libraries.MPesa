using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Rocket.Libraries.FormValidationHelper;
using Rocket.Libraries.MPesa.AccessToken;
using Rocket.Libraries.MPesa.ApiCalling;
using Rocket.Libraries.MPesa.ApiCredentials;
using Rocket.Libraries.MPesa.BusinessToCustomer;
using Rocket.Libraries.MPesa.CustomerToBusinessRegistration;
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
                .RegisterHttpClients()
                .AddScoped<IEnvironmentSpecificValues, EnvironmentSpecificValues>()
                .AddScoped<ICustomHttpClientProvider, CustomHttpClientProvider>()
                .AddScoped<ILogWriter, MPesaLogWriter>()
                .AddScoped<ILoggedExceptionFetcher, LoggedExceptionFetcher>()
                .AddScoped<ITokenFetcher, TokenFetcher>()
                .AddScoped<IHttpCaller, HttpCaller>()
                .AddScoped<ISTKPusher, STKPusher>()
                .AddScoped<ICredentialResolver, CredentialResolver>()
                .AddScoped<ICustomCredentialProvider, CustomCredentialProvider>()
                .AddScoped<ITokenizedApiCaller, TokenizedApiCaller>()
                .AddScoped<ICredentialEncryptor, CredentialEncryptor>()
                .AddScoped<IBusinessToCustomerPaymentRequester, BusinessToCustomerPaymentRequester>()
                .AddScoped<IValidationResponseHelper, ValidationResponseHelper>()
                .AddScoped<ICustomerToBusinessUrlRegistrar, CustomerToBusinessUrlRegistrar>();
        }

        private static IServiceCollection RegisterHttpClients (this IServiceCollection services)
        {
            const byte defaultRetriesCount = 6;
            services
                .AddHttpClient (HttpClientTypes.TokenFetcher.ToString ())
                .AddPolicyHandler (GetRetryPolicy (totalRetries: defaultRetriesCount));

            services
                .AddHttpClient (HttpClientTypes.GenericClient.ToString ())
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