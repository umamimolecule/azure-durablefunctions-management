using Microsoft.AspNetCore.Http;

namespace Umamimolecule.AzureDurableFunctions.Management.Tests.Functions
{
    public abstract class BaseFunctionTest<T> where T : new()
    {
        public FunctionTestFixture<T> CreateTestFixture()
        {
            return new FunctionTestFixture<T>();
        }

        protected abstract HttpRequest CreateValidRequest(IQueryCollection query = null);
    }
}
