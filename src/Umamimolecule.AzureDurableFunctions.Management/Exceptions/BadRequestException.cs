using System;
using System.Net;

namespace Umamimolecule.AzureDurableFunctions.Management.Exceptions
{
    /// <summary>
    /// Represents an exception which will return a 400 Bad Request response.
    /// </summary>
    public class BadRequestException : StatusCodeException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BadRequestException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The inner exception.</param>
        public BadRequestException(string message, Exception innerException = null)
            : base(HttpStatusCode.BadRequest, message, innerException)
        {
        }
    }
}
