using Microsoft.Extensions.DependencyInjection;
using Rocket.Libraries.MPesa.STKPush;
using Rocket.Libraries.MPesa.Extensions;
using Microsoft.Extensions.Configuration;
using System.IO;
using Rocket.Libraries.MPesa.ApiCredentials;

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

            var stkPusher = serviceProvider.GetRequiredService<ISTKPusher> ();

            var transaction = new Transaction(
                requesterPhoneNumber: 254721553229, // Set the phone number to push the STK validation to.  
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