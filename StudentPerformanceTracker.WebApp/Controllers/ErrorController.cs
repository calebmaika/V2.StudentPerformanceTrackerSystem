using Microsoft.AspNetCore.Mvc;

namespace StudentPerformanceTracker.WebApp.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error")]
        [Route("Error/{statusCode}")]
        public IActionResult Index(int? statusCode = null)
        {
            var errorViewModel = new ErrorViewModel
            {
                StatusCode = statusCode ?? 500,
                Message = GetErrorMessage(statusCode ?? 500)
            };

            return View("~/Views/Shared/Error.cshtml", errorViewModel);
        }

        private string GetErrorMessage(int statusCode)
        {
            return statusCode switch
            {
                404 => "The page you're looking for could not be found.",
                403 => "You don't have permission to access this resource.",
                401 => "You must be logged in to access this resource.",
                500 => "An internal server error occurred. Please try again later.",
                _ => "An error occurred while processing your request."
            };
        }
    }

    public class ErrorViewModel
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Details { get; set; }
    }
}