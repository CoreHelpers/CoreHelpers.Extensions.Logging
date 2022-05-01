using System;
using CoreHelpers.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;

namespace CoreHelpers.Extensions.Logging.AzureFunctions.Appenders
{
    internal class AzureFunctionsLogAppenderFactory : ILogAppenderFactory
    {
        private ILogAppenderFactory _baseLogAppenderFactory;
        private ILogAppender _defaultLogAppender;

        public AzureFunctionsLogAppenderFactory(ILogAppenderFactory baseLogAppenderFactory)
        {
            _baseLogAppenderFactory = baseLogAppenderFactory;    
        }
        
        public ILogAppender CreateLogAppender(string logSinkIdentifier = null)
        {
            if (String.IsNullOrEmpty(logSinkIdentifier))
                return null;

            if (logSinkIdentifier.StartsWith("hosts"))
                return GetHostsAppender(logSinkIdentifier);
            else
                return CreateFunctionInstanceLogAppender(logSinkIdentifier);             
        }

        private ILogAppender GetHostsAppender(string logSinkIdentifier)
        {
            if (_defaultLogAppender == null)
            {
                _defaultLogAppender = _baseLogAppenderFactory.CreateLogAppender(logSinkIdentifier);

                // log the first line and flush
                _defaultLogAppender.Append<String>(LogLevel.Information, new EventId(0), $"Logstream for Azure Function Host established", null, (state, exc) => state);
                _defaultLogAppender.Flush();
            }

            return _defaultLogAppender;
        }

        private ILogAppender CreateFunctionInstanceLogAppender(string logSinkIndetified)
        {
            return _baseLogAppenderFactory.CreateLogAppender(logSinkIndetified);            
        }
    }
}
