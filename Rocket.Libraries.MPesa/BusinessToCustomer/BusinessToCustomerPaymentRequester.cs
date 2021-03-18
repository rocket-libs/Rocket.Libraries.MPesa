using System.Net.Http;
using System.Threading.Tasks;
using Rocket.Libraries.FormValidationHelper;
using Rocket.Libraries.FormValidationHelper.Attributes;
using Rocket.Libraries.MPesa.ApiCalling;
using Rocket.Libraries.MPesa.ApiCredentials;
using Rocket.Libraries.MPesa.HttpClients;

namespace Rocket.Libraries.MPesa.BusinessToCustomer
{
    public interface IBusinessToCustomerPaymentRequester
    {
        Task<ValidationResponse<BusinessToCustomerResponse>> RequestPaymentAsync(BusinessToCustomerRequest businessToCustomerRequest);
    }

    public class BusinessToCustomerPaymentRequester : IBusinessToCustomerPaymentRequester
    {
        private readonly ITokenizedApiCaller tokenizedApiCaller;
        
        private readonly ICredentialEncryptor credentialEncryptor;
        private readonly IValidationResponseHelper validationResponseHelper;

        public BusinessToCustomerPaymentRequester(
            ITokenizedApiCaller tokenizedApiCaller,
            ICredentialEncryptor credentialEncryptor,
            IValidationResponseHelper validationResponseHelper
        )
        {
            this.tokenizedApiCaller = tokenizedApiCaller;
            this.credentialEncryptor = credentialEncryptor;
            this.validationResponseHelper = validationResponseHelper;
        }

        public async Task<ValidationResponse<BusinessToCustomerResponse>> RequestPaymentAsync(BusinessToCustomerRequest businessToCustomerRequest)
        {
            if (businessToCustomerRequest != null)
            {
                businessToCustomerRequest.SecurityCredential = await credentialEncryptor.GetEncryptedCredentialsAsync();
            }
            var validationReponse = await new BasicFormValidator<BusinessToCustomerRequest>()
                .ValidateAndPackAsync(businessToCustomerRequest);

            return await validationResponseHelper.RouteResponseTypedAsync<BusinessToCustomerRequest, BusinessToCustomerResponse>(
                validationReponse,
                async (_) => await RequestValidatedPaymentAsync(businessToCustomerRequest)
            );

        }

        private async Task<ValidationResponse<BusinessToCustomerResponse>> RequestValidatedPaymentAsync(BusinessToCustomerRequest businessToCustomerRequest)
        {
            var response = await tokenizedApiCaller
            .CallEndpoint<BusinessToCustomerResponse>(
                "mpesa/b2c/v1/paymentrequest",
                HttpMethod.Post,
                businessToCustomerRequest
            );
            return validationResponseHelper.SuccessValue(response);
        }

        
    }
}