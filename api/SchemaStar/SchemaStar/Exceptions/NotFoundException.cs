namespace SchemaStar.Exceptions
{
    public sealed class NotFoundException : AppException
    {
        public NotFoundException(string resourceName, object key) 
            : base($"{resourceName} with identifier '{key}' was not found.", System.Net.HttpStatusCode.NotFound)
        { 
        }
    }
}
