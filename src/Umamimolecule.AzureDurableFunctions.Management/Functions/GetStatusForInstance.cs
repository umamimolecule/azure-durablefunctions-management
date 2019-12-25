using System;
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
    public class GetStatusForInstance
    {
        [FunctionName("GetStatusForInstance")]
        public virtual async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "orchestration/instances/{instanceId}")]HttpRequest req,
            [DurableClient] IDurableOrchestrationClient client,
            string instanceId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(instanceId))
                {
                    throw new RequiredParameterMissingException("instanceId");
                }

                var showHistory = req.Query.GetQueryParameter("ShowHistory", false, Converters.BoolConverter("ShowHistory"), false);
                var showHistoryOutput = req.Query.GetQueryParameter("ShowHistoryOutput", false, Converters.BoolConverter("ShowHistoryOutput"), false);
                var showInput = req.Query.GetQueryParameter("ShowInput", false, Converters.BoolConverter("ShowInput"), false);

                var status = await client.GetStatusAsync(instanceId, showHistory, showHistoryOutput, showInput);
                if (status == null)
                {
                    var message = string.Format(Resources.ExceptionMessages.InstanceNotFound, instanceId);
                    return ResponseHelper.CreateNotFoundResult(message);
                }

                return new OkObjectResult(new Models.DurableOrchestrationStatus(status));
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
