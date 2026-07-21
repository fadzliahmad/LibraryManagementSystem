namespace LibraryManagementSystem.Core.Models;

public class AppLog
{
    public int Id { get; set; }
    public string LogLevel { get; set; } = string.Empty; // Error, Warning, Information, etc.
    public string Message { get; set; } = string.Empty;
    public string? Exception { get; set; }
    public string? StackTrace { get; set; }
    public string? Source { get; set; } // The logger name or middleware name
    public string? UserId { get; set; }
    public string? RequestPath { get; set; }
    public string? RequestMethod { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? AdditionalData { get; set; } // JSON or other serialized data
}
