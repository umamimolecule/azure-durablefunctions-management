using System;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Umamimolecule.AzureDurableFunctions.Management.Models
{
    /// <summary>
    /// Represents the status of a durable orchestration instance.
    /// </summary>
    public class DurableOrchestrationStatus
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DurableOrchestrationStatus"/> class.
        /// </summary>
        /// <param name="status">The object to copy the data from.</param>
        public DurableOrchestrationStatus(Microsoft.Azure.WebJobs.Extensions.DurableTask.DurableOrchestrationStatus status)
        {
            this.CreatedTime = status.CreatedTime;
            this.CustomStatus = status.CustomStatus;
            this.History = status.History;
            this.Input = status.Input;
            this.InstanceId = status.InstanceId;
            this.LastUpdatedTime = status.LastUpdatedTime;
            this.Name = status.Name;
            this.Output = status.Output;
            this.RuntimeStatus = status.RuntimeStatus;
        }

        /// <summary>
        /// Gets or sets the name of the queried orchestrator function.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the ID of the queried orchestration instance.
        /// </summary>
        [JsonProperty("instanceId")]
        public string InstanceId { get; set; }

        /// <summary>
        /// Gets or sets the time at which the orchestration instance was created.
        /// </summary>
        [JsonProperty("createdTime")]
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// Gets or sets the time at which the orchestration instance last updated its execution.
        /// </summary>
        [JsonProperty("lastUpdatedTime")]
        public DateTime LastUpdatedTime { get; set; }

        /// <summary>
        /// Gets the input of the orchestrator function instance.
        /// </summary>
        [JsonProperty("input")]
        public JToken Input { get; set; }

        /// <summary>
        /// Gets the output of the queried orchestration instance.
        /// </summary>
        [JsonProperty("output")]
        public JToken Output { get; set; }

        /// <summary>
        /// Gets the runtime status of the queried orchestration instance.
        /// </summary>
        [JsonProperty("runtimeStatus")]
        [JsonConverter(typeof(StringEnumConverter))]
        public OrchestrationRuntimeStatus RuntimeStatus { get; set; }

        /// <summary>
        /// Gets the custom status payload (if any) that was set by the orchestrator function.
        /// </summary>
        [JsonProperty("customStatus")]
        public JToken CustomStatus { get; set; }

        /// <summary>
        /// Gets the execution history of the orchestration instance.
        /// </summary>
        [JsonProperty("history")]
        public JArray History { get; set; }
    }
}
