using System.Net.Http;
using System.Threading.Tasks;
using Rocket.Libraries.MPesa.AccessToken;
using Rocket.Libraries.MPesa.HttpClients;

namespace Rocket.Libraries.MPesa.STKPush
{
    public class TillNumberPayments
    {
        private readonly ITokenFetcher tokenFetcher;
        private readonly ICustomHttpClientProvider customHttpClientProvider;
        private readonly IEnvironmentSpecificValues environmentSpecificValues;
        private readonly IHttpCaller httpCaller;

        public TillNumberPayments (
            ITokenFetcher tokenFetcher,
            ICustomHttpClientProvider customHttpClientProvider,
            IEnvironmentSpecificValues environmentSpecificValues,
            IHttpCaller httpCaller)
        {
            this.tokenFetcher = tokenFetcher;
            this.customHttpClientProvider = customHttpClientProvider;
            this.environmentSpecificValues = environmentSpecificValues;
            this.httpCaller = httpCaller;
        }

        public async Task PushToPhoneAsync (string requesterPhoneNumber, long tillNumber, long amount)
        {
            var lipaNaMpesaOnline = new LipaNaMpesaOnline
            {
                BusinessShortCode = tillNumber.ToString (), // "174379",
                Amount = amount.ToString (),
                PhoneNumber = requesterPhoneNumber,
                CallBackURL = $"{environmentSpecificValues.BaseUrl}/mpesa/",
                AccountReference = "Blah",
                TransactionDesc = string.Empty,
                Passkey = "bfb279f9aa9bdbcf158e97dd71a467cd2e0c893059b10f78e6b72ada1ed2c919"
            };
            var tokenResponse = await tokenFetcher.FetchAsync ();

            await httpCaller.CallEndpoint<object> (
                HttpClientTypes.STKPusher,
                relativePath: "mpesa/stkpush/v1/processrequest",
                HttpMethod.Post,
                lipaNaMpesaOnline,
                (request) =>
                {
                    request.Headers.Add ("Authorization", $"Bearer {tokenResponse.AccessToken}");
                }
            );

        }
    }
}