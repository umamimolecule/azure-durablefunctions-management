using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DurableTask.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using Shouldly;
using Umamimolecule.AzureDurableFunctions.Management.Functions;
using Xunit;

namespace Umamimolecule.AzureDurableFunctions.Management.Tests.Functions
{
    public class PurgeInstanceHistoryForConditionTests : BaseFunctionTest<PurgeInstanceHistoryForCondition>
    {
        [Fact]
        public async Task Success()
        {
            using (var fixture = this.CreateTestFixture())
            {
                var client = fixture.Client;

                var query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>()
                {
                    { "createdTimeFrom", "2019-12-13T09:30:45.123Z" },
                    { "createdTimeTo", "2020-12-13T09:30:45.123Z" },
                    { "runtimeStatus", "Completed, Running" },
                });

                client.Setup(x => x.PurgeInstanceHistoryAsync(DateTime.Parse("2019-12-13T09:30:45.123Z"), DateTime.Parse("2020-12-13T09:30:45.123Z"), new OrchestrationStatus[] { OrchestrationStatus.Completed, OrchestrationStatus.Running }))
                      .ReturnsAsync(new Microsoft.Azure.WebJobs.Extensions.DurableTask.PurgeHistoryResult(3));

                var result = await fixture.Instance.Run(this.CreateValidRequest(query),
                                                  client.Object);
                result.ShouldNotBeNull();
                var objectResult = result.ShouldBeOfType<OkObjectResult>();
                objectResult.StatusCode.ShouldBe(200);
                var payload = objectResult.Value.ShouldBeOfType<Models.PurgeHistoryResult>();
                payload.InstancesDeleted.ShouldBe(3);
            }
        }

        [Fact]
        public async Task MissingCreatedTimeFrom()
        {
            using (var fixture = this.CreateTestFixture())
            {
                var client = fixture.Client;

                var query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>()
                {
                    { "createdTimeTo", "2020-12-13T09:30:45.123Z" },
                    { "runtimeStatus", "Completed, Running" },
                });
                var result = await fixture.Instance.Run(this.CreateValidRequest(query),
                                                  client.Object);
                result.ShouldNotBeNull();
                var objectResult = result.ShouldBeOfType<ObjectResult>();
                objectResult.StatusCode.ShouldBe(400);
                var payload = JsonConvert.DeserializeObject<ErrorPayload>(JsonConvert.SerializeObject(objectResult.Value));
                payload.Error.Code.ShouldBe("BADREQUEST");
                payload.Error.Message.ShouldBe("The required query parameter 'CreatedTimeFrom' was missing.");
            }
        }

        [Fact]
        public async Task UnexpectedException()
        {
            using (var fixture = this.CreateTestFixture())
            {
                var client = fixture.Client;

                client.Setup(x => x.PurgeInstanceHistoryAsync(It.IsAny<DateTime>(), It.IsAny<DateTime?>(), It.IsAny<IEnumerable<OrchestrationStatus>>()))
                      .ThrowsAsync(new ApplicationException("Oops"));

                var query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>()
                {
                    { "createdTimeFrom", "2019-12-13T09:30:45.123Z" },
                    { "createdTimeTo", "2020-12-13T09:30:45.123Z" },
                    { "runtimeStatus", "Completed, Running" },
                });

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
