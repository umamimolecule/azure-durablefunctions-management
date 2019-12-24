using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Umamimolecule.AzureDurableFunctions.Management.Exceptions;
using Umamimolecule.AzureDurableFunctions.Management.Extensions;

namespace Umamimolecule.AzureDurableFunctions.Management.Functions
{
    public class GetManagementWebhookUrls
    {
        [FunctionName("GetManagementWebhookUrls")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "orchestration/instances/{instanceId}/managementWebhookUrls")]HttpRequest req,
            [DurableClient] IDurableOrchestrationClient client,
            string instanceId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(instanceId))
                {
                    throw new RequiredParameterMissingException("instanceId");
                }

                var payload = client.CreateHttpManagementPayload(instanceId);
                return new OkObjectResult(payload);
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
