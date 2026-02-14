using System.Net;

namespace SchemaStar.Exceptions
{
    public sealed class UnsupportedMediaTypeException: AppException
    {
        public UnsupportedMediaTypeException(string message) 
            : base(message, HttpStatusCode.UnsupportedMediaType) 
        { 
        }

        public UnsupportedMediaTypeException(string resourceName, string[] allowedTypes) 
            :base(messageFormat(resourceName, allowedTypes), HttpStatusCode.UnsupportedMediaType)
        { 
        }

        private static string messageFormat(string resourceName, string[] allowedTypes) 
        {
            var typesList = string.Join(", ", allowedTypes);
            return $"The media type provided for {resourceName} is not supported. Supported types are: {typesList}.";
        }
    }
}
