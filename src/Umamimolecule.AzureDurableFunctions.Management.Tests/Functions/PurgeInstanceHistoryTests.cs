using System;
using System.Threading.Tasks;
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
    public class PurgeInstanceHistoryTests : BaseFunctionTest<PurgeInstanceHistory>
    {
        [Fact]
        public async Task Success()
        {
            using var fixture = this.CreateTestFixture();
            var instanceId = "1";
            var client = fixture.Client;

            var result = await fixture.Instance.Run(this.CreateValidRequest(),
                                              client.Object,
                                              instanceId);
            result.ShouldNotBeNull();
            var objectResult = result.ShouldBeOfType<AcceptedResult>();
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
        public async Task PurgeInstanceHistoryAsyncThrowsException()
        {
            using var fixture = this.CreateTestFixture();
            string instanceId = "1";
            var client = fixture.Client;

            client.Setup(x => x.PurgeInstanceHistoryAsync(instanceId))
                  .ThrowsAsync(new ArgumentException("Oops", "instanceId"));

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

            client.Setup(x => x.PurgeInstanceHistoryAsync(instanceId))
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

        protected override HttpRequest CreateValidRequest(IQueryCollection query = null)
        {
            Mock<HttpRequest> request = new Mock<HttpRequest>();
            request.SetupGet(x => x.Query)
                   .Returns(query ?? new QueryCollection());
            return request.Object;
        }
    }
}
