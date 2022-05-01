using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace CoreHelpers.Extensions.Logging.AzureFunctions.Sandbox
{
    public static class RecurrentTask
    {
        [FunctionName("RecurrentTask")]
        public static async Task Run([TimerTrigger("0/30 * * * * *")] TimerInfo myTimer, [DurableClient] IDurableOrchestrationClient starter, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            
            string instanceId = await starter.StartNewAsync("FanOutOrchestrator", null);
            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");            
        }
    }
}
