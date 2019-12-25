using System.Collections.Generic;
using System.Linq;

namespace Umamimolecule.AzureDurableFunctions.Management.Models
{
    /// <summary>
    /// The status of all orchestration instances with paging for a given query.
    /// </summary>
    public class OrchestrationStatusQueryResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrchestrationStatusQueryResult"/> class.
        /// </summary>
        public OrchestrationStatusQueryResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrchestrationStatusQueryResult"/> class.
        /// </summary>
        /// <param name="result">The object to copy the data from.</param>
        public OrchestrationStatusQueryResult(Microsoft.Azure.WebJobs.Extensions.DurableTask.OrchestrationStatusQueryResult result)
        {
            this.ContinuationToken = result.ContinuationToken;
            this.DurableOrchestrationState = result.DurableOrchestrationState?.Select(x => new Models.DurableOrchestrationStatus(x));
        }

        /// <summary>
        /// Gets or sets a collection of statuses of orchestration instances matching the query description.
        /// </summary>
        public IEnumerable<Models.DurableOrchestrationStatus> DurableOrchestrationState { get; set; }

        /// <summary>
        /// Gets or sets a token that can be used to resume the query with data not already returned by this query.
        /// </summary>
        public string ContinuationToken { get; set; }
    }
}
