using System.Net.Http;
using System.Threading.Tasks;
using Rocket.Libraries.MPesa.AccessToken;
using Rocket.Libraries.MPesa.HttpClients;

namespace Rocket.Libraries.MPesa.STKPush
{
    public interface ITillNumberPayments
    {
        Task PushToPhoneAsync(long requesterPhoneNumber, long tillNumber, long amount);
    }

    public class TillNumberPayments : ITillNumberPayments
    {
        private readonly ITokenFetcher tokenFetcher;
        private readonly ICustomHttpClientProvider customHttpClientProvider;
        private readonly IEnvironmentSpecificValues environmentSpecificValues;
        private readonly IHttpCaller httpCaller;

        public TillNumberPayments(
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

        public async Task PushToPhoneAsync(long requesterPhoneNumber, long tillNumber, long amount)
        {
            var lipaNaMpesaOnline = new LipaNaMpesaOnline
            {
                BusinessShortCode = tillNumber.ToString(),
                Amount = amount.ToString(),
                PhoneNumber = requesterPhoneNumber.ToString(),
                CallBackURL = $"{environmentSpecificValues.BaseUrl}mpesa/",
                AccountReference = "Jamaica",
                TransactionDesc = "This is a test",
                Passkey = "bfb279f9aa9bdbcf158e97dd71a467cd2e0c893059b10f78e6b72ada1ed2c919",
            };
            var tokenResponse = await tokenFetcher.FetchAsync();

            var response = await httpCaller.CallEndpoint<GenericResponse>(
                HttpClientTypes.STKPusher,
                relativePath: "mpesa/stkpush/v1/processrequest",
                HttpMethod.Post,
                lipaNaMpesaOnline,
                (request) =>
                {
                    request.Headers.Add("Authorization", $"Bearer {tokenResponse.AccessToken}");
                }
            );

        }
    }
}