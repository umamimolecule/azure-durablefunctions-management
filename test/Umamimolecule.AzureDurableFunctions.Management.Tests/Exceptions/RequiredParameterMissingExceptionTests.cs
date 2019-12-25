using Shouldly;
using Umamimolecule.AzureDurableFunctions.Management.Exceptions;
using Xunit;

namespace Umamimolecule.AzureDurableFunctions.Management.Tests.Exceptions
{
    public class RequiredParameterMissingExceptionTests
    {
        [Fact]
        public void Construct()
        {
            var e = new RequiredParameterMissingException("name");
            e.ParameterName.ShouldBe("name");
            e.InnerException.ShouldBeNull();
            e.Message.ShouldBe("The required parameter 'name' was missing.");
        }
    }
}
