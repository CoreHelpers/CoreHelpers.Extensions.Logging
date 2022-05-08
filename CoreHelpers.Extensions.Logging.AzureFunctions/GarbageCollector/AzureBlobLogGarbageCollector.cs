using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CoreHelpers.Extensions.Logging.Abstractions;
using CoreHelpers.Extensions.Logging.AzureFunctions.Appenders;
using Microsoft.Extensions.Logging;

namespace CoreHelpers.Extensions.Logging.AzureFunctions.GarbageCollector
{
    public class AzureBlobLogGarbageCollector : ILogGarbageCollector
    {
        private ILogger _logger;

        public AzureBlobLogGarbageCollector(ILogger<AzureBlobLogGarbageCollector> logger)
        {
            _logger = logger;
        }

        public async Task CleanLogRepository(string connectionString, string containerName, int retentionWindowsInMonths)
        {
            // build the conenction string
            _logger.LogInformation("Connecting to storage");
            var options = new BlobClientOptions(BlobClientOptions.ServiceVersion.V2021_02_12);
            var blobServiceClient = new BlobServiceClient(connectionString, options);

            // generate the valid container names
            var validContainerNames = AzureBlobAppenderContainerNameBuilder.BuildMonthlyContainerNames(containerName, retentionWindowsInMonths);

            // get the containers
            _logger.LogInformation("Receiving containers");
            var resultSegment = blobServiceClient.GetBlobContainersAsync(BlobContainerTraits.None).AsPages(default);

            var scheduledForDeletion = new List<BlobContainerItem>();

            // enumerate all containers
            _logger.LogInformation("Evaluating containers in the storage");
            await foreach (Azure.Page<BlobContainerItem> containerPage in resultSegment)
            {
                foreach (BlobContainerItem containerItem in containerPage.Values)
                {
                    _logger.LogInformation($"Evaluating container: {containerItem.Name}");

                    if (!containerItem.Name.StartsWith(containerName))
                    {
                        _logger.LogInformation("It's not a valid log container");
                        continue;
                    }

                    if (containerItem.Name.Equals(containerName))
                    {
                        _logger.LogInformation("It's the container we use for non monthly mode, don't touch ");
                        continue;
                    }

                    if (validContainerNames.Contains(containerItem.Name))
                    {
                        _logger.LogInformation("It's a valid log container, ignoring");
                        continue;
                    }

                    _logger.LogInformation("Scheudle container for deletion");
                    scheduledForDeletion.Add(containerItem);
                }
            }

            // delete the scheduled container
            _logger.LogInformation($"Removing #{scheduledForDeletion.Count} containers");
            foreach (var containerToDelete in scheduledForDeletion)
            {
                _logger.LogInformation($"Deleting container {containerToDelete.Name}");
                await blobServiceClient.DeleteBlobContainerAsync(containerToDelete.Name);
            }            
        }
    }
}
