using Microsoft.AspNetCore.Mvc;
using System.Net;
using Umamimolecule.AzureDurableFunctions.Management.Exceptions;

namespace Umamimolecule.AzureDurableFunctions.Management.Utility
{
    public static class ResponseHelper
    {
        public static IActionResult CreateInvalidOperationResult(string message)
        {
            return new ObjectResult(CreateError(ErrorCodes.InvalidOperation, message))
            {
                StatusCode = 400
            };
        }

        public static IActionResult CreateNotFoundResult(string message)
        {
            return new NotFoundObjectResult(CreateError(ErrorCodes.NotFound, message));
        }

        public static IActionResult CreateInternalServerErrorResult(string message)
        {
            return new ObjectResult(CreateError(ErrorCodes.InternalServerError, message))
            {
                StatusCode = 500
            };
        }

        public static IActionResult CreateStatusCodeResult(HttpStatusCode statusCode, string message)
        {
            return new ObjectResult(CreateError(statusCode.ToString().ToUpper(), message))
            {
                StatusCode = (int)statusCode
            };
        }

        public static dynamic CreateError(string code, string message)
        {
            return new { error = new { code, message } };
        }
    }
}
