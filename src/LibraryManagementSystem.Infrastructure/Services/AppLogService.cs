using LibraryManagementSystem.Core.Interfaces;
using LibraryManagementSystem.Core.Models;
using LibraryManagementSystem.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace LibraryManagementSystem.Infrastructure.Services;

public class AppLogService : IAppLogService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<AppLogService> _logger;

    public AppLogService(AppDbContext dbContext, ILogger<AppLogService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public void LogError(string message, Exception? exception = null, string? source = null, string? userId = null, string? requestPath = null, string? requestMethod = null)
    {
        try
        {
            var appLog = new AppLog
            {
                LogLevel = "Error",
                Message = message,
                Exception = exception?.Message,
                StackTrace = exception?.StackTrace,
                Source = source,
                UserId = userId,
                RequestPath = requestPath,
                RequestMethod = requestMethod,
                Timestamp = DateTime.UtcNow
            };

            _dbContext.AppLogs.Add(appLog);
            _dbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while logging an error to the database. Original error: {message}", message);
        }
    }

    public void LogWarning(string message, string? source = null, string? userId = null)
    {
        try
        {
            var appLog = new AppLog
            {
                LogLevel = "Warning",
                Message = message,
                Source = source,
                UserId = userId,
                Timestamp = DateTime.UtcNow
            };

            _dbContext.AppLogs.Add(appLog);
            _dbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while logging a warning to the database. Original message: {message}", message);
        }
    }

    public void LogInformation(string message, string? source = null, string? userId = null)
    {
        try
        {
            var appLog = new AppLog
            {
                LogLevel = "Information",
                Message = message,
                Source = source,
                UserId = userId,
                Timestamp = DateTime.UtcNow
            };

            _dbContext.AppLogs.Add(appLog);
            _dbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while logging information to the database. Original message: {message}", message);
        }
    }

    public void LogDebug(string message, string? source = null)
    {
        try
        {
            var appLog = new AppLog
            {
                LogLevel = "Debug",
                Message = message,
                Source = source,
                Timestamp = DateTime.UtcNow
            };

            _dbContext.AppLogs.Add(appLog);
            _dbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while logging debug information to the database. Original message: {message}", message);
        }
    }

    public IEnumerable<AppLog> GetLogs(int? limit = null, string? logLevel = null)
    {
        try
        {
            var query = _dbContext.AppLogs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(logLevel))
            {
                query = query.Where(l => l.LogLevel == logLevel);
            }

            query = query.OrderByDescending(l => l.Timestamp);

            if (limit.HasValue && limit > 0)
            {
                query = query.Take(limit.Value);
            }

            return query.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving logs from the database");
            return Enumerable.Empty<AppLog>();
        }
    }

    public IEnumerable<AppLog> GetLogsByDateRange(DateTime startDate, DateTime endDate)
    {
        try
        {
            return _dbContext.AppLogs
                .Where(l => l.Timestamp >= startDate && l.Timestamp <= endDate)
                .OrderByDescending(l => l.Timestamp)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving logs by date range");
            return Enumerable.Empty<AppLog>();
        }
    }

    public IEnumerable<AppLog> GetRecentErrors(int count = 50)
    {
        try
        {
            return _dbContext.AppLogs
                .Where(l => l.LogLevel == "Error")
                .OrderByDescending(l => l.Timestamp)
                .Take(count)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving recent errors");
            return Enumerable.Empty<AppLog>();
        }
    }

    public void ClearOldLogs(int retentionDays)
    {
        try
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);
            var logsToDelete = _dbContext.AppLogs.Where(l => l.Timestamp < cutoffDate);

            _dbContext.AppLogs.RemoveRange(logsToDelete);
            _dbContext.SaveChanges();

            _logger.LogInformation("Cleared old logs older than {cutoffDate}", cutoffDate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while clearing old logs");
        }
    }
}
