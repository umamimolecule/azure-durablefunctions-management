using Newtonsoft.Json;

namespace Umamimolecule.AzureDurableFunctions.Management.Models
{
    /// <summary>
    /// Class to hold statistics about this execution of purge history.
    /// </summary>
    public class PurgeHistoryResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PurgeHistoryResult"/> class.
        /// </summary>
        public PurgeHistoryResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PurgeHistoryResult"/> class.
        /// </summary>
        /// <param name="result">The object to copy the data from.</param>
        public PurgeHistoryResult(Microsoft.Azure.WebJobs.Extensions.DurableTask.PurgeHistoryResult result)
        {
            this.InstancesDeleted = result.InstancesDeleted;
        }

        /// <summary>
        /// Gets or sets the number of deleted instances.
        /// </summary>
        [JsonProperty("instancesDeleted")]
        public int InstancesDeleted { get; set; }
    }
}
