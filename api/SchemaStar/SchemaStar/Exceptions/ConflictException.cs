using System.Net;

namespace SchemaStar.Exceptions
{
    public sealed class ConflictException : AppException
    {
        public ConflictException(string message) 
            : base(message, HttpStatusCode.Conflict)
        {
        }
    }
}
