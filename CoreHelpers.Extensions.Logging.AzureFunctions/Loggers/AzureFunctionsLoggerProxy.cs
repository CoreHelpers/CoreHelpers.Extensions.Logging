using System;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Extensions.Logging;

namespace CoreHelpers.Extensions.Logging.AzureFunctions.Loggers
{
    internal static class AzureFunctionsLoggerProxyState
    {
        public static bool ConsoleLoggingEnabled = false;
    }

    internal class AzureFunctionsLoggerProxy<T> : ILogger<T>
    {
        readonly ILogger logger;

        public AzureFunctionsLoggerProxy(ILoggerFactory factory)
        {
            // See https://github.com/Azure/azure-functions-host/issues/4689#issuecomment-533195224
            logger = factory.CreateLogger(LogCategories.CreateFunctionUserCategory(typeof(T).FullName));
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return logger.BeginScope(state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logger.IsEnabled(logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (AzureFunctionsLoggerProxyState.ConsoleLoggingEnabled)
            {
                var timeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                var message = formatter(state, exception);

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write($"[{timeStamp}] ");
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine(message);
                Console.ResetColor();                
            }

            logger.Log(logLevel, eventId, state, exception, formatter);
        }
    }
}
