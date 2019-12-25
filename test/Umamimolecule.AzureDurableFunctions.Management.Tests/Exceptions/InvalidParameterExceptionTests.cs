using System;
using Shouldly;
using Umamimolecule.AzureDurableFunctions.Management.Exceptions;
using Xunit;

namespace Umamimolecule.AzureDurableFunctions.Management.Tests.Exceptions
{
    public class InvalidParameterExceptionTests
    {
        [Fact]
        public void Construct()
        {
            var e = new InvalidParameterException("name", "value", null);
            e.ParameterName.ShouldBe("name");
            e.ParameterValue.ShouldBe("value");
            e.InnerException.ShouldBeNull();
            e.Message.ShouldBe("Invalid value 'value' for parameter 'name'.");
        }

        [Fact]
        public void ConstructWithInnerException()
        {
            var innerException = new ApplicationException("Oops");
            var e = new InvalidParameterException("name", "value", innerException);
            e.ParameterName.ShouldBe("name");
            e.ParameterValue.ShouldBe("value");
            e.InnerException.ShouldBe(innerException);
            e.Message.ShouldBe("Invalid value 'value' for parameter 'name'.");
        }
    }
}
