using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Rocket.Libraries.MPesa.AccessToken;
using Rocket.Libraries.MPesa.HttpClients;

namespace Rocket.Libraries.MPesa.STKPush
{
    public interface ISTKPusher
    {
        Task PushToPhoneAsync(Transaction transaction, Credentials credentials);
    }

    public class STKPusher : ISTKPusher
    {
        private readonly ITokenFetcher tokenFetcher;
        private readonly ICustomHttpClientProvider customHttpClientProvider;
        private readonly IEnvironmentSpecificValues environmentSpecificValues;
        private readonly IHttpCaller httpCaller;

        private readonly MPesaSettings mPesaSettings;

        public STKPusher(
            ITokenFetcher tokenFetcher,
            ICustomHttpClientProvider customHttpClientProvider,
            IEnvironmentSpecificValues environmentSpecificValues,
            IHttpCaller httpCaller,
            IOptions<MPesaSettings> mPesaSettingsOptions)
        {
            this.tokenFetcher = tokenFetcher;
            this.customHttpClientProvider = customHttpClientProvider;
            this.environmentSpecificValues = environmentSpecificValues;
            this.httpCaller = httpCaller;
            this.mPesaSettings = mPesaSettingsOptions.Value;
        }

        public async Task PushToPhoneAsync(Transaction transaction, Credentials credentials)
        {
            var lipaNaMpesaOnline = new LipaNaMpesaOnline
            {
                BusinessShortCode = transaction.BusinessShortCode.ToString(),
                Amount = transaction.Amount.ToString(),
                PhoneNumber = transaction.RequesterPhoneNumber.ToString(),
                CallBackURL = mPesaSettings.CallBackUrl,
                AccountReference = string.IsNullOrEmpty(transaction.AccountReference) ? "NA" : transaction.AccountReference,
                TransactionDesc = string.IsNullOrEmpty(transaction.Description) ? "NA" : transaction.Description,
                Passkey = credentials.PassKey,
                TransactionType = transaction.TransactionType
            };
            
            var accessToken = await tokenFetcher.FetchAsync(credentials);

            var response = await httpCaller.CallEndpoint<GenericResponse>(
                HttpClientTypes.STKPusher,
                relativePath: "mpesa/stkpush/v1/processrequest",
                HttpMethod.Post,
                lipaNaMpesaOnline,
                (request) =>
                {
                    request.Headers.Add("Authorization", $"Bearer {accessToken}");
                }
            );

        }
    }
}