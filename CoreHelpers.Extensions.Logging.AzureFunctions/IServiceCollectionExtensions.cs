using System;
using System.Linq;
using CoreHelpers.Extensions.Logging.Abstractions;
using CoreHelpers.Extensions.Logging.AzureFunctions.GarbageCollector;
using CoreHelpers.Extensions.Logging.AzureFunctions.Loggers;
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

        public static void DisableExternalLoggerFunctionsFilter(this IServiceCollection services, bool enableConsole = false)
        {
            // set the globale state
            AzureFunctionsLoggerProxyState.ConsoleLoggingEnabled = enableConsole;

            // Replace ILogger<T> with the one that works fine in all scenarios 
            var logger = services.FirstOrDefault(s => s.ServiceType == typeof(ILogger<>));
            if (logger != null)
                services.Remove(logger);

            services.Add(new ServiceDescriptor(typeof(ILogger<>), typeof(AzureFunctionsLoggerProxy<>), ServiceLifetime.Transient));
        }        
    }
}
