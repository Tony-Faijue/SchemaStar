using System.Net;

namespace SchemaStar.Exceptions
{
    public sealed class ConflictException : AppException
    {
        //Production Safe Message
        public ConflictException(string message) 
            : base(message, HttpStatusCode.Conflict)
        {
        }

        //For Internal Tracking
        public ConflictException(string resourceName, object key)
            : base($"{resourceName} with identifier '{key}' was not found.", HttpStatusCode.Conflict)
        {
        }
    }
}
