using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(CoreHelpers.Extensions.Logging.AzureFunctions.Sandbox.Startup))]

namespace CoreHelpers.Extensions.Logging.AzureFunctions.Sandbox
{   
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = builder.GetContext().Configuration;

            // add the logger
            builder.Services.AddLogging((logBuilder) =>
            {
                // add the functions logging 
                logBuilder.AddAzureFunctionsDurableTaskBlobLogging(configuration, "AzureWebJobsStorage", "function-logs");
            });            
        }
    }
}
