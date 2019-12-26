using System.Net;
using Microsoft.AspNetCore.Mvc;
using Umamimolecule.AzureDurableFunctions.Management.Exceptions;

namespace Umamimolecule.AzureDurableFunctions.Management.Utility
{
    /// <summary>
    /// Contains methods to create various <see cref="IActionResult"/> insatnces to be returned from the function endpoints.
    /// </summary>
    public static class ResponseHelper
    {
        /// <summary>
        /// Creates an <see cref="IActionResult"/> instance representing an invalid operation.
        /// </summary>
        /// <param name="message">The message to return in the response body.</param>
        /// <returns>An <see cref="IActionResult"/> instance representing an invalid operation.</returns>
        public static IActionResult CreateInvalidOperationResult(string message)
        {
            return new ObjectResult(CreateError(ErrorCodes.InvalidOperation, message))
            {
                StatusCode = 400,
            };
        }

        /// <summary>
        /// Creates an <see cref="IActionResult"/> instance representing a 404 Not Found result.
        /// </summary>
        /// <param name="message">The message to return in the response body.</param>
        /// <returns>An <see cref="IActionResult"/> instance representing a 404 Not Found result.</returns>
        public static IActionResult CreateNotFoundResult(string message)
        {
            return new NotFoundObjectResult(CreateError(ErrorCodes.NotFound, message));
        }

        /// <summary>
        /// Creates an <see cref="IActionResult"/> instance representing an internal server error.
        /// </summary>
        /// <param name="message">The message to return in the response body.</param>
        /// <returns>An <see cref="IActionResult"/> instance representing an internal server error.</returns>
        public static IActionResult CreateInternalServerErrorResult(string message)
        {
            return new ObjectResult(CreateError(ErrorCodes.InternalServerError, message))
            {
                StatusCode = 500,
            };
        }

        /// <summary>
        /// Creates an <see cref="IActionResult"/> instance for a specified status code.
        /// </summary>
        /// <param name="statusCode">The status code to be returned.</param>
        /// <param name="message">The message to return in the response body.</param>
        /// <returns>An <see cref="IActionResult"/> instance for the specified status code.</returns>
        public static IActionResult CreateStatusCodeResult(HttpStatusCode statusCode, string message)
        {
            return new ObjectResult(CreateError(statusCode.ToString().ToUpper(), message))
            {
                StatusCode = (int)statusCode,
            };
        }

        /// <summary>
        /// Creates a response body representing an error.
        /// </summary>
        /// <param name="code">The error code.</param>
        /// <param name="message">The error message.</param>
        /// <returns>A response body representing an error.</returns>
        public static dynamic CreateError(string code, string message)
        {
            return new { error = new { code, message } };
        }
    }
}
