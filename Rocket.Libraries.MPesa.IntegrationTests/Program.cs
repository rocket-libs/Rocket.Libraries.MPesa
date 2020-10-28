using System;
using Microsoft.Extensions.DependencyInjection;
using Rocket.Libraries.MPesa.STKPush;
using Rocket.Libraries.MPesa.Extensions;
using Microsoft.Extensions.DependencyInjection.Abstractions;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Rocket.Libraries.MPesa.AccessToken;

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
            var serviceProvider = serviceProviderBuilder.BuildServiceProvider();

            var stkPusher = serviceProvider.GetRequiredService<ISTKPusher> ();

            var transaction = new Transaction(
                requesterPhoneNumber: 0, // Set the phone number to push the STK validation to.  
                businessShortCode: 174379,
                amount: 1,
                transactionType: TransactionTypes.CustomerBuyGoodsOnline);

            var credentials = new Credentials(
                consumerKey: "your-consumer-key-goes-here",
                consumerSecret: "your-consumer-secret-goes-here",
                passKey: "your-pass-key-goes-here");

            stkPusher.PushToPhoneAsync(
                transaction, // REPLACE THE ZERO WITH TARGET PHONE NUMBER HERE
                credentials)
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