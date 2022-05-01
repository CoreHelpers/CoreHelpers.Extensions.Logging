using CoreHelpers.Extensions.Logging.AzureFunctions.EventListener;
using Microsoft.Extensions.Configuration;

namespace CoreHelpers.Extensions.Logging.AzureFunctions.LoggerProviders
{
    internal class AzureFunctionsDurableTaskBlobLoggerProvider : AzureFunctionsBlobLoggerProvider
    {
        public AzureFunctionsDurableTaskBlobLoggerProvider(IConfiguration configuration, string connectionStringName, string containerName, bool monthlyContainer, int messageBuffer)
            : base(configuration, connectionStringName, containerName, monthlyContainer, messageBuffer)
        {
            DurableTaskEventListerner.Instance.Initialize();
        }
    }
}
