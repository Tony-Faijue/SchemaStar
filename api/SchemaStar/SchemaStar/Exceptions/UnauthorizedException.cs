using System.Net;

namespace SchemaStar.Exceptions
{
    public sealed class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message)
            : base(message, HttpStatusCode.Unauthorized)
        {
        }

        public UnauthorizedException(string resourceName, object key)
            : base($"Authentication failed for {resourceName} with identifier '{key}'. Please provide valid credentials.", HttpStatusCode.Unauthorized)
        {
        }
    }
}
