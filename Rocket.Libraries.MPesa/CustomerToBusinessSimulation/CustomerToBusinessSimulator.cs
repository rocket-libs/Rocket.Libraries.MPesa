using System.Net.Http;
using System.Threading.Tasks;
using Rocket.Libraries.FormValidationHelper;
using Rocket.Libraries.FormValidationHelper.Attributes;
using Rocket.Libraries.MPesa.ApiCalling;

namespace Rocket.Libraries.MPesa.CustomerToBusinessSimulation
{
    public interface ICustomerToBusinessSimulator
    {
        Task<ValidationResponse<CustomerToBusinessSimulationResponse>> SimulateAsync(CustomerToBusinessSimulationRequest customerToBusinessSimulationRequest);
    }

    public class CustomerToBusinessSimulator : ICustomerToBusinessSimulator
    {
        private readonly IValidationResponseHelper validationResponseHelper;
        private readonly ITokenizedApiCaller tokenizedApiCaller;

        public CustomerToBusinessSimulator(
            IValidationResponseHelper validationResponseHelper,
            ITokenizedApiCaller tokenizedApiCaller
        )
        {
            this.validationResponseHelper = validationResponseHelper;
            this.tokenizedApiCaller = tokenizedApiCaller;
        }

        public async Task<ValidationResponse<CustomerToBusinessSimulationResponse>> SimulateAsync(
            CustomerToBusinessSimulationRequest customerToBusinessSimulationRequest
        )
        {
            using (var validator = new BasicFormValidator<CustomerToBusinessSimulationRequest>())
            {
                var validationResponse = await validator.ValidateAndPackAsync(customerToBusinessSimulationRequest);
                return await validationResponseHelper.RouteResponseTypedAsync<CustomerToBusinessSimulationRequest, CustomerToBusinessSimulationResponse>(
                    validationResponse,
                    async (_) => await SimulateValidatedAsync(customerToBusinessSimulationRequest)
                );
            }

        }

        private async Task<ValidationResponse<CustomerToBusinessSimulationResponse>> SimulateValidatedAsync(
            CustomerToBusinessSimulationRequest customerToBusinessSimulationRequest)
        {
            var response = await tokenizedApiCaller.CallEndpoint<CustomerToBusinessSimulationResponse>(
                "mpesa/c2b/v1/simulate",
                HttpMethod.Post,
                customerToBusinessSimulationRequest
            );
            return validationResponseHelper.SuccessValue(response);
        }
    }
}

   