using System;
using System.Collections.Generic;

namespace CoreHelpers.Extensions.Logging.AzureFunctions.Appenders
{
    public static class AzureBlobAppenderContainerNameBuilder
    {
        public static string BuildMonthlyContainerName(string containerName)
        {
            return BuildMonthlyContainerName(containerName, DateTime.Now);
        }

        public static string BuildMonthlyContainerName(string containerName, DateTime referenceData)
        {
            return $"{containerName}-{referenceData.ToString("yyyy-MM")}";
        }

        public static List<string> BuildMonthlyContainerNames(string containerName, int retentionPeriodInMonths)
        {
            var containerNames = new List<string>();

            for(int i = 0; i < retentionPeriodInMonths; i++)            
                containerNames.Add(BuildMonthlyContainerName(containerName, DateTime.Now.AddMonths(-1 * i)));

            return containerNames;
        }
    }
}
