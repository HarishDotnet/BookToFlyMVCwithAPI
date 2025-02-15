using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using BookToFlyMVC.Exceptions;

namespace BookToFlyMVC.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            // Log the exception
            _logger.LogError(context.Exception, "Unhandled exception occurred");

            // Set the error message and code
            var statusCode = 500; // Default error code
            var exception = context.Exception;
            var additionalDetails = string.Empty;

            if (exception is NotFoundException) // Example: Handle specific exceptions
            {
                statusCode = 404;
                additionalDetails = "The requested resource could not be found.";
            }
            else if (exception is UnauthorizedAccessException) // Handle Unauthorized
            {
                statusCode = 403;
                additionalDetails = "You do not have permission to access this resource.";
            }
            else
            {
                additionalDetails = exception.StackTrace; // For generic errors, include stack trace (or a custom message)
            }

            // Pass error details to ViewData
            context.Result = new RedirectToActionResult("Error", "Home", new { errorMessage = exception.Message, errorCode = statusCode, additionalDetails });

            // Mark exception as handled
            context.ExceptionHandled = true;
        }
    }
}
