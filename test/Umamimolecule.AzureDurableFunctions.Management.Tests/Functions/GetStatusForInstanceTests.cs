using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Primitives;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shouldly;
using Umamimolecule.AzureDurableFunctions.Management.Functions;
using Xunit;

namespace Umamimolecule.AzureDurableFunctions.Management.Tests.Functions
{
    public class GetStatusForInstanceTests : BaseFunctionTest<GetStatusForInstance>
    {
        [Theory]
        [InlineData(false, false, false)]
        [InlineData(false, false, true)]
        [InlineData(false, true, false)]
        [InlineData(false, true, true)]
        [InlineData(true, false, false)]
        [InlineData(true, false, true)]
        [InlineData(true, true, false)]
        [InlineData(true, true, true)]
        public async Task Success(bool showHistory, bool showHistoryOutput, bool showInput)
        {
            using (var fixture = this.CreateTestFixture())
            {
                var instanceId = "1";
                var client = fixture.Client;

                var expectedResponse = new DurableOrchestrationStatus()
                {
                    CreatedTime = DateTime.Now.AddDays(-1),
                    InstanceId = instanceId,
                    LastUpdatedTime = DateTime.Now.AddDays(-0.5),
                    Name = "My Instance",
                    RuntimeStatus = OrchestrationRuntimeStatus.Running,
                    CustomStatus = JToken.Parse("{\"customStatus\":\"abc123\"}"),
                    History = new JArray(JToken.Parse("{\"history[0]\":\"abc123\"}")),
                    Input = JToken.Parse("{\"input\":\"abc123\"}"),
                    Output = JToken.Parse("{\"output\":\"abc123\"}"),
                };

                var query = new QueryCollection(new Dictionary<string, StringValues>()
            {
                { "showHistory", showHistory.ToString() },
                { "showHistoryOutput", showHistoryOutput.ToString() },
                { "showInput", showInput.ToString() },
            });

                client.Setup(x => x.GetStatusAsync(instanceId, showHistory, showHistoryOutput, showInput))
                      .ReturnsAsync(expectedResponse);
                var result = await fixture.Instance.Run(this.CreateValidRequest(query),
                                                  client.Object,
                                                  instanceId);
                result.ShouldNotBeNull();
                var objectResult = result.ShouldBeOfType<OkObjectResult>();
                var payload = objectResult.Value.ShouldBeOfType<Models.DurableOrchestrationStatus>();
                payload.CreatedTime.ShouldBe(expectedResponse.CreatedTime);
                payload.CustomStatus.ShouldBe(expectedResponse.CustomStatus);
                payload.History.ShouldBe(expectedResponse.History);
                payload.Input.ShouldBe(expectedResponse.Input);
                payload.InstanceId.ShouldBe(expectedResponse.InstanceId);
                payload.LastUpdatedTime.ShouldBe(expectedResponse.LastUpdatedTime);
                payload.Name.ShouldBe(expectedResponse.Name);
                payload.Output.ShouldBe(expectedResponse.Output);
                payload.RuntimeStatus.ShouldBe(expectedResponse.RuntimeStatus);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        public async Task MissingOrNullInstanceId(string instanceId)
        {
            using (var fixture = this.CreateTestFixture())
            {
                var client = fixture.Client;

                var result = await fixture.Instance.Run(this.CreateValidRequest(),
                                                  client.Object,
                                                  instanceId);
                result.ShouldNotBeNull();
                var objectResult = result.ShouldBeOfType<ObjectResult>();
                objectResult.StatusCode.ShouldBe(400);
                var payload = JsonConvert.DeserializeObject<ErrorPayload>(JsonConvert.SerializeObject(objectResult.Value));
                payload.Error.Code.ShouldBe("BADREQUEST");
                payload.Error.Message.ShouldBe("The required parameter 'instanceId' was missing.");
            }
        }

        [Fact]
        public async Task GetStatusAsyncReturnsNull()
        {
            using (var fixture = this.CreateTestFixture())
            {
                string instanceId = "1";
                var client = fixture.Client;

                client.Setup(x => x.GetStatusAsync(instanceId, false, false, false))
                      .ReturnsAsync((DurableOrchestrationStatus)null);

                var result = await fixture.Instance.Run(this.CreateValidRequest(),
                                                  client.Object,
                                                  instanceId);
                result.ShouldNotBeNull();
                var objectResult = result.ShouldBeOfType<NotFoundObjectResult>();
                objectResult.StatusCode.ShouldBe(404);
                var payload = JsonConvert.DeserializeObject<ErrorPayload>(JsonConvert.SerializeObject(objectResult.Value));
                payload.Error.Code.ShouldBe("NOTFOUND");
                payload.Error.Message.ShouldBe("No instance with ID '1' was found.");
            }
        }

        [Fact]
        public async Task UnexpectedException()
        {
            using (var fixture = this.CreateTestFixture())
            {
                string instanceId = "1";
                var client = fixture.Client;

                client.Setup(x => x.GetStatusAsync(instanceId, false, false, false))
                      .ThrowsAsync(new ApplicationException("Oops"));

                var result = await fixture.Instance.Run(this.CreateValidRequest(),
                                                  client.Object,
                                                  instanceId);
                result.ShouldNotBeNull();
                var objectResult = result.ShouldBeOfType<ObjectResult>();
                objectResult.StatusCode.ShouldBe(500);
                var payload = JsonConvert.DeserializeObject<ErrorPayload>(JsonConvert.SerializeObject(objectResult.Value));
                payload.Error.Code.ShouldBe("INTERNALSERVERERROR");
                payload.Error.Message.ShouldBe("Oops");
            }
        }


        private HttpRequest CreateValidRequest(IQueryCollection query = null)
        {
            Mock<HttpRequest> request = new Mock<HttpRequest>();
            request.SetupGet(x => x.Query)
                   .Returns(query ?? new QueryCollection());
            return request.Object;
        }
    }
}
