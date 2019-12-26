using System;

namespace Umamimolecule.AzureDurableFunctions.Management.Exceptions
{
    /// <summary>
    /// The exception to be thrown when a parameter contains an invalid value.
    /// </summary>
    public class InvalidParameterException : BadRequestException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidParameterException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="parameterValue">The valid of the parameter.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidParameterException(string message, string parameterName, string parameterValue, Exception innerException = null)
            : base(message, innerException)
        {
            this.ParameterName = parameterName;
            this.ParameterValue = parameterValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidParameterException"/> class.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="parameterValue">The valid of the parameter.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidParameterException(string parameterName, string parameterValue, Exception innerException = null)
            : base(string.Format(Resources.ExceptionMessages.InvalidParameterException, parameterValue, parameterName), innerException)
        {
            this.ParameterName = parameterName;
            this.ParameterValue = parameterValue;
        }

        /// <summary>
        /// Gets the parameter name.
        /// </summary>
        public string ParameterName { get; private set; }

        /// <summary>
        /// Gets the parameter value.
        /// </summary>
        public string ParameterValue { get; private set; }
    }
}
