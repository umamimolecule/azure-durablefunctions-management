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
    /// <summary>
    /// Contains HTTP-triggered Azure Functions related to management webhook URLs.
    /// </summary>
    public class GetManagementWebhookUrls
    {
        /// <summary>
        /// The Azure Function to retrieve HTTP management webhook URLs that external
        /// systems can communicate with Durable Functions through.
        /// </summary>
        /// <param name="req">The HTTP request.</param>
        /// <param name="client">The durable orchestration client.</param>
        /// <param name="instanceId">The ID of the orchestration instance being queried.</param>
        /// <returns>A payload containing the HTTP management webhook URLs.</returns>
        [FunctionName("GetManagementWebhookUrls")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = Routes.GetManagementWebhookUrls)]HttpRequest req,
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
