using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using Shouldly;
using System.Collections.Generic;
using Umamimolecule.AzureDurableFunctions.Management.Exceptions;
using Umamimolecule.AzureDurableFunctions.Management.Extensions;
using Xunit;

namespace Umamimolecule.AzureDurableFunctions.Management.Tests.Extensions
{
    public class QueryCollectionExtensionsTests
    {
        [Fact]
        public void GetQueryParameter_NotFoundNullDefault()
        {
            var value = QueryCollectionExtensions.GetQueryParameter(new QueryCollection(), "a", false, x => x, null);
            value.ShouldBeNull();
        }

        [Fact]
        public void GetQueryParameter_NotFoundEmptyDefault()
        {
            var value = QueryCollectionExtensions.GetQueryParameter(new QueryCollection(), "a", false, x => x, string.Empty);
            value.ShouldBe(string.Empty);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        [InlineData(" ")]
        public void GetQueryParameter_FoundButEmptyValue(string parameterValue)
        {
            var query = new QueryCollection(new Dictionary<string, StringValues>()
            {
                { "a", new StringValues(parameterValue) }
            });

            var value = QueryCollectionExtensions.GetQueryParameter(query, "a", false, x => x, string.Empty);
            value.ShouldBe(string.Empty);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("\t")]
        [InlineData(" ")]
        public void GetQueryParameter_FoundButEmptyValue_Required(string parameterValue)
        {
            var query = new QueryCollection(new Dictionary<string, StringValues>()
            {
                { "a", new StringValues(parameterValue) }
            });

            var exception = ShouldThrowExtensions.ShouldThrow<RequiredQueryParameterMissingException>(() => QueryCollectionExtensions.GetQueryParameter(query, "a", true, x => x, string.Empty));
            exception.ParameterName.ShouldBe("a");
        }
    }
}
