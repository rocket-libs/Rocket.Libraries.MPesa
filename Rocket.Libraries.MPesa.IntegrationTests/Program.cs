using Microsoft.Extensions.DependencyInjection;
using Rocket.Libraries.MPesa.STKPush;
using Rocket.Libraries.MPesa.Extensions;
using Microsoft.Extensions.Configuration;
using System.IO;
using Rocket.Libraries.MPesa.ApiCredentials;
using Rocket.Libraries.MPesa.BusinessToCustomer;
using System;
using System.Linq;
using Rocket.Libraries.MPesa.CustomerToBusinessRegistration;

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
            RegisterC2BPayment(serviceProvider);


        }

        static void RegisterC2BPayment(ServiceProvider serviceProvider)
        {
            var c2bUrlRegistrar = serviceProvider.GetRequiredService<ICustomerToBusinessUrlRegistrar> ();
            var customerToBusinessRegisterUrlRequest = new CustomerToBusinessRegisterUrlRequest
            {
                ConfirmationURL = "https://example.com",
                ResponseType = CustomerToBusinessResponseTypes.Canceled,
                ShortCode = 123456,
                ValidationURL = "https://example.com",
            };
            var response = c2bUrlRegistrar.RegisterUrlAsync(customerToBusinessRegisterUrlRequest)
                .GetAwaiter().GetResult();
            if(response.HasErrors)
            {
                throw new Exception(response.ValidationErrors.First().Errors.First());
            }
        }

        static void B2CPaymentRequest(ServiceProvider serviceProvider)
        {
            var b2cPaymentRequester = serviceProvider.GetRequiredService<IBusinessToCustomerPaymentRequester> ();
            var businessToCustomerRequest = new BusinessToCustomerRequest
            {
                Amount = 1,
                CommandID = BusinessToCustomerCommandIds.BusinessPayment,
                InitiatorName = "Acme Payer",
                PartyA = 123454,
                PartyB = 254721553229,
                Remarks = "This is a test",
                QueueTimeOutURL = "https://example.com",
                ResultURL = "https://example.com"
            };
            var response = b2cPaymentRequester.RequestPaymentAsync(businessToCustomerRequest)
                .GetAwaiter().GetResult();
            if(response.HasErrors)
            {
                throw new Exception(response.ValidationErrors.First().Errors.First());
            }
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