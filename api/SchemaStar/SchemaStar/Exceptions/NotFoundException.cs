using System.Net;

namespace SchemaStar.Exceptions
{
    public sealed class NotFoundException : AppException
    {
        //Production Safe Message
        public NotFoundException(string message)
            : base(message, HttpStatusCode.NotFound)
        {
        }
        //For Internal Tracking
        public NotFoundException(string resourceName, object key) 
            : base($"{resourceName} with identifier '{key}' was not found.", System.Net.HttpStatusCode.NotFound)
        { 
        }
    }
}
