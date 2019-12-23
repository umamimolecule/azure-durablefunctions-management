using System;
using System.Net;

namespace Umamimolecule.AzureDurableFunctionsStatus.Exceptions
{
    public class BadRequestException : StatusCodeException
    {
        public BadRequestException(string message, Exception innerException = null)
            : base(HttpStatusCode.BadRequest, message, innerException)
        {
        }
    }
}
