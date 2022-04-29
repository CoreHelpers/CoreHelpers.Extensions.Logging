namespace CoreHelpers.Extensions.Logging.AzureFunctions.Constants
{
    internal static class AzureFunctionsDurableTaskConstants
    {
        public const string EventSourcePattern = "DurableTask";

        public const string FunctionStartEvent = "FunctionStarting";

        public const string FunctionCompletedEvent = "FunctionCompleted";

        public const string TaskOrchestrationDispatcherProcessEvent = "TaskOrchestrationDispatcher-ProcessEvent";

        public const string InfoEvent = "Info";

        public const string InfoEventSubEventType = "eventType";

        public const string InfoEventSubInstanceId = "instanceId";

        public const string EventPayloadNameFunctionType = "FunctionType";

        public const string EventPayloadNameInstanceId = "InstanceId";
    }
}
