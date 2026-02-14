using System.Net;

namespace SchemaStar.Exceptions
{
    public sealed class ForbiddenException : AppException
    {
        public ForbiddenException(string message)
            :base(message, HttpStatusCode.Forbidden)
        {
        }

        public ForbiddenException(string resourceName, object key)
            : base($"{resourceName} with identifier '{key}': Access Denied. Insufficient permissions.", HttpStatusCode.Forbidden)
        {
        }
    }
}
