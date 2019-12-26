namespace Umamimolecule.AzureDurableFunctions.Management.Exceptions
{
    /// <summary>
    /// Contains the error codes returned in the response body.
    /// </summary>
    public static class ErrorCodes
    {
        /// <summary>
        /// The code for when an instance cannot be found.
        /// </summary>
        public const string NotFound = "NOTFOUND";

        /// <summary>
        /// The code for when an invalid operation was attempted on an instance.
        /// </summary>
        public const string InvalidOperation = "INVALIDOPERATION";

        /// <summary>
        /// The code for when an unexpected error occurred.
        /// </summary>
        public const string InternalServerError = "INTERNALSERVERERROR";
    }
}
