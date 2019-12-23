namespace Umamimolecule.AzureDurableFunctionsStatus.Exceptions
{
    public class RequiredQueryParameterMissingException : BadRequestException
    {
        public RequiredQueryParameterMissingException(string parameterName)
            : base(string.Format(Resources.ExceptionMessages.RequiredQueryParameterMissingException, parameterName))
        {
            this.ParameterName = parameterName;
        }

        public string ParameterName { get; private set; }
    }
}
