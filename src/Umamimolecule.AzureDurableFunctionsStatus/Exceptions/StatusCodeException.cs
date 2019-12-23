using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Umamimolecule.AzureDurableFunctions.Management.Exceptions
{
    public class StatusCodeException : Exception
    {
        public StatusCodeException(HttpStatusCode statusCode, string message, Exception innerException = null)
            : base(message, innerException)
        {
            this.StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; private set; }

        public ObjectResult ToObjectResult()
        {
            dynamic value = new
            {
                error = new
                {
                    code = this.StatusCode.ToString().ToUpper(),
                    message = this.Message
                }
            };

            return new ObjectResult(value)
            {
                StatusCode = (int)this.StatusCode
            };
        }
    }
}
