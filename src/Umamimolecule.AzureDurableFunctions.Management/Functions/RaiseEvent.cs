using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umamimolecule.AzureDurableFunctions.Management.Exceptions;
using Umamimolecule.AzureDurableFunctions.Management.Extensions;
using Umamimolecule.AzureDurableFunctions.Management.Utility;

namespace Umamimolecule.AzureDurableFunctions.Management.Functions
{
    /// <summary>
    /// Contains HTTP-triggered Azure Functions related to raising of events for
    /// individual orchestration instances.
    /// </summary>
    public class RaiseEvent
    {
        /// <summary>
        /// The Azure Function to raise events for individual orchestration instances.
        /// </summary>
        /// <param name="req">The HTTP request.</param>
        /// <param name="client">The durable orchestration client.</param>
        /// <param name="instanceId">The ID of the orchestration instance to raise the event for.</param>
        /// <returns>202 Accepted if the input is valid.</returns>
        [FunctionName("RaiseEvent")]
        public virtual async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = Routes.RaiseEvent)]HttpRequest req,
            [DurableClient] IDurableOrchestrationClient client,
            string instanceId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(instanceId))
                {
                    throw new RequiredParameterMissingException("instanceId");
                }

                var eventName = req.Query.GetQueryParameter("EventName", true, x => x);
                var taskHubName = req.Query.GetQueryParameter("TaskHubName", false, x => x);
                var connectionName = req.Query.GetQueryParameter("ConnectionName", false, x => x);

                var body = await req.ReadAsStringAsync();
                JObject eventData = null;
                if (!string.IsNullOrWhiteSpace(body))
                {
                    eventData = JsonConvert.DeserializeObject<JObject>(body);
                }

                if (string.IsNullOrWhiteSpace(taskHubName))
                {
                    await client.RaiseEventAsync(instanceId, eventName, eventData);
                }
                else
                {
                    await client.RaiseEventAsync(taskHubName, instanceId, eventName, eventData, connectionName);
                }

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
