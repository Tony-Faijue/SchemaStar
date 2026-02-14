namespace SchemaStar.Exceptions
{
    public sealed class ContentTooLargeException : AppException
    {
        public ContentTooLargeException(string message)
            : base(message, System.Net.HttpStatusCode.RequestEntityTooLarge)
        { 
        }

        public ContentTooLargeException(string resourceName, long? maxSizeBytes = null) 
            : base(messageFormat(resourceName, maxSizeBytes), System.Net.HttpStatusCode.RequestEntityTooLarge)
        { 
        }

        private static string messageFormat (string resourceName, long? maxSizeBytes)
        {
            var limitInfo = maxSizeBytes.HasValue ? $"(Limit: {maxSizeBytes / 1024 / 1024} MB)" : string.Empty;
            return $"The request content for {resourceName} is too large and cannot be processed. {limitInfo}";
        }
    }
}
