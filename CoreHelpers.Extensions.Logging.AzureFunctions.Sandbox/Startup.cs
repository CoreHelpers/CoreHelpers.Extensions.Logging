using System;
using System.Linq;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

            // add support for garbage collectors 
            builder.Services.AddLoggingGarbageCollectors();

            // ensure we route all to the right logfile
            builder.Services.DisableExternalLoggerFunctionsFilter(true);
        }               
    }
}
