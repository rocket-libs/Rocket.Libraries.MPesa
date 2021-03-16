using System.Net.Http;
using System.Threading.Tasks;
using Rocket.Libraries.MPesa.ApiCalling;
using Rocket.Libraries.MPesa.ApiCredentials;
using Rocket.Libraries.MPesa.HttpClients;

namespace Rocket.Libraries.MPesa.BusinessToCustomer
{
    public interface IBusinessToCustomerPaymentRequester
    {
        Task<BusinessToCustomerResponse> RequestPaymentAsync(BusinessToCustomerRequest businessToCustomerRequest);
    }

    public class BusinessToCustomerPaymentRequester : IBusinessToCustomerPaymentRequester
    {
        private readonly ITokenizedApiCaller tokenizedApiCaller;
        
        private readonly ICredentialEncryptor credentialEncryptor;
        

        public BusinessToCustomerPaymentRequester(
            ITokenizedApiCaller tokenizedApiCaller,
            ICredentialEncryptor credentialEncryptor
        )
        {
            this.tokenizedApiCaller = tokenizedApiCaller;
            this.credentialEncryptor = credentialEncryptor;
        }

        public async Task<BusinessToCustomerResponse> RequestPaymentAsync(BusinessToCustomerRequest businessToCustomerRequest)
        {
            if (businessToCustomerRequest != null)
            {
                businessToCustomerRequest.SecurityCredential = await credentialEncryptor.GetEncryptedCredentialsAsync();
            }
            var response = await tokenizedApiCaller
            .CallEndpoint<BusinessToCustomerResponse>(
                HttpClientTypes.GenericClient,
                "mpesa/b2c/v1/paymentrequest",
                HttpMethod.Post,
                businessToCustomerRequest
            );
            return response;

        }

        
    }
}