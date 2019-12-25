namespace Umamimolecule.AzureDurableFunctions.Management.Functions
{
    public static class Routes
    {
        public const string GetManagementWebhookUrls = "orchestration/instances/{instanceId}/managementWebhookUrls";
        public const string GetStatusForAllInstances = "orchestration/instances";
        public const string GetStatusForInstance = "orchestration/instances/{instanceId}";
        public const string PurgeInstanceHistory = "orchestration/instances/{instanceId}/purgeInstanceHistory";
        public const string PurgeInstanceHistoryForCondition = "orchestration/instances/purgeInstanceHistory";
        public const string RaiseEvent = "orchestration/instances/{instanceId}/raiseEvent";
        public const string RewindInstance = "orchestration/instances/{instanceId}/rewind";
        public const string StartInstance = "orchestration/instances/startNew";
        public const string TerminateInstance = "orchestration/instances/{instanceId}/terminate";
    }
}
