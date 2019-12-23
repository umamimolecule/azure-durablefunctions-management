using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umamimolecule.AzureDurableFunctionsStatus.Exceptions;
using Umamimolecule.AzureDurableFunctionsStatus.Extensions;

namespace Umamimolecule.AzureDurableFunctionsStatus.Functions
{
    public class StartInstance
    {
        [FunctionName("StartInstance")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "instances/startNew")]HttpRequest req,
            [DurableClient] IDurableOrchestrationClient client)
        {
            try
            {
                var orchestratorFunctionName = req.Query.GetQueryParameter("OrchestratorFunctionName", true, x => x);
                var body = await req.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(body))
                {
                    var instanceId = await client.StartNewAsync(orchestratorFunctionName);
                    dynamic result = new
                    {
                        instanceId
                    };
                    return new OkObjectResult(result);
                }
                else
                {
                    var input = JsonConvert.DeserializeObject<JObject>(body);
                    var instanceId = await client.StartNewAsync(orchestratorFunctionName, input);
                    dynamic result = new
                    {
                        instanceId
                    };
                    return new OkObjectResult(result);
                }
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
