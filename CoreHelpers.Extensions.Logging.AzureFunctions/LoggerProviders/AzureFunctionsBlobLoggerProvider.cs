using System;
using CoreHelpers.Extensions.Logging.AzureFunctions.Appenders;
using CoreHelpers.Extensions.Logging.AzureFunctions.Loggers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CoreHelpers.Extensions.Logging.AzureFunctions.LoggerProviders
{
    internal class AzureFunctionsBlobLoggerProvider : ILoggerProvider
    {
        private AzureFunctionsLogAppenderFactory _azureFunctionsLogAppenderFactory;

        public AzureFunctionsBlobLoggerProvider(IConfiguration configuration, string connectionStringName, string containerName, bool monthlyContainer, int messageBuffer)
        {
            _azureFunctionsLogAppenderFactory = new AzureFunctionsLogAppenderFactory(
                new AzureBlobAppenderFactory(configuration, connectionStringName, containerName, monthlyContainer, messageBuffer)
            );
        }

        public ILogger CreateLogger(string categoryName)
        {            
            return new AzureFunctionsLogger(_azureFunctionsLogAppenderFactory, categoryName);
        }

        public void Dispose()
        {
            
        }
    }
}
