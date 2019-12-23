using System;
using Microsoft.AspNetCore.Mvc;

namespace Umamimolecule.AzureDurableFunctionsStatus.Extensions
{
    public static class ExceptionExtensions
    {
        public static IActionResult ToUnhandledErrorResult(this Exception exception)
        {
            dynamic value = new
            {
                error = new
                {
                    code = "INTERNALSERVERERROR",
                    message = exception.Message
                }
            };

            return new ObjectResult(value)
            {
                StatusCode = 500
            };
        }
    }
}
