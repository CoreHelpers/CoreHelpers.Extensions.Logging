using System;
using CoreHelpers.Extensions.Logging.Abstractions;

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
                return GetHostsAppender();
            else
                return CreateFunctionInstanceLogAppender(logSinkIdentifier);             
        }

        private ILogAppender GetHostsAppender()
        {
            if (_defaultLogAppender == null)
            {
                _defaultLogAppender = _baseLogAppenderFactory.CreateLogAppender($"hosts/{Guid.NewGuid().ToString()}");
            }

            return _defaultLogAppender;
        }

        private ILogAppender CreateFunctionInstanceLogAppender(string logSinkIndetified)
        {
            return _baseLogAppenderFactory.CreateLogAppender(logSinkIndetified);            
        }
    }
}
