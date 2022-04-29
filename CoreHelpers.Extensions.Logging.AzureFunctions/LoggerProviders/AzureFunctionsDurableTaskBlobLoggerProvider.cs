using CoreHelpers.Extensions.Logging.AzureFunctions.EventListener;
using Microsoft.Extensions.Configuration;

namespace CoreHelpers.Extensions.Logging.AzureFunctions.LoggerProviders
{
    internal class AzureFunctionsDurableTaskBlobLoggerProvider : AzureFunctionsBlobLoggerProvider
    {
        public AzureFunctionsDurableTaskBlobLoggerProvider(IConfiguration configuration, string connectionStringName, string containerName, int messageBuffer)
            : base(configuration, connectionStringName, containerName, messageBuffer)
        {
            DurableTaskEventListerner.Instance.Initialize();
        }
    }
}
