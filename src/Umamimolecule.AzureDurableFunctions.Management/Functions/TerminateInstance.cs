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
    /// <summary>
    /// Contains HTTP-triggered Azure Functions related to terminating individual
    /// orchestration instances.
    /// </summary>
    public class TerminateInstance
    {
        /// <summary>
        /// The Azure Function to rewind individual orchestration instances.
        /// </summary>
        /// <param name="req">The HTTP request.</param>
        /// <param name="client">The durable orchestration client.</param>
        /// <param name="instanceId">The ID of the orchestration instance being terminated.</param>
        /// <returns>202 Accepted if the input is valid.</returns>
        [FunctionName("TerminateInstance")]
        public virtual async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = Routes.TerminateInstance)]HttpRequest req,
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
                await client.TerminateAsync(instanceId, reason);
                return new AcceptedResult();
            }
            catch (InvalidOperationException ioe)
            {
                return ResponseHelper.CreateInvalidOperationResult(ioe.Message);
            }
            catch (ArgumentException ae)
            {
                if (string.Compare(ae.ParamName, "instanceId", true) == 0)
                {
                    var message = string.Format(Resources.ExceptionMessages.InstanceNotFound, instanceId);
                    return ResponseHelper.CreateNotFoundResult(message);
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
