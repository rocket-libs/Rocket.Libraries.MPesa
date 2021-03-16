using System;
using System.Threading.Tasks;
using Rocket.Libraries.MPesa.ApiCalling;
using Rocket.Libraries.MPesa.HttpClients;

namespace Rocket.Libraries.MPesa.BusinessToCustomer
{
    public class BusinessToCustomerPusher
    {
        private readonly IHttpCaller httpCaller;
        private readonly ITokenizedApiCaller tokenizedApiCaller;

        public BusinessToCustomerPusher(
            ITokenizedApiCaller tokenizedApiCaller
        )
        {
            this.tokenizedApiCaller = tokenizedApiCaller;
        }

        public async Task SendAsync()
        {
            throw new NotImplementedException();
        }
    }
}