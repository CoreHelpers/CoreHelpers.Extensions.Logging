using System;
using System.Net.Http;
using System.Threading.Tasks;
using CoreHelpers.Extensions.Logging.Abstractions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CoreHelpers.Extensions.Logging.AzureFunctions.Sandbox
{
    public class LoggingGarbageCollector
    {
        private ILogGarbageCollector _logGarbageCollector;
        private IConfiguration _configuration;

        public LoggingGarbageCollector(ILogGarbageCollector logGarbageCollector, IConfiguration configuration)
        {
            _logGarbageCollector = logGarbageCollector;
            _configuration = configuration;
        }

        private async Task CleanLog(string connectionStringName, string containerName)
        {
            // get connection string from configuration
            var connectionString = _configuration.GetConnectionString(connectionStringName);
            if (String.IsNullOrEmpty(connectionString))
                connectionString = _configuration.GetValue<string>(connectionStringName);
            
            await _logGarbageCollector.CleanLogRepository(connectionString, containerName, 3);
        }

        [FunctionName("LoggingGarbageCollectorTask")]
        public async Task Run([TimerTrigger("0 0 0 1 * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation("Running cleanup job");
            await CleanLog("AzureWebJobsStorage", "function-logs");
        }

        [FunctionName("TriggerLoggingGarbageCollectorTask")]
        public async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,            
            ILogger log)
        {
            log.LogInformation("Running cleanup job");
            await CleanLog("AzureWebJobsStorage", "function-logs");
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        }
    }
}
