using System.Net;
using System.Text.Json;

namespace StudentPerformanceTracker.WebApp.Middleware
{
    /// <summary>
    /// Global exception handling middleware
    /// Catches all unhandled exceptions and returns appropriate responses
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Log the exception with full details
            _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

            // Set response status code
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            // Create error response
            var errorResponse = new ErrorResponse
            {
                StatusCode = context.Response.StatusCode,
                Message = _environment.IsDevelopment() 
                    ? exception.Message 
                    : "An error occurred while processing your request.",
                Details = _environment.IsDevelopment() 
                    ? exception.StackTrace 
                    : null
            };

            // If it's an AJAX request, return JSON
            if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var json = JsonSerializer.Serialize(errorResponse);
                await context.Response.WriteAsync(json);
            }
            else
            {
                // For regular requests, redirect to error page
                context.Response.Redirect($"/Error?statusCode={errorResponse.StatusCode}");
            }
        }
    }

    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Details { get; set; }
    }
}