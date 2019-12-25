using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
    public class GetStatusForAllInstances
    {
        [FunctionName("GetStatusForAllInstances")]
        public virtual async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = Routes.GetStatusForAllInstances)]HttpRequest req,
            [DurableClient] IDurableOrchestrationClient client)
        {
            try
            {
                if (req.Query.Any())
                {
                    OrchestrationStatusQueryCondition condition;
                    condition = this.BuildQueryCondition(req);

                    var result = await client.GetStatusAsync(condition, CancellationToken.None);
                    return new OkObjectResult(new Models.OrchestrationStatusQueryResult(result));
                }
                else
                {
                    var statuses = await client.GetStatusAsync();
                    return new OkObjectResult(
                        new Models.OrchestrationStatusQueryResult()
                        {
                            DurableOrchestrationState = statuses.Select(x => new Models.DurableOrchestrationStatus(x))
                        }
                    );
                }
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

        private OrchestrationStatusQueryCondition BuildQueryCondition(HttpRequest req)
        {
            OrchestrationStatusQueryCondition condition = new OrchestrationStatusQueryCondition()
            {
                RuntimeStatus = req.Query.GetQueryParameter("RuntimeStatus", false, Converters.EnumCollectionConverter<OrchestrationRuntimeStatus>("RuntimeStatus")),
                CreatedTimeFrom = req.Query.GetQueryParameter("CreatedTimeFrom", false, Converters.DateTimeConverter("CreatedTimeFrom"), DateTime.MinValue),
                CreatedTimeTo = req.Query.GetQueryParameter("CreatedTimeTo", false, Converters.DateTimeConverter("CreatedTimeTo"), DateTime.MaxValue.AddSeconds(-1)),
                InstanceIdPrefix = req.Query.GetQueryParameter("InstanceIdPrefix", false, x => x),
                PageSize = req.Query.GetQueryParameter("PageSize", false, Converters.IntConverter("PageSize"), 100),
                ShowInput = req.Query.GetQueryParameter("ShowInput", false, Converters.BoolConverter("ShowInput"), false),
                TaskHubNames = req.Query.GetQueryParameter("TaskHubNames", false, Converters.StringCollectionConverter("TaskHubNames"), null),
                ContinuationToken = req.Query.GetQueryParameter("ContinuationToken", false, x => x),
            };

            return condition;
        }
    }
}
