using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Rocket.Libraries.MPesa.Logging;

namespace Rocket.Libraries.MPesa.HttpClients
{
    public interface IHttpCaller
    {
        Task<TResult> CallEndpoint<TResult> (HttpClientTypes httpClientType, string relativePath, HttpMethod method, object payload, Action<HttpRequestMessage> onBeforeRequest);
    }

    public class HttpCaller : IHttpCaller
    {
        private readonly ICustomHttpClientProvider customHttpClientProvider;
        private readonly ILoggedExceptionFetcher loggedExceptionFetcher;
        private readonly IEnvironmentSpecificValues environmentSpecificValues;
        private readonly ILogWriter logWriter;

        public HttpCaller (
            ICustomHttpClientProvider customHttpClientProvider,
            ILoggedExceptionFetcher loggedExceptionFetcher,
            IEnvironmentSpecificValues environmentSpecificValues,
            ILogWriter logWriter
        )
        {
            this.customHttpClientProvider = customHttpClientProvider;
            this.loggedExceptionFetcher = loggedExceptionFetcher;
            this.environmentSpecificValues = environmentSpecificValues;
            this.logWriter = logWriter;
        }

        public async Task<TResult> CallEndpoint<TResult> (
            HttpClientTypes httpClientType,
            string relativePath,
            HttpMethod method,
            object payload,
            Action<HttpRequestMessage> onBeforeRequest)
        {
            var endpointUri = new Uri ($"{environmentSpecificValues.BaseUrl}{relativePath}");

            // Don't wrap httpClient in a 'using' block. 
            // See: https://josef.codes/you-are-probably-still-using-httpclient-wrong-and-it-is-destabilizing-your-software/
            var httpClient = customHttpClientProvider.GetHttpClient (HttpClientTypes.TokenFetcher);

            try
            {
                using (var request = new HttpRequestMessage (method, endpointUri))
                {
                    SetPayloadIfAvailable (request, payload);
                    FireCallbackIfAvailable (request, onBeforeRequest);
                    using (var response = await httpClient.SendAsync (request))
                    {

                        var responseData = await response.Content.ReadAsStringAsync ();
                        ReportFailureIfAny (response, relativePath, responseData);
                        return JsonConvert.DeserializeObject<TResult> (responseData);
                    }
                }
            }
            catch(Exception e)
            {
                logWriter.LogException(e, "Error making call to M-Pesa");
                throw;
            }
            finally
            {
                onBeforeRequest = null;
            }
        }

        private void SetPayloadIfAvailable (HttpRequestMessage request, object payload)
        {
            if (payload != default)
            {
                request.Content = new StringContent (
                    JsonConvert.SerializeObject (payload),
                    Encoding.UTF8,
                    "application/json"
                );
            }
        }

        private void FireCallbackIfAvailable (HttpRequestMessage request, Action<HttpRequestMessage> onBeforeRequest)
        {
            if (onBeforeRequest != default)
            {
                onBeforeRequest (request);
            }
        }

        private void ReportFailureIfAny (HttpResponseMessage response, string relativePath, string responseData)
        {
            if (response.IsSuccessStatusCode == false)
            {
                throw loggedExceptionFetcher.GetLoggedException (
                    $"M-Pesa call failed. Endpoint: '{relativePath}'" +
                    $"\nHTTP Status Code: {response.StatusCode}" +
                    $"\nResponse Content: {responseData}",
                    "Call to M-Pesa failed"
                );
            }
        }
    }
}