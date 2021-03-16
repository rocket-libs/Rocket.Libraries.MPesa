using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Rocket.Libraries.MPesa.AccessToken;
using Rocket.Libraries.MPesa.ApiCalling;
using Rocket.Libraries.MPesa.ApiCredentials;
using Rocket.Libraries.MPesa.HttpClients;

namespace Rocket.Libraries.MPesa.STKPush
{
    public interface ISTKPusher
    {
        Task PushToPhoneAsync(Transaction transaction);
    }

    public class STKPusher : ISTKPusher
    {
        private readonly IEnvironmentSpecificValues environmentSpecificValues;
        private readonly ITokenizedApiCaller tokenizedApiCaller;
        private readonly MPesaSettings mPesaSettings;

        public STKPusher(
            IEnvironmentSpecificValues environmentSpecificValues,
            IOptions<MPesaSettings> mPesaSettingsOptions,
            ITokenizedApiCaller tokenizedApiCaller)
        {
            this.environmentSpecificValues = environmentSpecificValues;
            this.tokenizedApiCaller = tokenizedApiCaller;
            this.mPesaSettings = mPesaSettingsOptions.Value;
        }

        public async Task PushToPhoneAsync(Transaction transaction)
        {
            var lipaNaMpesaOnline = new LipaNaMpesaOnline
            {
                BusinessShortCode = transaction.BusinessShortCode.ToString(),
                Amount = transaction.Amount.ToString(),
                PhoneNumber = transaction.RequesterPhoneNumber.ToString(),
                CallBackURL = mPesaSettings.CallBackUrl,
                AccountReference = string.IsNullOrEmpty(transaction.AccountReference) ? "NA" : transaction.AccountReference,
                TransactionDesc = string.IsNullOrEmpty(transaction.Description) ? "NA" : transaction.Description,
                Passkey = "bfb279f9aa9bdbcf158e97dd71a467cd2e0c893059b10f78e6b72ada1ed2c919", // string.Empty, //credentials.PassKey,
                TransactionType = transaction.TransactionType
            };
            
            
            var response = await tokenizedApiCaller.CallEndpoint<GenericResponse>(
                HttpClientTypes.STKPusher,
                relativePath: "mpesa/stkpush/v1/processrequest",
                HttpMethod.Post,
                lipaNaMpesaOnline
            );

        }
    }
}