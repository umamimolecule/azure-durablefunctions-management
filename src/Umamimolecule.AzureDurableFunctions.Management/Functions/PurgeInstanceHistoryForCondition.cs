using System;
using System.Threading.Tasks;
using DurableTask.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Umamimolecule.AzureDurableFunctions.Management.Exceptions;
using Umamimolecule.AzureDurableFunctions.Management.Extensions;
using Umamimolecule.AzureDurableFunctions.Management.Utility;

namespace Umamimolecule.AzureDurableFunctions.Management.Functions
{
    /// <summary>
    /// Contains HTTP-triggered Azure Functions related to purging of history for
    /// multiple orchestration instances.
    /// </summary>
    public class PurgeInstanceHistoryForCondition
    {
        private static readonly OrchestrationStatus[] ValidStatuses = new OrchestrationStatus[]
        {
            OrchestrationStatus.Completed,
            OrchestrationStatus.Terminated,
            OrchestrationStatus.Failed,
        };

        /// <summary>
        /// The Azure Function to purge the history for multiple orchestration instances.
        /// </summary>
        /// <param name="req">The HTTP request.</param>
        /// <param name="client">The durable orchestration client.</param>
        /// <returns>A payload containing the number of instances purged.</returns>
        [FunctionName("PurgeInstanceHistoryForCondition")]
        public virtual async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = Routes.PurgeInstanceHistoryForCondition)]HttpRequest req,
            [DurableClient] IDurableOrchestrationClient client)
        {
            try
            {
                var createdTimeFrom = req.Query.GetQueryParameter("CreatedTimeFrom", true, Converters.DateTimeConverter("CreatedTimeFrom"));
                var createdTimeTo = req.Query.GetQueryParameter("CreatedTimeTo", false, Converters.DateTimeConverter("CreatedTimeTo"), DateTime.MaxValue.AddSeconds(-1));
                var runtimeStatus = req.Query.GetQueryParameter("RuntimeStatus", false, Converters.EnumCollectionConverter<OrchestrationStatus>("RuntimeStatus", ValidStatuses), null);

                var purgeHistoryResult = await client.PurgeInstanceHistoryAsync(createdTimeFrom, createdTimeTo, runtimeStatus);
                return new OkObjectResult(new Models.PurgeHistoryResult(purgeHistoryResult));
            }
            catch (StatusCodeException e)
            {
                return e.ToObjectResult();
            }
            catch (Exception e)
            {
                return e.ToUnhandledErrorResult();
            }
        }
    }
}
