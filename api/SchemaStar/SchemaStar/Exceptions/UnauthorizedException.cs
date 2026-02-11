using System.Net;

namespace SchemaStar.Exceptions
{
    public sealed class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message)
            : base(message, HttpStatusCode.Unauthorized)
        {
        }
    }
}
