namespace Umamimolecule.AzureDurableFunctions.Management.Functions
{
    /// <summary>
    /// Contains the routes for the function endpoints.
    /// </summary>
    public static class Routes
    {
        /// <summary>
        /// The route for the <see cref="GetManagementWebhookUrls"/> function.
        /// </summary>
        public const string GetManagementWebhookUrls = "orchestration/instances/{instanceId}/managementWebhookUrls";

        /// <summary>
        /// The route for the <see cref="GetStatusForAllInstances"/> function.
        /// </summary>
        public const string GetStatusForAllInstances = "orchestration/instances";

        /// <summary>
        /// The route for the <see cref="GetStatusForInstance"/> function.
        /// </summary>
        public const string GetStatusForInstance = "orchestration/instances/{instanceId}";

        /// <summary>
        /// The route for the <see cref="GetManagementWebhookUrls"/> function.
        /// </summary>
        public const string PurgeInstanceHistory = "orchestration/instances/{instanceId}/purgeInstanceHistory";

        /// <summary>
        /// The route for the <see cref="PurgeInstanceHistoryForCondition"/> function.
        /// </summary>
        public const string PurgeInstanceHistoryForCondition = "orchestration/instances/purgeInstanceHistory";

        /// <summary>
        /// The route for the <see cref="RaiseEvent"/> function.
        /// </summary>
        public const string RaiseEvent = "orchestration/instances/{instanceId}/raiseEvent";

        /// <summary>
        /// The route for the <see cref="RewindInstance"/> function.
        /// </summary>
        public const string RewindInstance = "orchestration/instances/{instanceId}/rewind";

        /// <summary>
        /// The route for the <see cref="StartInstance"/> function.
        /// </summary>
        public const string StartInstance = "orchestration/instances/startNew";

        /// <summary>
        /// The route for the <see cref="TerminateInstance"/> function.
        /// </summary>
        public const string TerminateInstance = "orchestration/instances/{instanceId}/terminate";
    }
}
