using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Umamimolecule.AzureDurableFunctions.Management.Exceptions;

namespace Umamimolecule.AzureDurableFunctions.Management.Extensions
{
    public static class QueryCollectionExtensions
    {
        public static T GetQueryParameter<T>(this IQueryCollection query, string key, bool required, Func<string, T> converter, T defaultValue = default(T))
        {
            var value = query.FirstOrDefault(x => string.Compare(x.Key, key, true) == 0);
            if (value.Value.Count == 0 || (value.Value.Count == 1 && string.IsNullOrWhiteSpace(value.Value[0])))
            {
                if (required)
                {
                    throw new RequiredQueryParameterMissingException(key);
                }

                return defaultValue;
            }

            return converter(value.Value.First());
        }
    }
}
