using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Umamimolecule.AzureDurableFunctionsStatus.Exceptions;
using Umamimolecule.AzureDurableFunctionsStatus.Extensions;
using Umamimolecule.AzureDurableFunctionsStatus.Utility;

namespace Umamimolecule.AzureDurableFunctionsStatus.Functions
{
    public class GetStatusForInstance
    {
        [FunctionName("GetStatusForInstance")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "instances/{instanceId}")]HttpRequest req,
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
                    dynamic value = new
                    {
                        error = new
                        {
                            code = "NOTFOUND",
                            message = string.Format(Resources.ExceptionMessages.InstanceNotFound, instanceId)
                        }
                    };

                    return new NotFoundObjectResult(value);
                }
                else
                {
                    return new OkObjectResult(status);
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
    }
}
