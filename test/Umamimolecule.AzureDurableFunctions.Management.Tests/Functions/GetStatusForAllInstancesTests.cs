using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
    public class GetStatusForAllInstancesTests : BaseFunctionTest<GetStatusForAllInstances>
    {
        [Fact]
        public async Task Success_NoQueryParameters()
        {
            using (var fixture = this.CreateTestFixture())
            {
                var instanceId = "1";
                var client = fixture.Client;

                var expectedResponse = new OrchestrationStatusQueryResult()
                {
                    ContinuationToken = null,
                    DurableOrchestrationState = new DurableOrchestrationStatus[]
                    {
                    new DurableOrchestrationStatus()
                    {
                        CreatedTime = DateTime.Now.AddDays(-1),
                        InstanceId = instanceId,
                        LastUpdatedTime = DateTime.Now.AddDays(-0.5),
                        Name = "My Instance",
                        RuntimeStatus = OrchestrationRuntimeStatus.Running
                    }
                    }
                };

                client.Setup(x => x.GetStatusAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(expectedResponse.DurableOrchestrationState.ToList());
                var result = await fixture.Instance.Run(this.CreateValidRequest(),
                                                  client.Object);
                result.ShouldNotBeNull();
                var objectResult = result.ShouldBeOfType<OkObjectResult>();
                var payload = objectResult.Value.ShouldBeOfType<Models.OrchestrationStatusQueryResult>();
                payload.ContinuationToken.ShouldBe(expectedResponse.ContinuationToken);
                payload.DurableOrchestrationState.ShouldNotBeNull();
                payload.DurableOrchestrationState.Count().ShouldBe(1);
                var status = payload.DurableOrchestrationState.First();
                var expectedStatus = expectedResponse.DurableOrchestrationState.First();
                status.CreatedTime.ShouldBe(expectedStatus.CreatedTime);
                status.CustomStatus.ShouldBe(expectedStatus.CustomStatus);
                status.History.ShouldBe(expectedStatus.History);
                status.Input.ShouldBe(expectedStatus.Input);
                status.InstanceId.ShouldBe(expectedStatus.InstanceId);
                status.LastUpdatedTime.ShouldBe(expectedStatus.LastUpdatedTime);
                status.Name.ShouldBe(expectedStatus.Name);
                status.Output.ShouldBe(expectedStatus.Output);
                status.RuntimeStatus.ShouldBe(expectedStatus.RuntimeStatus);
            }
        }

        [Fact]
        public async Task Success_QueryParameters()
        {
            using (var fixture = this.CreateTestFixture())
            {
                var instanceId = "1";
                var client = fixture.Client;

                var expectedResponse = new OrchestrationStatusQueryResult()
                {
                    ContinuationToken = null,
                    DurableOrchestrationState = new DurableOrchestrationStatus[]
                    {
                        new DurableOrchestrationStatus()
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
                        }
                    }
                };

                QueryCollection query = new QueryCollection(
                    new Dictionary<string, StringValues>()
                    {
                    { "runtimestatus", "Running,Completed" },
                    { "createdTimeFrom", "2019-12-13T09:30:15.123Z" },
                    { "CreatedTimeTo", "2020-12-13T09:30:15.123Z" },
                    { "InstanceIdPrefix", "blah" },
                    { "PageSize", "500" },
                    { "ShowInput", "true" },
                    { "TaskHubNames", "hub1,hub2" },
                    { "ContinuationToken", "abc" },
                    }
                );

                client.Setup(x => x.GetStatusAsync(It.IsAny<OrchestrationStatusQueryCondition>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(expectedResponse);
                var result = await fixture.Instance.Run(this.CreateValidRequest(query),
                                                  client.Object);
                result.ShouldNotBeNull();
                var objectResult = result.ShouldBeOfType<OkObjectResult>();
                var payload = objectResult.Value.ShouldBeOfType<Models.OrchestrationStatusQueryResult>();
                payload.ContinuationToken.ShouldBe(expectedResponse.ContinuationToken);
                payload.DurableOrchestrationState.ShouldNotBeNull();
                payload.DurableOrchestrationState.Count().ShouldBe(1);
                var status = payload.DurableOrchestrationState.First();
                var expectedStatus = expectedResponse.DurableOrchestrationState.First();
                status.CreatedTime.ShouldBe(expectedStatus.CreatedTime);
                status.CustomStatus.ShouldBe(expectedStatus.CustomStatus);
                status.History.ShouldBe(expectedStatus.History);
                status.Input.ShouldBe(expectedStatus.Input);
                status.InstanceId.ShouldBe(expectedStatus.InstanceId);
                status.LastUpdatedTime.ShouldBe(expectedStatus.LastUpdatedTime);
                status.Name.ShouldBe(expectedStatus.Name);
                status.Output.ShouldBe(expectedStatus.Output);
                status.RuntimeStatus.ShouldBe(expectedStatus.RuntimeStatus);
            }
        }

        [Fact]
        public async Task InvalidRuntimeStatus()
        {
            using (var fixture = this.CreateTestFixture())
            {
                var client = fixture.Client;

                QueryCollection query = new QueryCollection(
                    new Dictionary<string, StringValues>()
                    {
                    { "runtimestatus", "invalid" },
                    { "createdTimeFrom", "2019-12-13T09:30:15.123Z" },
                    { "CreatedTimeTo", "2020-12-13T09:30:15.123Z" },
                    { "InstanceIdPrefix", "blah" },
                    { "PageSize", "500" },
                    { "ShowInput", "true" },
                    { "TaskHubNames", "hub1,hub2" },
                    { "ContinuationToken", "abc" },
                    }
                );

                var result = await fixture.Instance.Run(this.CreateValidRequest(query), client.Object);
                result.ShouldNotBeNull();
                var objectResult = result.ShouldBeOfType<ObjectResult>();
                objectResult.StatusCode.ShouldBe(400);
                var payload = JsonConvert.DeserializeObject<ErrorPayload>(JsonConvert.SerializeObject(objectResult.Value));
                payload.Error.Code.ShouldBe("BADREQUEST");
                payload.Error.Message.ShouldBe("Invalid value 'invalid' for parameter 'RuntimeStatus'.  Valid values are: Running, Completed, ContinuedAsNew, Failed, Canceled, Terminated, Pending, Unknown");
            }
        }

        [Fact]
        public async Task InvalidCreatedTimeFrom()
        {
            using (var fixture = this.CreateTestFixture())
            {
                var client = fixture.Client;

                QueryCollection query = new QueryCollection(
                    new Dictionary<string, StringValues>()
                    {
                    { "runtimestatus", "Running" },
                    { "createdTimeFrom", "invalid" },
                    { "CreatedTimeTo", "2020-12-13T09:30:15.123Z" },
                    { "InstanceIdPrefix", "blah" },
                    { "PageSize", "500" },
                    { "ShowInput", "true" },
                    { "TaskHubNames", "hub1,hub2" },
                    { "ContinuationToken", "abc" },
                    }
                );

                var result = await fixture.Instance.Run(this.CreateValidRequest(query), client.Object);
                result.ShouldNotBeNull();
                var objectResult = result.ShouldBeOfType<ObjectResult>();
                objectResult.StatusCode.ShouldBe(400);
                var payload = JsonConvert.DeserializeObject<ErrorPayload>(JsonConvert.SerializeObject(objectResult.Value));
                payload.Error.Code.ShouldBe("BADREQUEST");
                payload.Error.Message.ShouldBe("Invalid value 'invalid' for parameter 'CreatedTimeFrom'.");
            }
        }

        [Fact]
        public async Task InvalidCreatedTimeTo()
        {
            using (var fixture = this.CreateTestFixture())
            {
                var client = fixture.Client;

                QueryCollection query = new QueryCollection(
                    new Dictionary<string, StringValues>()
                    {
                    { "runtimestatus", "Running" },
                    { "CreatedTimeFrom", "2020-12-13T09:30:15.123Z" },
                    { "createdTimeTo", "invalid" },
                    { "InstanceIdPrefix", "blah" },
                    { "PageSize", "500" },
                    { "ShowInput", "true" },
                    { "TaskHubNames", "hub1,hub2" },
                    { "ContinuationToken", "abc" },
                    }
                );

                var result = await fixture.Instance.Run(this.CreateValidRequest(query), client.Object);
                result.ShouldNotBeNull();
                var objectResult = result.ShouldBeOfType<ObjectResult>();
                objectResult.StatusCode.ShouldBe(400);
                var payload = JsonConvert.DeserializeObject<ErrorPayload>(JsonConvert.SerializeObject(objectResult.Value));
                payload.Error.Code.ShouldBe("BADREQUEST");
                payload.Error.Message.ShouldBe("Invalid value 'invalid' for parameter 'CreatedTimeTo'.");
            }
        }

        [Fact]
        public async Task InvalidPageSize()
        {
            using (var fixture = this.CreateTestFixture())
            {
                var client = fixture.Client;

                QueryCollection query = new QueryCollection(
                    new Dictionary<string, StringValues>()
                    {
                    { "runtimestatus", "Running" },
                    { "CreatedTimeFrom", "2019-12-13T09:30:15.123Z" },
                    { "createdTimeTo", "2020-12-13T09:30:15.123Z" },
                    { "InstanceIdPrefix", "blah" },
                    { "PageSize", "invalid" },
                    { "ShowInput", "true" },
                    { "TaskHubNames", "hub1,hub2" },
                    { "ContinuationToken", "abc" },
                    }
                );

                var result = await fixture.Instance.Run(this.CreateValidRequest(query), client.Object);
                result.ShouldNotBeNull();
                var objectResult = result.ShouldBeOfType<ObjectResult>();
                objectResult.StatusCode.ShouldBe(400);
                var payload = JsonConvert.DeserializeObject<ErrorPayload>(JsonConvert.SerializeObject(objectResult.Value));
                payload.Error.Code.ShouldBe("BADREQUEST");
                payload.Error.Message.ShouldBe("Invalid value 'invalid' for parameter 'PageSize'.");
            }
        }

        [Fact]
        public async Task InvalidShowInput()
        {
            using (var fixture = this.CreateTestFixture())
            {
                var client = fixture.Client;

                QueryCollection query = new QueryCollection(
                    new Dictionary<string, StringValues>()
                    {
                    { "runtimestatus", "Running" },
                    { "CreatedTimeFrom", "2019-12-13T09:30:15.123Z" },
                    { "createdTimeTo", "2020-12-13T09:30:15.123Z" },
                    { "InstanceIdPrefix", "blah" },
                    { "PageSize", "500" },
                    { "ShowInput", "invalid" },
                    { "TaskHubNames", "hub1,hub2" },
                    { "ContinuationToken", "abc" },
                    }
                );

                var result = await fixture.Instance.Run(this.CreateValidRequest(query), client.Object);
                result.ShouldNotBeNull();
                var objectResult = result.ShouldBeOfType<ObjectResult>();
                objectResult.StatusCode.ShouldBe(400);
                var payload = JsonConvert.DeserializeObject<ErrorPayload>(JsonConvert.SerializeObject(objectResult.Value));
                payload.Error.Code.ShouldBe("BADREQUEST");
                payload.Error.Message.ShouldBe("Invalid value 'invalid' for parameter 'ShowInput'.");
            }
        }

        [Fact]
        public async Task UnexpectedException_NoQueryParameters()
        {
            using (var fixture = this.CreateTestFixture())
            {
                var client = fixture.Client;

                client.Setup(x => x.GetStatusAsync(It.IsAny<CancellationToken>()))
                      .ThrowsAsync(new ApplicationException("Oops"));

                var result = await fixture.Instance.Run(this.CreateValidRequest(),
                                                        client.Object);
                result.ShouldNotBeNull();
                var objectResult = result.ShouldBeOfType<ObjectResult>();
                objectResult.StatusCode.ShouldBe(500);
                var payload = JsonConvert.DeserializeObject<ErrorPayload>(JsonConvert.SerializeObject(objectResult.Value));
                payload.Error.Code.ShouldBe("INTERNALSERVERERROR");
                payload.Error.Message.ShouldBe("Oops");
            }
        }

        [Fact]
        public async Task UnexpectedException_WithQueryParameters()
        {
            using (var fixture = this.CreateTestFixture())
            {
                var client = fixture.Client;

                QueryCollection query = new QueryCollection(
                    new Dictionary<string, StringValues>()
                    {
                    { "runtimestatus", "Running" },
                    { "CreatedTimeFrom", "2019-12-13T09:30:15.123Z" },
                    { "createdTimeTo", "2020-12-13T09:30:15.123Z" },
                    { "InstanceIdPrefix", "blah" },
                    { "PageSize", "500" },
                    { "ShowInput", "true" },
                    { "TaskHubNames", "hub1,hub2" },
                    { "ContinuationToken", "abc" },
                    }
                );

                client.Setup(x => x.GetStatusAsync(It.IsAny<OrchestrationStatusQueryCondition>(), It.IsAny<CancellationToken>()))
                      .ThrowsAsync(new ApplicationException("Oops"));

                var result = await fixture.Instance.Run(this.CreateValidRequest(query),
                                                        client.Object);
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
