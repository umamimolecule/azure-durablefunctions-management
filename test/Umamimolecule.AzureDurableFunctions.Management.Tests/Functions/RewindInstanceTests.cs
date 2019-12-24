using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;
using Newtonsoft.Json;
using Shouldly;
using Umamimolecule.AzureDurableFunctions.Management.Functions;
using Xunit;

namespace Umamimolecule.AzureDurableFunctions.Management.Tests.Functions
{
    public class RewindInstanceTests : BaseFunctionTest<RewindInstance>
    {
        [Fact]
        public async Task Success()
        {
            using (var fixture = this.CreateTestFixture())
            {
                var instanceId = "1";
                var reason = "It's stuck";
                var client = fixture.Client;

                var query = new QueryCollection(new Dictionary<string, StringValues>()
                {
                    { "reason", reason }
                });

                var result = await fixture.Instance.Run(this.CreateValidRequest(query),
                                                  client.Object,
                                                  instanceId);

#pragma warning disable CS0618 // Type or member is obsolete
                client.Verify(x => x.RewindAsync(instanceId, reason), Times.Once);
#pragma warning restore CS0618 // Type or member is obsolete

                result.ShouldNotBeNull();
                var objectResult = result.ShouldBeOfType<AcceptedResult>();
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
                var reason = "It's stuck";

                var query = new QueryCollection(new Dictionary<string, StringValues>()
                {
                    { "reason", reason }
                });

                var result = await fixture.Instance.Run(this.CreateValidRequest(query),
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

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        public async Task MissingOrNullReason(string reason)
        {
            using (var fixture = this.CreateTestFixture())
            {
                var client = fixture.Client;
                var instanceId = "1";

                var query = new QueryCollection(new Dictionary<string, StringValues>()
                {
                    { "reason", reason }
                });

                var result = await fixture.Instance.Run(this.CreateValidRequest(query),
                                                  client.Object,
                                                  instanceId);
                result.ShouldNotBeNull();
                var objectResult = result.ShouldBeOfType<ObjectResult>();
                objectResult.StatusCode.ShouldBe(400);
                var payload = JsonConvert.DeserializeObject<ErrorPayload>(JsonConvert.SerializeObject(objectResult.Value));
                payload.Error.Code.ShouldBe("BADREQUEST");
                payload.Error.Message.ShouldBe("The required query parameter 'Reason' was missing.");
            }
        }

        [Fact]
        public async Task InstanceIdNotFound()
        {
            using (var fixture = this.CreateTestFixture())
            {
                string instanceId = "1";
                string reason = "It's stuck";
                var client = fixture.Client;

                var query = new QueryCollection(new Dictionary<string, StringValues>()
                {
                    { "reason", reason }
                });

#pragma warning disable CS0618 // Type or member is obsolete
                client.Setup(x => x.RewindAsync(instanceId, reason))
#pragma warning restore CS0618 // Type or member is obsolete
                      .ThrowsAsync(new ArgumentException("Oops", "instanceId"));

                var result = await fixture.Instance.Run(this.CreateValidRequest(query),
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
        public async Task UnexpectedArgumentException()
        {
            using (var fixture = this.CreateTestFixture())
            {
                string instanceId = "1";
                string reason = "It's stuck";
                var client = fixture.Client;

                var query = new QueryCollection(new Dictionary<string, StringValues>()
                {
                    { "reason", reason }
                });

#pragma warning disable CS0618 // Type or member is obsolete
                client.Setup(x => x.RewindAsync(instanceId, reason))
#pragma warning restore CS0618 // Type or member is obsolete
                      .ThrowsAsync(new ArgumentException("Oops", "notInstanceId"));

                var result = await fixture.Instance.Run(this.CreateValidRequest(query),
                                                  client.Object,
                                                  instanceId);

                result.ShouldNotBeNull();
                var objectResult = result.ShouldBeOfType<ObjectResult>();
                objectResult.StatusCode.ShouldBe(500);
                var payload = JsonConvert.DeserializeObject<ErrorPayload>(JsonConvert.SerializeObject(objectResult.Value));
                payload.Error.Code.ShouldBe("INTERNALSERVERERROR");
                payload.Error.Message.ShouldBe("Oops\r\nParameter name: notInstanceId");
            }
        }

        [Fact]
        public async Task UnexpectedException()
        {
            using (var fixture = this.CreateTestFixture())
            {
                string instanceId = "1";
                string reason = "It's stuck";
                var client = fixture.Client;

                var query = new QueryCollection(new Dictionary<string, StringValues>()
                {
                    { "reason", reason }
                });

#pragma warning disable CS0618 // Type or member is obsolete
                client.Setup(x => x.RewindAsync(instanceId, reason))
#pragma warning restore CS0618 // Type or member is obsolete
                      .ThrowsAsync(new ApplicationException("Oops"));

                var result = await fixture.Instance.Run(this.CreateValidRequest(query),
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
