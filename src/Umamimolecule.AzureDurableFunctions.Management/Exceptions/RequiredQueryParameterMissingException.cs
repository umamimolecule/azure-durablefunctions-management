namespace Umamimolecule.AzureDurableFunctions.Management.Exceptions
{
    /// <summary>
    /// The exception to be thrown when a required parameter query is missing or contains an empty value.
    /// </summary>
    public class RequiredQueryParameterMissingException : BadRequestException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredQueryParameterMissingException"/> class.
        /// </summary>
        /// <param name="parameterName">The parameter name.</param>
        public RequiredQueryParameterMissingException(string parameterName)
            : base(string.Format(Resources.ExceptionMessages.RequiredQueryParameterMissingException, parameterName))
        {
            this.ParameterName = parameterName;
        }

        /// <summary>
        /// Gets the parameter name.
        /// </summary>
        public string ParameterName { get; private set; }
    }
}
