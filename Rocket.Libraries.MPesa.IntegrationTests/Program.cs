using Microsoft.Extensions.DependencyInjection;
using Rocket.Libraries.MPesa.STKPush;
using Rocket.Libraries.MPesa.Extensions;
using Microsoft.Extensions.Configuration;
using System.IO;
using Rocket.Libraries.MPesa.ApiCredentials;
using Rocket.Libraries.MPesa.BusinessToCustomer;

namespace Rocket.Libraries.MPesa.IntegrationTests
{
    class Program
    {
        static void Main (string[] args)
        {
            var configuration = GetConfiguration();
            var serviceProviderBuilder = new ServiceCollection ();
            serviceProviderBuilder.AddMPesaSupport();
            serviceProviderBuilder.Configure<MPesaSettings>(x => configuration.GetSection(nameof(MPesaSettings)).Bind(x));
            serviceProviderBuilder.Configure<Credential>(x => configuration.GetSection("SingleMPesaTenantCredentials").Bind(x));
            var serviceProvider = serviceProviderBuilder.BuildServiceProvider();
            B2CPaymentRequest(serviceProvider);


        }

        static void B2CPaymentRequest(ServiceProvider serviceProvider)
        {
            var b2cPaymentRequester = serviceProvider.GetRequiredService<IBusinessToCustomerPaymentRequester> ();
            var businessToCustomerRequest = new BusinessToCustomerRequest
            {
                Amount = 1,
                CommandID = BusinessToCustomerCommandIds.BusinessPayment,
                InitiatorName = "Acme Payer",
                PartyA = 30671,
                PartyB = 254721553229,
            };
            b2cPaymentRequester.RequestPaymentAsync(businessToCustomerRequest);
        }

        static void StkPush(ServiceProvider serviceProvider)
        {
            var stkPusher = serviceProvider.GetRequiredService<ISTKPusher> ();

            var transaction = new Transaction(
                requesterPhoneNumber: 0, // Set the phone number to push the STK validation to.  
                businessShortCode: 174379,
                amount: 1,
                transactionType: TransactionTypes.CustomerBuyGoodsOnline);

            stkPusher.PushToPhoneAsync(
                transaction // REPLACE THE ZERO WITH TARGET PHONE NUMBER HERE
                    )
                .GetAwaiter().GetResult();

        }

        static IConfiguration GetConfiguration()
        {
            var basePath = $"{Directory.GetCurrentDirectory()}";
            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables();
                
            return builder.Build();
                
        }

    }
}