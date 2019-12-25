using Shouldly;
using Umamimolecule.AzureDurableFunctions.Management.Exceptions;
using Xunit;

namespace Umamimolecule.AzureDurableFunctions.Management.Tests.Exceptions
{
    public class RequiredQueryParameterMissingExceptionTests
    {
        [Fact]
        public void Construct()
        {
            var e = new RequiredQueryParameterMissingException("name");
            e.ParameterName.ShouldBe("name");
            e.InnerException.ShouldBeNull();
            e.Message.ShouldBe("The required query parameter 'name' was missing.");
        }
    }
}
