using Microsoft.Extensions.Logging;
using StudentPerformanceTracker.Data.Context;
using StudentPerformanceTracker.Data.Entities.Auditing;

namespace StudentPerformanceTracker.Services.Auditing
{
    public interface IAuditService
    {
        Task LogActionAsync(string action, string entityType, int entityId, string username, string? details = null);
    }

    public class AuditService : IAuditService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuditService> _logger;

        public AuditService(ApplicationDbContext context, ILogger<AuditService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task LogActionAsync(
            string action, 
            string entityType, 
            int entityId, 
            string username, 
            string? details = null)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    Action = action,
                    EntityType = entityType,
                    EntityId = entityId,
                    Username = username,
                    Details = details,
                    Timestamp = DateTime.UtcNow
                };

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "Audit: {Action} on {EntityType} ({EntityId}) by {Username}", 
                    action, entityType, entityId, username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log audit action");
                // Don't throw - auditing failure shouldn't break the main operation
            }
        }
    }
}