using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Umamimolecule.AzureDurableFunctions.Management.Exceptions;
using Umamimolecule.AzureDurableFunctions.Management.Extensions;

namespace Umamimolecule.AzureDurableFunctions.Management.Functions
{
    public class PurgeInstanceHistory
    {
        [FunctionName("PurgeInstanceHistory")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "instances/{instanceId}/purgeInstanceHistory")]HttpRequest req,
            [DurableClient] IDurableOrchestrationClient client,
            string instanceId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(instanceId))
                {
                    throw new RequiredParameterMissingException("instanceId");
                }

                await client.PurgeInstanceHistoryAsync(instanceId);
                return new AcceptedResult();
            }
            catch (ArgumentException ae)
            {
                if (string.Compare(ae.ParamName, "instanceId", true) == 0)
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

                throw;
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
