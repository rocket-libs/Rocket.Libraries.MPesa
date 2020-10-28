using System;

namespace Rocket.Libraries.MPesa.Logging
{
    public interface ILoggedExceptionFetcher
    {
        Exception GetLoggedException(string exceptionMessage, string additionalInformation);
    }

    public class LoggedExceptionFetcher : ILoggedExceptionFetcher
    {
        private readonly ILogWriter logWriter;

        public LoggedExceptionFetcher(ILogWriter logWriter)
        {
            this.logWriter = logWriter;
        }

        public Exception GetLoggedException(string exceptionMessage, string additionalInformation)
        {
            var exception = new Exception(exceptionMessage);
            logWriter.LogException(exception, additionalInformation);
            return exception;
        }
    }
}