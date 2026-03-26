using System.Net;

namespace SchemaStar.Exceptions
{
    public sealed class ValidationException : AppException
    {
        //Production
        public ValidationException(string message) 
            :base(message, HttpStatusCode.BadRequest)
        {
        }
        //Internal
        public ValidationException(string resourceName, object errors)
            :base($"{resourceName} with validations errors: '{errors}'", HttpStatusCode.BadRequest)
        {
        }
    }
}
