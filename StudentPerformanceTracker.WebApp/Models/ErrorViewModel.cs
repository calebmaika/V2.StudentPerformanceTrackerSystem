namespace StudentPerformanceTracker.WebApp.Models;

public class ErrorViewModel
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    
    // Keep this for backwards compatibility
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}