namespace Umamimolecule.AzureDurableFunctions.Management.Exceptions
{
    /// <summary>
    /// The exception to be thrown when a required path parameter is missing or contains an empty value.
    /// </summary>
    public class RequiredParameterMissingException : BadRequestException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredParameterMissingException"/> class.
        /// </summary>
        /// <param name="parameterName">The parameter name.</param>
        public RequiredParameterMissingException(string parameterName)
            : base(string.Format(Resources.ExceptionMessages.RequiredParameterMissingException, parameterName))
        {
            this.ParameterName = parameterName;
        }

        /// <summary>
        /// Gets the parameter name.
        /// </summary>
        public string ParameterName { get; private set; }
    }
}
