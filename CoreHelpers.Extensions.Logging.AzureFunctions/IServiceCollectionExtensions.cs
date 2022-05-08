using System;
using CoreHelpers.Extensions.Logging.Abstractions;
using CoreHelpers.Extensions.Logging.AzureFunctions.GarbageCollector;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoreHelpers.Extensions.Logging.AzureFunctions
{
    public static class IServiceCollectionExtensions
    {
        public static void AddLoggingGarbageCollectors(this IServiceCollection services)
        {        
            services.AddTransient<ILogGarbageCollector, AzureBlobLogGarbageCollector>();
        }
    }
}
