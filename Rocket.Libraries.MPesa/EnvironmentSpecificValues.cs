using Microsoft.Extensions.Options;
using Rocket.Libraries.MPesa.Logging;

namespace Rocket.Libraries.MPesa
{
    public interface IEnvironmentSpecificValues
    {
        string BaseUrl { get; }

    }

    public class EnvironmentSpecificValues : IEnvironmentSpecificValues
    {
        private readonly MPesaSettings mPesaSettings;
        private readonly ILoggedExceptionFetcher loggedExceptionFetcher;
        private readonly ILogWriter logWriter;

        public EnvironmentSpecificValues (
            IOptions<MPesaSettings> mPesaSettingsOptions,
            ILoggedExceptionFetcher loggedExceptionFetcher,
            ILogWriter logWriter)
        {
            mPesaSettings = mPesaSettingsOptions.Value;
            this.loggedExceptionFetcher = loggedExceptionFetcher;
            this.logWriter = logWriter;
        }

        public string BaseUrl
        {
            get
            {
                const string currentAction = "Attempting to retrieve base url";
                if (string.IsNullOrEmpty (mPesaSettings.Environment))
                {
                    throw loggedExceptionFetcher.GetLoggedException ("M-Pesa environment has not been set", currentAction);
                }
                else
                {
                    logWriter.LogInformation ($"M-Pesa environment is: '${mPesaSettings.Environment}'");
                    var environmentToLowerCase = mPesaSettings.Environment.ToLowerInvariant ();
                    switch (environmentToLowerCase)
                    {
                        case "sandbox":
                            return "https://sandbox.safaricom.co.ke/";
                        default:
                            throw loggedExceptionFetcher.GetLoggedException ($"Unsupported M-Pesa environment '{mPesaSettings.Environment}", currentAction);
                    }

                }
            }
        }

    }
}