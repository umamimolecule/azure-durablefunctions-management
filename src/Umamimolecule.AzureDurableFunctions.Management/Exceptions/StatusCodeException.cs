using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Umamimolecule.AzureDurableFunctions.Management.Utility;

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

        public IActionResult ToObjectResult()
        {
            return ResponseHelper.CreateStatusCodeResult(this.StatusCode, this.Message);
        }
    }
}
