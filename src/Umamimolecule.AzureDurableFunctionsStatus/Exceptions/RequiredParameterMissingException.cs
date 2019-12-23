namespace Umamimolecule.AzureDurableFunctionsStatus.Exceptions
{
    public class RequiredParameterMissingException : BadRequestException
    {
        public RequiredParameterMissingException(string parameterName)
            : base(string.Format(Resources.ExceptionMessages.RequiredParameterMissingException, parameterName))
        {
            this.ParameterName = parameterName;
        }

        public string ParameterName { get; private set; }
    }
}
