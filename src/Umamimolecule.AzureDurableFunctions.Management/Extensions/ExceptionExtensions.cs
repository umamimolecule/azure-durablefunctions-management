using System;
using Microsoft.AspNetCore.Mvc;
using Umamimolecule.AzureDurableFunctions.Management.Utility;

namespace Umamimolecule.AzureDurableFunctions.Management.Extensions
{
    /// <summary>
    /// Contains extension methods for <see cref="Exception"/> instances.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Creates an <see cref="IActionResult"/> instance representing an unhandled error.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>The <see cref="IActionResult"/> instance.</returns>
        public static IActionResult ToUnhandledErrorResult(this Exception exception)
        {
            return ResponseHelper.CreateInternalServerErrorResult(exception.Message);
        }
    }
}
