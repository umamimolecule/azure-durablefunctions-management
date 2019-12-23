using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Primitives;
using Moq;
using Newtonsoft.Json;
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
            using var fixture = this.CreateTestFixture();
            var instanceId = "1";
            var client = fixture.Client;

            var expectedResponse = new DurableOrchestrationStatus()
            {
                CreatedTime = DateTime.Now.AddDays(-1),
                InstanceId = instanceId,
                LastUpdatedTime = DateTime.Now.AddDays(-0.5),
                Name = "My Instance",
                RuntimeStatus = OrchestrationRuntimeStatus.Running
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
            var payload = objectResult.Value.ShouldBeOfType<DurableOrchestrationStatus>();
            payload.ShouldBe(expectedResponse);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        public async Task MissingOrNullInstanceId(string instanceId)
        {
            using var fixture = this.CreateTestFixture();
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

        [Fact]
        public async Task GetStatusAsyncReturnsNull()
        {
            using var fixture = this.CreateTestFixture();
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

        [Fact]
        public async Task UnexpectedException()
        {
            using var fixture = this.CreateTestFixture();
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

        //[Fact]
        //public async Task UnexpectedException_WithQueryParameters()
        //{
        //    using var fixture = this.CreateTestFixture();
        //    var client = fixture.Client;

        //    QueryCollection query = new QueryCollection(
        //        new Dictionary<string, StringValues>()
        //        {
        //            { "runtimestatus", "Running" },
        //            { "CreatedTimeFrom", "2019-12-13T09:30:15.123Z" },
        //            { "createdTimeTo", "2020-12-13T09:30:15.123Z" },
        //            { "InstanceIdPrefix", "blah" },
        //            { "PageSize", "500" },
        //            { "ShowInput", "true" },
        //            { "TaskHubNames", "hub1,hub2" },
        //            { "ContinuationToken", "abc" },
        //        }
        //    );

        //    client.Setup(x => x.GetStatusAsync(It.IsAny<OrchestrationStatusQueryCondition>(), It.IsAny<CancellationToken>()))
        //          .ThrowsAsync(new ApplicationException("Oops"));

        //    var result = await fixture.Instance.Run(this.CreateValidRequest(query),
        //                                            client.Object);
        //    result.ShouldNotBeNull();
        //    var objectResult = result.ShouldBeOfType<ObjectResult>();
        //    objectResult.StatusCode.ShouldBe(500);
        //    var payload = JsonConvert.DeserializeObject<ErrorPayload>(JsonConvert.SerializeObject(objectResult.Value));
        //    payload.Error.Code.ShouldBe("INTERNALSERVERERROR");
        //    payload.Error.Message.ShouldBe("Oops");
        //}

        protected override HttpRequest CreateValidRequest(IQueryCollection query = null)
        {
            Mock<HttpRequest> request = new Mock<HttpRequest>();
            request.SetupGet(x => x.Query)
                   .Returns(query ?? new QueryCollection());
            return request.Object;
        }
    }
}
