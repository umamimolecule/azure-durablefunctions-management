using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Umamimolecule.AzureDurableFunctions.Management.Utility;

namespace Umamimolecule.AzureDurableFunctions.Management.Exceptions
{
    /// <summary>
    /// Represents an exception which will return a particular status code response.
    /// </summary>
    public class StatusCodeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusCodeException"/> class.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The inner exception.</param>
        public StatusCodeException(HttpStatusCode statusCode, string message, Exception innerException = null)
            : base(message, innerException)
        {
            this.StatusCode = statusCode;
        }

        /// <summary>
        /// Gets the status code.
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }

        /// <summary>
        /// Creates a <see cref="IActionResult"/> response using the status code and error message.
        /// </summary>
        /// <returns>The <see cref="IActionResult"/> response.</returns>
        public IActionResult ToObjectResult()
        {
            return ResponseHelper.CreateStatusCodeResult(this.StatusCode, this.Message);
        }
    }
}
