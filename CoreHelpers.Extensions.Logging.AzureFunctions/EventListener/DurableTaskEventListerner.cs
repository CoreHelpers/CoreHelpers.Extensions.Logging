using System;
using System.Diagnostics.Tracing;
using CoreHelpers.Extensions.Logging.AzureFunctions.Constants;

namespace CoreHelpers.Extensions.Logging.AzureFunctions.EventListener
{
    internal sealed class DurableTaskEventListerner : System.Diagnostics.Tracing.EventListener
    {
        private static readonly DurableTaskEventListerner instance = new DurableTaskEventListerner();

        static DurableTaskEventListerner()
        {}

        private DurableTaskEventListerner()
        {}

        public static DurableTaskEventListerner Instance
        {
            get
            {
                return instance;
            }
        }

        public string LastKnownInstanceId = String.Empty;

        public void Initialize()
        {}
                
        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            if (eventSource.Name.Contains(AzureFunctionsDurableTaskConstants.EventSourcePattern))
                EnableEvents(eventSource, EventLevel.LogAlways);            
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            if (eventData.EventName.Equals(AzureFunctionsDurableTaskConstants.InfoEvent))
            {
                // eventType
                var eventType = GetPayloadValueByNameOrDefault(eventData, AzureFunctionsDurableTaskConstants.InfoEventSubEventType);
                if (!eventType.Equals(AzureFunctionsDurableTaskConstants.TaskOrchestrationDispatcherProcessEvent))
                    return;

                // collect the instanceId
                var instanceId = GetPayloadValueByNameOrDefault(eventData, AzureFunctionsDurableTaskConstants.InfoEventSubInstanceId);
                if (String.IsNullOrEmpty(instanceId))
                    return;

                // Announcing Durable Orchestrator Task for the logger
                this.LastKnownInstanceId = instanceId;
            }                       
        }

        private string GetPayloadValueByNameOrDefault(EventWrittenEventArgs eventData, string payloadName)
        {
            if (eventData == null || eventData.PayloadNames == null || eventData.Payload == null)
                return String.Empty;

            var payloadIndex = eventData.PayloadNames.IndexOf(payloadName);
            if (payloadIndex == -1)
                return String.Empty;

            return Convert.ToString(eventData.Payload[payloadIndex]);
        }
    }
}

