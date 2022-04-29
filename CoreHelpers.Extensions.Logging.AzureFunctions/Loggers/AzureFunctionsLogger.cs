using System;
using System.Collections.Generic;
using CoreHelpers.Extensions.Logging.Abstractions;
using CoreHelpers.Extensions.Logging.AzureFunctions.EventListener;
using CoreHelpers.Extensions.Logging.Constants;
using CoreHelpers.Extensions.Logging.Extensions;
using Microsoft.Extensions.Logging;

namespace CoreHelpers.Extensions.Logging.AzureFunctions.Loggers
{
    internal class AzureFunctionsLogger : IScopedLogger
    {
        private const string OperationContext = "CH_OperationContext";
        private ILogAppenderFactory _logAppenderFactory;        
        private string _category;

        public AzureFunctionsLogger(ILogAppenderFactory logAppenderFactory, string category)            
        {            
            _category = category;
            _logAppenderFactory = logAppenderFactory;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            if (state == null)         
                throw new ArgumentNullException(nameof(state));            

            StartTelemetryIfFunctionInvocation(state as IDictionary<string, object>);

            return LoggerScopeStack.Push(state, this);
        }

        public void DisposeScope<TState>(TState state)
        {            
            StopTelemetryIfOperationContextIsActive(state as IDictionary<string, object>);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {            
            // define the appender
            var logAppender = default(ILogAppender);
            var stateValues = LoggerScopeStack.GetMergedStateDictionaryOrNull();

            // identify the right appender
            if (stateValues != null && stateValues.ContainsKey(OperationContext))            
                logAppender = stateValues[OperationContext] as ILogAppender;            
            else            
                logAppender = _logAppenderFactory.CreateLogAppender(null);            

            // write the log
            logAppender.Append<TState>(logLevel, eventId, state, exception, formatter);
        }
        
        private void StartTelemetryIfFunctionInvocation(IDictionary<string, object> stateValues)
        {
            // in case we have no state value bail out
            if (stateValues == null)
                return;

            // check we have a function start event
            string functionName = stateValues.GetValueOrDefault<string>(AzureFunctionsLogScopeKeys.FunctionName);
            string functionInvocationId = stateValues.GetValueOrDefault<string>(AzureFunctionsLogScopeKeys.FunctionInvocationId);
            string eventName = stateValues.GetValueOrDefault<string>(AzureFunctionsLogScopeKeys.Event);

            // If we have the invocation id, function name, and event, we know it's a new function. That means
            // that we want to start a new operation and let App Insights track it for us.
            if (!string.IsNullOrEmpty(functionName) &&
                !string.IsNullOrEmpty(functionInvocationId) &&
                eventName == AzureFunctionsLogConstants.FunctionStartEvent &&
                !stateValues.ContainsKey(OperationContext))
            {
                // build the logfilename
                // functions/YYYY/MM/DD/HH/EPOCH.FUNCTIONNAME.INVOCATIONID.log
                var dateString = DateTime.Now.ToString("yyyy/MM/dd/HH");                
                var logFileName = $"activities/{dateString}/{functionName}.{functionInvocationId}";

                // check if we need to adapt the logfilename for a durable orchestration task
                if (!String.IsNullOrEmpty(DurableTaskEventListerner.Instance.LastKnownInstanceId))
                    logFileName = $"orchestrations/{dateString}/{functionName}.{DurableTaskEventListerner.Instance.LastKnownInstanceId}";

                // generate the logappender
                var logAppender = _logAppenderFactory.CreateLogAppender(logFileName);

                // log the first line and flush
                logAppender.Append<String>(LogLevel.Information, new EventId(0), $"Logstream for Azure Function {functionName} ({functionInvocationId}) established", null, (state, exc) => state);
                logAppender.Flush();

                // store in state                
                stateValues[OperationContext] = logAppender;                
            }
        }

        private void StopTelemetryIfOperationContextIsActive(IDictionary<string, object> stateValues)
        {
            if (stateValues == null)
                return;

            if (!stateValues.ContainsKey(OperationContext))
                return;

            var logAppender = stateValues[OperationContext] as ILogAppender;
            if (logAppender == null)
                return;

            logAppender.Dispose();

            DurableTaskEventListerner.Instance.LastKnownInstanceId = String.Empty;
        }
    }
}
