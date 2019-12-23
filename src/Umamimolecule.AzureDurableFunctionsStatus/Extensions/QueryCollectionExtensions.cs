using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Umamimolecule.AzureDurableFunctionsStatus.Exceptions;

namespace Umamimolecule.AzureDurableFunctionsStatus.Extensions
{
    public static class QueryCollectionExtensions
    {
        public static T GetQueryParameter<T>(this IQueryCollection query, string key, bool required, Func<string, T> converter, T defaultValue = default(T))
        {
            if (!query.TryGetValue(key, out var value) || value.Count == 0 || (value.Count == 1 && string.IsNullOrWhiteSpace(value[0])))
            {
                if (required)
                {
                    throw new RequiredQueryParameterMissingException(key);
                }

                return defaultValue;
            }

            return converter(value.First());
        }
    }
}
