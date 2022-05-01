using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace CoreHelpers.Extensions.Logging.AzureFunctions.Sandbox
{
    public static class FanOutActivity
    {
        [FunctionName("FanOutActivity")]
        public static async Task Activity([ActivityTrigger] ILogger log)
        {
            log.LogInformation("Executing the FanOutActivity");

            for (int i = 0; i < 32; i++)
                log.LogInformation($"Log-Item #{i}");

            await Task.CompletedTask;
        }
    }
}