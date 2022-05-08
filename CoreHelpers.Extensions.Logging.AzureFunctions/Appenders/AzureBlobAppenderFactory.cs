using System;
using CoreHelpers.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Configuration;

namespace CoreHelpers.Extensions.Logging.AzureFunctions.Appenders
{
    internal class AzureBlobAppenderFactory : ILogAppenderFactory
    {
        private string _connectionString;
        private string _containerName;
        private int _messageBuffer;

        public AzureBlobAppenderFactory(IConfiguration configuration, string connectionStringName, string containerName, bool monthlyContainer, int messageBuffer)
        {
            // get connection string from configuration
            var connectionString = configuration.GetConnectionString(connectionStringName);
            if (String.IsNullOrEmpty(connectionString))
                _connectionString = configuration.GetValue<string>(connectionStringName);

            // build the containername
            _containerName = containerName;
            if (monthlyContainer)
                _containerName = AzureBlobAppenderContainerNameBuilder.BuildMonthlyContainerName(_containerName);

            // assign the buffer size
            _messageBuffer = messageBuffer;
        }

        public AzureBlobAppenderFactory(string connectionString, string containerName)
        {
            _connectionString = connectionString;
            _containerName = containerName;            
        }

        public ILogAppender CreateLogAppender(string logSinkIdentifier = null)
        {
            // create the appender
            var appender = new AzureBlobAppender(_connectionString, _containerName, $"{logSinkIdentifier}.log", _messageBuffer);

            // open the connection 
            appender.Open();

            // done
            return appender;
        }        
    }
}
