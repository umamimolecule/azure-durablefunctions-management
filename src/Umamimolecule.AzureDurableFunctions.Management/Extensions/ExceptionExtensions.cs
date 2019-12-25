using System;
using Microsoft.AspNetCore.Mvc;
using Umamimolecule.AzureDurableFunctions.Management.Utility;

namespace Umamimolecule.AzureDurableFunctions.Management.Extensions
{
    public static class ExceptionExtensions
    {
        public static IActionResult ToUnhandledErrorResult(this Exception exception)
        {
            return ResponseHelper.CreateInternalServerErrorResult(exception.Message);
        }
    }
}
