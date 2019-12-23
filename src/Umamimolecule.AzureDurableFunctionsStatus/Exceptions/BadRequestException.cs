using System;
using System.Net;

namespace Umamimolecule.AzureDurableFunctions.Management.Exceptions
{
    public class BadRequestException : StatusCodeException
    {
        public BadRequestException(string message, Exception innerException = null)
            : base(HttpStatusCode.BadRequest, message, innerException)
        {
        }
    }
}
