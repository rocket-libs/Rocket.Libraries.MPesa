using System.Net.Http;
using System.Threading.Tasks;
using Rocket.Libraries.FormValidationHelper;
using Rocket.Libraries.FormValidationHelper.Attributes;
using Rocket.Libraries.MPesa.ApiCalling;

namespace Rocket.Libraries.MPesa.CustomerToBusinessRegistration
{
    public interface ICustomerToBusinessUrlRegistrar
    {
        Task<ValidationResponse<CustomerToBusinessRegisterUrlResponse>> RegisterUrlAsync(CustomerToBusinessRegisterUrlRequest customerToBusinessRegisterUrlRequest);
    }

    public class CustomerToBusinessUrlRegistrar : ICustomerToBusinessUrlRegistrar
    {
        private readonly IValidationResponseHelper validationResponseHelper;
        private readonly ITokenizedApiCaller tokenizedApiCaller;

        public CustomerToBusinessUrlRegistrar(
            IValidationResponseHelper validationResponseHelper,
            ITokenizedApiCaller tokenizedApiCaller
        )
        {
            this.validationResponseHelper = validationResponseHelper;
            this.tokenizedApiCaller = tokenizedApiCaller;
        }

        public async Task<ValidationResponse<CustomerToBusinessRegisterUrlResponse>> RegisterUrlAsync(
            CustomerToBusinessRegisterUrlRequest customerToBusinessRegisterUrlRequest
        )
        {
            var validationResponse = await new BasicFormValidator<CustomerToBusinessRegisterUrlRequest>()
                .ValidateAndPackAsync(customerToBusinessRegisterUrlRequest);
            return await validationResponseHelper.RouteResponseTypedAsync<CustomerToBusinessRegisterUrlRequest, CustomerToBusinessRegisterUrlResponse>(
                validationResponse,
                async (_) => await RegisterValidatedUrlAsync(customerToBusinessRegisterUrlRequest)
            );

        }

        private async Task<ValidationResponse<CustomerToBusinessRegisterUrlResponse>> RegisterValidatedUrlAsync(
            CustomerToBusinessRegisterUrlRequest customerToBusinessRegisterUrlRequest
        )
        {
            var result = await tokenizedApiCaller.CallEndpoint<CustomerToBusinessRegisterUrlResponse>(
                "mpesa/c2b/v1/registerurl",
                HttpMethod.Post,
                customerToBusinessRegisterUrlRequest
            );
            return validationResponseHelper.SuccessValue(result);
        }
    }
}