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
    public class StartInstance
    {
        [FunctionName("StartInstance")]
        public virtual async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "instances/startNew")]HttpRequest req,
            [DurableClient] IDurableOrchestrationClient client)
        {
            try
            {
                var orchestratorFunctionName = req.Query.GetQueryParameter("OrchestratorFunctionName", true, x => x);
                var specifiedInstanceId = req.Query.GetQueryParameter("InstanceId", false, x => x, string.Empty);
                if (string.IsNullOrWhiteSpace(specifiedInstanceId))
                {
                    specifiedInstanceId = string.Empty;
                }

                var body = await req.ReadAsStringAsync();
                var input = string.IsNullOrWhiteSpace(body) ? null : JsonConvert.DeserializeObject<JObject>(body);
                var instanceId = await client.StartNewAsync(orchestratorFunctionName, specifiedInstanceId, input);
                dynamic result = new
                {
                    instanceId
                };

                return new OkObjectResult(result);
            }
            catch (ArgumentException ae)
            {
                dynamic value = new
                {
                    error = new
                    {
                        code = "NOTFOUND",
                        message = ae.Message
                    }
                };

                return new NotFoundObjectResult(value);
            }
            catch (StatusCodeException sce)
            {
                return sce.ToObjectResult();
            }
            catch (Exception e)
            {
                return e.ToUnhandledErrorResult();
            }
        }
    }
}
