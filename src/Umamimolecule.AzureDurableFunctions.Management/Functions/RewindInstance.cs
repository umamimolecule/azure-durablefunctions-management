using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Umamimolecule.AzureDurableFunctions.Management.Exceptions;
using Umamimolecule.AzureDurableFunctions.Management.Extensions;

namespace Umamimolecule.AzureDurableFunctions.Management.Functions
{
    public class RewindInstance
    {
        [FunctionName("RewindInstance")]
        public virtual async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "instances/{instanceId}/rewind")]HttpRequest req,
            [DurableClient] IDurableOrchestrationClient client,
            string instanceId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(instanceId))
                {
                    throw new RequiredParameterMissingException("instanceId");
                }

                var reason = req.Query.GetQueryParameter("Reason", true, x => x);
                await client.RewindAsync(instanceId, reason);
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

                return ae.ToUnhandledErrorResult();
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