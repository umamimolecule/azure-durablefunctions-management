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
    public class PurgeInstanceHistoryForCondition
    {
        [FunctionName("PurgeInstanceHistoryForCondition")]
        public virtual async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "orchestration/instances/purgeInstanceHistory")]HttpRequest req,
            [DurableClient] IDurableOrchestrationClient client)
        {
            try
            {
                var createdTimeFrom = req.Query.GetQueryParameter("CreatedTimeFrom", true, Converters.DateTimeConverter("CreatedTimeFrom"));
                var createdTimeTo = req.Query.GetQueryParameter("CreatedTimeTo", false, Converters.DateTimeConverter("CreatedTimeTo"), DateTime.MaxValue.AddSeconds(-1));
                var runtimeStatus = req.Query.GetQueryParameter("RuntimeStatus", false, Converters.EnumCollectionConverter<OrchestrationStatus>("RuntimeStatus"), null);

                await client.PurgeInstanceHistoryAsync(createdTimeFrom, createdTimeTo, runtimeStatus);
                return new AcceptedResult();
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
