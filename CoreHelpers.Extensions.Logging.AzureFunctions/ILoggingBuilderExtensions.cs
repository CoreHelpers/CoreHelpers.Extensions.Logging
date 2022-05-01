using CoreHelpers.Extensions.Logging.AzureFunctions.LoggerProviders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CoreHelpers.Extensions.Logging.AzureFunctions
{
    public static class ILoggingBuilderExtensions
    {
        public static void AddAzureFunctionsBlobLogging(this ILoggingBuilder builder, IConfiguration configuration, string connectionStringName, string containerName, bool monthlyContainer = true, int messageBuffer = 25)
        {
            builder.AddProvider(new AzureFunctionsBlobLoggerProvider(configuration, connectionStringName, containerName, monthlyContainer, messageBuffer));
        }

        public static void AddAzureFunctionsDurableTaskBlobLogging(this ILoggingBuilder builder, IConfiguration configuration, string connectionStringName, string containerName, bool monthlyContainer = true, int messageBuffer = 25)
        {           
            builder.AddProvider(new AzureFunctionsDurableTaskBlobLoggerProvider(configuration, connectionStringName, containerName, monthlyContainer, messageBuffer));
        }
    }
}
