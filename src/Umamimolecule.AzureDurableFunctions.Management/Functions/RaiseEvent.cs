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

namespace Umamimolecule.AzureDurableFunctions.Management.Functions
{
    public class RaiseEvent
    {
        [FunctionName("RaiseEvent")]
        public virtual async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "instances/{instanceId}/raiseEvent")]HttpRequest req,
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