using System;

namespace Rocket.Libraries.MPesa.Logging
{
    public interface ILogWriter
    {
        void LogInformation (string information);

        void LogError (string error);

        void LogException (Exception exception, string additionalInformation);

    }
}