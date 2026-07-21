using LibraryManagementSystem.Core.Models;

namespace LibraryManagementSystem.Core.Interfaces;

public interface IAppLogService
{
    /// <summary>
    /// Logs an error with optional exception details
    /// </summary>
    void LogError(string message, Exception? exception = null, string? source = null, string? userId = null, string? requestPath = null, string? requestMethod = null);

    /// <summary>
    /// Logs a warning
    /// </summary>
    void LogWarning(string message, string? source = null, string? userId = null);

    /// <summary>
    /// Logs information
    /// </summary>
    void LogInformation(string message, string? source = null, string? userId = null);

    /// <summary>
    /// Logs a debug message
    /// </summary>
    void LogDebug(string message, string? source = null);

    /// <summary>
    /// Retrieves all logs with optional filtering
    /// </summary>
    IEnumerable<AppLog> GetLogs(int? limit = null, string? logLevel = null);

    /// <summary>
    /// Retrieves logs for a specific date range
    /// </summary>
    IEnumerable<AppLog> GetLogsByDateRange(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Retrieves recent error logs
    /// </summary>
    IEnumerable<AppLog> GetRecentErrors(int count = 50);

    /// <summary>
    /// Clears old logs based on retention days
    /// </summary>
    void ClearOldLogs(int retentionDays);
}
