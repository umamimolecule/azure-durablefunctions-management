using System;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Moq;

namespace Umamimolecule.AzureDurableFunctions.Management.Tests.Functions
{
    public class FunctionTestFixture<T> : IDisposable where T : new()
    {
        public FunctionTestFixture()
        {
            this.Instance = new T();
            this.Client = new Mock<IDurableOrchestrationClient>();
        }

        public T Instance { get; set; }

        public Mock<IDurableOrchestrationClient> Client { get; set; }

        public void Dispose()
        {
            // TODO: Dispose here if required
        }
    }
}
