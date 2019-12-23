using Microsoft.AspNetCore.Http;

namespace Umamimolecule.AzureDurableFunctions.Management.Tests
{
    public abstract class BaseFunctionTest<T> where T : new()
    {
        public FunctionTestFixture<T> CreateTestFixture()
        {
            return new FunctionTestFixture<T>();
        }

        protected abstract HttpRequest CreateValidRequest();
    }
}
