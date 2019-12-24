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
    public class StartInstanceTests : BaseFunctionTest<StartInstance>
    {
        [Fact]
        public async Task Success_NoBody()
        {
            using var fixture = this.CreateTestFixture();
            var orchestratorFunctionName = "MyFunction";
            var client = fixture.Client;

            var query = new QueryCollection(new Dictionary<string, StringValues>()
            {
                { "orchestratorFunctionName", orchestratorFunctionName },
            });

            client.Setup(x => x.StartNewAsync(orchestratorFunctionName, string.Empty, (object)null))
                  .ReturnsAsync("random");

            var result = await fixture.Instance.Run(this.CreateValidRequest(query),
                                                    client.Object);

            client.Verify(x => x.StartNewAsync(orchestratorFunctionName, string.Empty, (object)null), Times.Once);

            result.ShouldNotBeNull();
            var objectResult = result.ShouldBeOfType<OkObjectResult>();
            var payloadJson = JsonConvert.SerializeObject(objectResult.Value);
            payloadJson.ShouldBe("{\"instanceId\":\"random\"}");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        [InlineData("instance123")]
        public async Task Success_WithBody(string instanceId)
        {
            using var fixture = this.CreateTestFixture();
            var orchestratorFunctionName = "MyFunction";
            var client = fixture.Client;

            var query = new QueryCollection(new Dictionary<string, StringValues>()
            {
                { "orchestratorFunctionName", orchestratorFunctionName },
                { "instanceId", instanceId }
            });

            dynamic body = new
            {
                id = 1,
                name = "Fred"
            };

            var expectedInstanceId = string.IsNullOrWhiteSpace(instanceId) ? "random" : instanceId;
            object receivedObject = null;
            client.Setup(x => x.StartNewAsync(orchestratorFunctionName, string.IsNullOrWhiteSpace(instanceId) ? string.Empty : instanceId, It.IsAny<object>()))
                  .ReturnsAsync(expectedInstanceId)
                  .Callback((string i, string e, object o) => { receivedObject = o; });

            var result = await fixture.Instance.Run(this.CreateValidRequest(query, body),
                                                    client.Object);

            var objectResult = result as OkObjectResult;
            objectResult.ShouldNotBeNull();
            var payloadJson = JsonConvert.SerializeObject(objectResult.Value);
            payloadJson.ShouldBe("{\"instanceId\":\"" + expectedInstanceId + "\"}");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        public async Task MissingOrNullOrchestratorFunctionName(string orchestratorFunctionName)
        {
            using var fixture = this.CreateTestFixture();
            var client = fixture.Client;

            var query = new QueryCollection(new Dictionary<string, StringValues>()
            {
                { "orchestratorFunctionName", orchestratorFunctionName },
            });

            var result = await fixture.Instance.Run(this.CreateValidRequest(query),
                                                    client.Object);
            result.ShouldNotBeNull();
            var objectResult = result.ShouldBeOfType<ObjectResult>();
            objectResult.StatusCode.ShouldBe(400);
            var payload = JsonConvert.DeserializeObject<ErrorPayload>(JsonConvert.SerializeObject(objectResult.Value));
            payload.Error.Code.ShouldBe("BADREQUEST");
            payload.Error.Message.ShouldBe("The required query parameter 'OrchestratorFunctionName' was missing.");
        }

        [Fact]
        public async Task OrchestratorFunctionNameNotFound()
        {
            using var fixture = this.CreateTestFixture();
            string orchestratorFunctionName = "MyFunction";
            var client = fixture.Client;

            var query = new QueryCollection(new Dictionary<string, StringValues>()
            {
                { "orchestratorFunctionName", orchestratorFunctionName },
            });

            client.Setup(x => x.StartNewAsync(orchestratorFunctionName, string.Empty, (object)null))
                  .ThrowsAsync(new ArgumentException($"A function with name {orchestratorFunctionName} cannot be found."));

            var result = await fixture.Instance.Run(this.CreateValidRequest(query),
                                                    client.Object);

            result.ShouldNotBeNull();
            var objectResult = result.ShouldBeOfType<NotFoundObjectResult>();
            objectResult.StatusCode.ShouldBe(404);
            var payload = JsonConvert.DeserializeObject<ErrorPayload>(JsonConvert.SerializeObject(objectResult.Value));
            payload.Error.Code.ShouldBe("NOTFOUND");
            payload.Error.Message.ShouldBe($"A function with name {orchestratorFunctionName} cannot be found.");
        }

        [Fact]
        public async Task UnexpectedException()
        {
            using var fixture = this.CreateTestFixture();
            string orchestratorFunctionName = "MyFunction";
            var client = fixture.Client;

            var query = new QueryCollection(new Dictionary<string, StringValues>()
            {
                { "orchestratorFunctionName", orchestratorFunctionName },
            });

            client.Setup(x => x.StartNewAsync(orchestratorFunctionName, string.Empty, (object)null))
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
