namespace StudentPerformanceTracker.WebApp.Exceptions
{
    /// <summary>
    /// Base exception for application-specific errors
    /// </summary>
    public class AppException : Exception
    {
        public AppException(string message) : base(message) { }
        public AppException(string message, Exception innerException) 
            : base(message, innerException) { }
    }

    /// <summary>
    /// Thrown when a requested resource is not found
    /// </summary>
    public class NotFoundException : AppException
    {
        public NotFoundException(string message) : base(message) { }
    }

    /// <summary>
    /// Thrown when business rule validation fails
    /// </summary>
    public class BusinessRuleException : AppException
    {
        public BusinessRuleException(string message) : base(message) { }
    }

    /// <summary>
    /// Thrown when a duplicate record is detected
    /// </summary>
    public class DuplicateException : AppException
    {
        public DuplicateException(string message) : base(message) { }
    }

    /// <summary>
    /// Thrown when an operation is not authorized
    /// </summary>
    public class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message) : base(message) { }
    }
}