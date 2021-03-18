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
        private readonly ICredentialResolver credentialResolver;
        private readonly MPesaSettings mPesaSettings;

        public STKPusher(
            IEnvironmentSpecificValues environmentSpecificValues,
            IOptions<MPesaSettings> mPesaSettingsOptions,
            ITokenizedApiCaller tokenizedApiCaller,
            ICredentialResolver credentialResolver)
        {
            this.environmentSpecificValues = environmentSpecificValues;
            this.tokenizedApiCaller = tokenizedApiCaller;
            this.credentialResolver = credentialResolver;
            this.mPesaSettings = mPesaSettingsOptions.Value;
        }

        public async Task PushToPhoneAsync(Transaction transaction)
        {
            var credentials = await credentialResolver.GetCredentialsAsync();
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
            
            
            var response = await tokenizedApiCaller.CallEndpoint<GenericResponse>(
                relativePath: "mpesa/stkpush/v1/processrequest",
                HttpMethod.Post,
                lipaNaMpesaOnline
            );

        }
    }
}