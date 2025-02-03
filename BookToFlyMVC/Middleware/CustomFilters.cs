namespace BookToFlyMVC.Middleware
{
    public class CustomExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomExceptionHandlingMiddleware> _logger;

        public CustomExceptionHandlingMiddleware(RequestDelegate next, ILogger<CustomExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message, "An error occurred.");
                httpContext.Response.Redirect("/Home/Error");
            }
        }

    }


}