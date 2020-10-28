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
            var tillNumberPayments = serviceProvider.GetRequiredService<ITillNumberPayments> ();
            tillNumberPayments.PushToPhoneAsync(
                requesterPhoneNumber: 0, // REPLACE THE ZERO WITH TARGET PHONE NUMBER HERE
                tillNumber: 174379,
                amount: 1).GetAwaiter().GetResult();


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