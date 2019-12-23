using System;

namespace Umamimolecule.AzureDurableFunctionsStatus.Exceptions
{
    public class InvalidParameterException : BadRequestException
    {
        public InvalidParameterException(string message, string parameterName, string parameterValue, Exception innerException = null)
            : base(message, innerException)
        {
            this.ParameterName = parameterName;
            this.ParameterValue = parameterValue;
        }

        public InvalidParameterException(string parameterName, string parameterValue, Exception innerException = null)
            : base(string.Format(Resources.ExceptionMessages.InvalidParameterException, parameterValue, parameterName), innerException)
        {
            this.ParameterName = parameterName;
            this.ParameterValue = parameterValue;
        }

        public string ParameterName { get; private set; }

        public string ParameterValue { get; private set; }
    }
}
