using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Rocket.Libraries.MPesa.Logging
{

    [ExcludeFromCodeCoverage]
    public class MPesaLogWriter : ILogWriter
    {
        private readonly ILogger<MPesaLogWriter> logger;

        public MPesaLogWriter (ILogger<MPesaLogWriter> logger)
        {
            this.logger = logger;
        }

        public void LogError (string error)
        {
            logger.LogError (error);
        }

        public void LogException (Exception exception, string additionalInformation)
        {
            logger.LogError (new EventId (1), exception, additionalInformation);
        }

        public void LogInformation (string information)
        {
            logger.LogInformation (information);
        }

    }
}