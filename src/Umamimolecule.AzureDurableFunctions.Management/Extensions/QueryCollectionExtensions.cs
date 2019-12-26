using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Umamimolecule.AzureDurableFunctions.Management.Exceptions;

namespace Umamimolecule.AzureDurableFunctions.Management.Extensions
{
    /// <summary>
    /// Contains extension methods for <see cref="IQueryCollection"/> instances.
    /// </summary>
    public static class QueryCollectionExtensions
    {
        /// <summary>
        /// Gets a query parameter value and validates it.
        /// </summary>
        /// <typeparam name="T">The object type to convert the value to.</typeparam>
        /// <param name="query">The query collection.</param>
        /// <param name="key">The key of the query parameter to get.</param>
        /// <param name="required">Determines whether the parameter is mandatory.  If missing or has a null of empty value, a <see cref="RequiredQueryParameterMissingException"/> exception will be thrown.</param>
        /// <param name="converter">The function to perform to conversion form a string to the specified type.</param>
        /// <param name="defaultValue">The default value to return if the query parameter is missing or has a null or empty value.</param>
        /// <returns>The query parameter value.</returns>
        /// <exception cref="RequiredQueryParameterMissingException">Thrown when a required query parameter is missing or has an null or empty value.</exception>.
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
