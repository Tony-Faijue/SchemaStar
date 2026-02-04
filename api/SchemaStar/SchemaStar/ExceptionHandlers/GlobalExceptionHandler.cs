using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SchemaStar.Exceptions;

namespace SchemaStar.ExceptionHandlers
{
    public sealed class GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IProblemDetailsService problemDetailsService) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancelaltionToken)
        {
            logger.LogError(exception, "Unhandled exception occured. TraceId: {TraceId}",
                httpContext.TraceIdentifier);

            var (statusCode, title) = MapException(exception);

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/problem+json";

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Type = GetProblemType(statusCode),
                Instance = httpContext.Request.Path,
                Detail = GetSafeErrorMessage(exception, httpContext)
            };

            problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
            problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails,
            });
        }

        // Map the exceptions to HTTP responses
        private static (int StatusCode, string Title) MapException(Exception exception) => exception switch
        {
            ArgumentNullException => (StatusCodes.Status400BadRequest, "Invalid argument provided"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Invalid argument provided"),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),

            NotFoundException => (StatusCodes.Status404NotFound, "Resource Not Found"),
            ConflictException => (StatusCodes.Status409Conflict, "Resource Already Exist"),

            AppException appException => ((int)appException.StatusCode, "Application Error"),

            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };


        /// <summary>
        /// Function to map the HTTP Status codes to the Problem Type URL
        /// </summary>
        /// <returns> string url to the problem type url</returns>
        private static string GetProblemType(int statusCode) => statusCode switch
        {
            400 => "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            401 => "https://tools.ietf.org/html/rfc9110#section-15.5.2",
            403 => "https://tools.ietf.org/html/rfc9110#section-15.5.4",
            404 => "https://tools.ietf.org/html/rfc9110#section-15.5.5",
            409 => "https://tools.ietf.org/html/rfc9110#section-15.5.10",
            413 => "https://tools.ietf.org/html/rfc9110#section-15.5.14",
            415 => "https://tools.ietf.org/html/rfc9110#section-15.5.16",
            _ => "https://tools.ietf.org/html/rfc9110#section-15.6.1"
        };

        /// <summary>
        /// Function to get safe error message in production and explicit error message in development
        /// </summary>
        /// <returns> string error message</returns>
        private static string? GetSafeErrorMessage(Exception exception, HttpContext context) 
        {
            //only expose details in development
            var env = context.RequestServices.GetRequiredService<IHostEnvironment>();
            if (env.IsDevelopment())
            {
                return exception.Message;
            }

            //In production, only expose message from custom safe exceptions
            return exception is AppException ? exception.Message : null;
        }

    }
}
