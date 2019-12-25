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
    public class RaiseEventTests : BaseFunctionTest<RaiseEvent>
    {
        [Fact]
        public async Task Success_NoBody()
        {
            using (var fixture = this.CreateTestFixture())
            {
                var instanceId = "1";
                var eventName = "MyEvent";
                var client = fixture.Client;

                var query = new QueryCollection(new Dictionary<string, StringValues>()
            {
                { "eventName", eventName }
            });

                var result = await fixture.Instance.Run(this.CreateValidRequest(query),
                                                  client.Object,
                                                  instanceId);

                client.Verify(x => x.RaiseEventAsync(instanceId, eventName, null), Times.Once);

                result.ShouldNotBeNull();
                var objectResult = result.ShouldBeOfType<AcceptedResult>();
            }
        }

        [Fact]
        public async Task Success_WithBody()
        {
            using (var fixture = this.CreateTestFixture())
            {
                var instanceId = "1";
                var eventName = "MyEvent";
                var client = fixture.Client;

                var query = new QueryCollection(new Dictionary<string, StringValues>()
            {
                { "eventName", eventName }
            });

                dynamic body = new
                {
                    id = 1,
                    name = "Fred"
                };

                object receivedObject = null;
                client.Setup(x => x.RaiseEventAsync(instanceId, eventName, It.IsAny<object>()))
                      .Callback((string i, string e, object o) => { receivedObject = o; });

                var result = await fixture.Instance.Run(this.CreateValidRequest(query, body),
                                                  client.Object,
                                                  instanceId);

                var objectResult = result as AcceptedResult;
                objectResult.ShouldNotBeNull();
                client.Verify(x => x.RaiseEventAsync(instanceId, eventName, It.IsAny<object>()), Times.Once);
                var receivedObjectJson = JsonConvert.SerializeObject(receivedObject);
                receivedObjectJson.ShouldBe((string)JsonConvert.SerializeObject(body));
            }
        }

        [Fact]
        public async Task Success_WithBodyAndTaskHubName()
        {
            using (var fixture = this.CreateTestFixture())
            {
                var instanceId = "1";
                var eventName = "MyEvent";
                var taskHubName = "MyTaskHub";
                var connectionName = "MyConnection";
                var client = fixture.Client;

                var query = new QueryCollection(new Dictionary<string, StringValues>()
            {
                { "eventName", eventName },
                { "taskHubName", taskHubName },
                { "connectionName", connectionName }
            });

                dynamic body = new
                {
                    id = 1,
                    name = "Fred"
                };

                object receivedObject = null;
                client.Setup(x => x.RaiseEventAsync(taskHubName, instanceId, eventName, It.IsAny<object>(), connectionName))
                      .Callback((string t, string i, string e, object o, string c) => { receivedObject = o; });

                var result = await fixture.Instance.Run(this.CreateValidRequest(query, body),
                                                  client.Object,
                                                  instanceId);

                var objectResult = result as AcceptedResult;
                objectResult.ShouldNotBeNull();
                client.Verify(x => x.RaiseEventAsync(taskHubName, instanceId, eventName, It.IsAny<object>(), connectionName), Times.Once);
                var receivedObjectJson = JsonConvert.SerializeObject(receivedObject);
                receivedObjectJson.ShouldBe((string)JsonConvert.SerializeObject(body));
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
        public async Task InstanceIdNotFound()
        {
            using (var fixture = this.CreateTestFixture())
            {
                string instanceId = "1";
                string eventName = "MyEvent";
                var client = fixture.Client;

                var query = new QueryCollection(new Dictionary<string, StringValues>()
                {
                    { "eventName", eventName }
                });

                client.Setup(x => x.RaiseEventAsync(instanceId, eventName, null))
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
        public async Task InvalidOperationException()
        {
            using (var fixture = this.CreateTestFixture())
            {
                string instanceId = "1";
                string eventName = "MyEvent";
                var client = fixture.Client;

                var query = new QueryCollection(new Dictionary<string, StringValues>()
                {
                    { "eventName", eventName }
                });

                client.Setup(x => x.RaiseEventAsync(instanceId, eventName, null))
                      .ThrowsAsync(new InvalidOperationException("Oops"));

                var result = await fixture.Instance.Run(this.CreateValidRequest(query),
                                                  client.Object,
                                                  instanceId);

                result.ShouldNotBeNull();
                var objectResult = result.ShouldBeOfType<ObjectResult>();
                objectResult.StatusCode.ShouldBe(400);
                var payload = JsonConvert.DeserializeObject<ErrorPayload>(JsonConvert.SerializeObject(objectResult.Value));
                payload.Error.Code.ShouldBe("INVALIDOPERATION");
                payload.Error.Message.ShouldBe("Oops");
            }
        }

        [Fact]
        public async Task UnexpectedArgumentException()
        {
            using (var fixture = this.CreateTestFixture())
            {
                string instanceId = "1";
                string eventName = "MyEvent";
                var client = fixture.Client;

                var query = new QueryCollection(new Dictionary<string, StringValues>()
                {
                    { "eventName", eventName }
                });

                client.Setup(x => x.RaiseEventAsync(instanceId, eventName, null))
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
                string eventName = "MyEvent";
                var client = fixture.Client;

                var query = new QueryCollection(new Dictionary<string, StringValues>()
                {
                    { "eventName", eventName }
                });

                client.Setup(x => x.RaiseEventAsync(instanceId, eventName, null))
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

        private HttpRequest CreateValidRequest(IQueryCollection query = null, object body = null)
        {
            Mock<HttpRequest> request = new Mock<HttpRequest>();
            request.SetupGet(x => x.Query)
                   .Returns(query ?? new QueryCollection());

            MemoryStream bodyStream = new MemoryStream();
            StreamWriter writer = new StreamWriter(bodyStream);
            if (body != null)
            {
                writer.Write(JsonConvert.SerializeObject(body));
            }
            writer.Flush();
            bodyStream.Seek(0, SeekOrigin.Begin);

            request.SetupGet(x => x.Body)
                   .Returns(bodyStream);

            return request.Object;
        }
    }
}
