using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Moq;
using Newtonsoft.Json;
using Shouldly;
using System;
using Umamimolecule.AzureDurableFunctions.Management.Functions;
using Umamimolecule.AzureDurableFunctions.Management.Tests.Extensions;
using Xunit;

namespace Umamimolecule.AzureDurableFunctions.Management.Tests
{
    public class GetManagementWebhookUrlsTests : BaseFunctionTest<GetManagementWebhookUrls>
    {
        [Fact]
        public void Success()
        {
            using var fixture = this.CreateTestFixture();
            var instanceId = "1";
            var client = fixture.Client;

            var expectedResponse = new HttpManagementPayload();
            expectedResponse.SetPropertyValue("Id", instanceId);
            expectedResponse.SetPropertyValue("StatusQueryGetUri", "https://StatusQueryGetUri");
            expectedResponse.SetPropertyValue("SendEventPostUri", "https://SendEventPostUri");
            expectedResponse.SetPropertyValue("TerminatePostUri", "https://TerminatePostUri");
            expectedResponse.SetPropertyValue("RewindPostUri", "https://RewindPostUri");
            expectedResponse.SetPropertyValue("PurgeHistoryDeleteUri", "https://PurgeHistoryDeleteUri");

            client.Setup(x => x.CreateHttpManagementPayload(instanceId))
                  .Returns(expectedResponse);
            var result = fixture.Instance.Run(this.CreateValidRequest(),
                                              client.Object,
                                              instanceId);
            result.ShouldNotBeNull();
            var objectResult = result.ShouldBeOfType<OkObjectResult>();
            var payload = objectResult.Value.ShouldBeOfType<HttpManagementPayload>();
            payload.Id.ShouldBe(expectedResponse.Id);
            payload.StatusQueryGetUri.ShouldBe(expectedResponse.StatusQueryGetUri);
            payload.SendEventPostUri.ShouldBe(expectedResponse.SendEventPostUri);
            payload.TerminatePostUri.ShouldBe(expectedResponse.TerminatePostUri);
            payload.RewindPostUri.ShouldBe(expectedResponse.RewindPostUri);
            payload.PurgeHistoryDeleteUri.ShouldBe(expectedResponse.PurgeHistoryDeleteUri);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(" ")]
        [InlineData("\t")]
        public void NullOrMissingInstanceId(string instanceId)
        {
            using var fixture = this.CreateTestFixture();
            var client = fixture.Client;

            var expectedResponse = new HttpManagementPayload();
            expectedResponse.SetPropertyValue("Id", instanceId);
            expectedResponse.SetPropertyValue("StatusQueryGetUri", "https://StatusQueryGetUri");
            expectedResponse.SetPropertyValue("SendEventPostUri", "https://SendEventPostUri");
            expectedResponse.SetPropertyValue("TerminatePostUri", "https://TerminatePostUri");
            expectedResponse.SetPropertyValue("RewindPostUri", "https://RewindPostUri");
            expectedResponse.SetPropertyValue("PurgeHistoryDeleteUri", "https://PurgeHistoryDeleteUri");

            client.Setup(x => x.CreateHttpManagementPayload(instanceId))
                  .Returns(expectedResponse);
            var result = fixture.Instance.Run(this.CreateValidRequest(),
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
        public void UnexpectedException()
        {
            using var fixture = this.CreateTestFixture();
            var instanceId = "1";
            var client = fixture.Client;

            client.Setup(x => x.CreateHttpManagementPayload(instanceId))
                  .Throws(new ApplicationException("Oops"));
            var result = fixture.Instance.Run(this.CreateValidRequest(),
                                              client.Object,
                                              instanceId);
            result.ShouldNotBeNull();
            var objectResult = result.ShouldBeOfType<ObjectResult>();
            objectResult.StatusCode.ShouldBe(500);
            var payload = JsonConvert.DeserializeObject<ErrorPayload>(JsonConvert.SerializeObject(objectResult.Value));
            payload.Error.Code.ShouldBe("INTERNALSERVERERROR");
            payload.Error.Message.ShouldBe("Oops");
        }

        protected override HttpRequest CreateValidRequest()
        {
            Mock<HttpRequest> request = new Mock<HttpRequest>();
            return request.Object;
        }
    }
}
