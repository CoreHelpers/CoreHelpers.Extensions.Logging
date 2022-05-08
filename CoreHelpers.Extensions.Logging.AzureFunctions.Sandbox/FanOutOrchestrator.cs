using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace CoreHelpers.Extensions.Logging.AzureFunctions.Sandbox
{
    public class FanOutOrchestrator
    {
        [FunctionName("FanOutOrchestrator")]
        public static async Task<bool> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            int fanOutActivities = 10;

            log.LogInformation($"Starting the Fan Out Operation with {fanOutActivities} activities");

            var parallelTasks = new List<Task>();

            for (int i = 0; i < fanOutActivities; i++)
            {
                log.LogInformation($"Triggering Fan-Out-Operation #{i}");
                var task = context.CallActivityAsync<bool>("FanOutActivity", null);
                parallelTasks.Add(task);
            }

            await Task.WhenAll(parallelTasks);
            return true;
        }

        [FunctionName("TriggerFanOut")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            string instanceId = await starter.StartNewAsync("FanOutOrchestrator", null);
            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            // reflect the status
            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
