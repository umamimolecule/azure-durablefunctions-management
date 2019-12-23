namespace Umamimolecule.AzureDurableFunctions.Management.Tests
{
    public class ErrorPayload
    {
        public Error Error { get; set; }
    }

    public class Error
    {
        public string Code { get; set; }

        public string Message { get; set; }
    }
}
