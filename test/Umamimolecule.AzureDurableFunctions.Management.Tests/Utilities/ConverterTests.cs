using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;
using Umamimolecule.AzureDurableFunctions.Management.Exceptions;
using Umamimolecule.AzureDurableFunctions.Management.Utility;
using Xunit;

namespace Umamimolecule.AzureDurableFunctions.Management.Tests.Utilities
{
    public class ConverterTests
    {
        [Fact]
        public void EnumCollectionConverter_NullInput()
        {
            var converter = Converters.EnumCollectionConverter<MyEnum>("param");
            var result = converter(null);
            result.ShouldBeNull();
        }

        [Fact]
        public void EnumCollectionConverter_SingleInput()
        {
            var converter = Converters.EnumCollectionConverter<MyEnum>("param");
            var result = converter("A");
            result.ShouldBe(new MyEnum[] { MyEnum.A });
        }

        [Fact]
        public void EnumCollectionConverter_InvalidInput()
        {
            var converter = Converters.EnumCollectionConverter<MyEnum>("param");
            var exception = ShouldThrowExtensions.ShouldThrow<InvalidParameterException>(() => converter("D"));
            exception.ParameterName.ShouldBe("param");
            exception.ParameterValue.ShouldBe("D");
        }

        [Fact]
        public void EnumCollectionConverter_DefineValidValues()
        {
            var converter = Converters.EnumCollectionConverter<MyEnum>("param", new MyEnum[] { MyEnum.A, MyEnum.B });
            var exception = ShouldThrowExtensions.ShouldThrow<InvalidParameterException>(() => converter("C"));
            exception.ParameterName.ShouldBe("param");
            exception.ParameterValue.ShouldBe("C");
        }

        [Theory]
        [InlineData(",")]
        [InlineData(" ")]
        [InlineData(";")]
        public void EnumCollectionConverter_MultipleInput(string delimiter)
        {
            var converter = Converters.EnumCollectionConverter<MyEnum>("param");
            var result = converter($"A{delimiter}b{delimiter} C\t");
            result.ShouldBe(new MyEnum[] { MyEnum.A, MyEnum.B, MyEnum.C });
        }

        enum MyEnum
        {
            A,
            B,
            C
        }
    }
}
