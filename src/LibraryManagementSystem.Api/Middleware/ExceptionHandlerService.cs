using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Api.Middleware
{
    /// <summary>
    /// Service to handle specific database exceptions gracefully
    /// </summary>
    public class ExceptionHandlerService
    {
        public class ExceptionResponse
        {
            public bool Success { get; set; } = false;
            public string Message { get; set; }
            public string ErrorCode { get; set; }
            public object Details { get; set; }
        }

        /// <summary>
        /// Handles DbUpdateException and returns a user-friendly response
        /// </summary>
        public static ExceptionResponse HandleException(Exception ex)
        {
            // Handle DbUpdateException (EF Core)
            if (ex is DbUpdateException dbUpdateEx)
            {
                var innerException = dbUpdateEx.InnerException;

                // Check for SQL foreign key constraint violation
                if (innerException is SqlException sqlEx && sqlEx.Number == 547) // Constraint violation error code
                {
                    // Check if it's a Loans/Books constraint
                    if (sqlEx.Message.Contains("FK_Loans_Books_BookId", StringComparison.OrdinalIgnoreCase))
                    {
                        return new ExceptionResponse
                        {
                            Success = false,
                            ErrorCode = "BOOK_HAS_ACTIVE_LOANS",
                            Message = "Cannot delete this book. It has active or inactive loans associated with it.",
                            Details = new
                            {
                                Reason = "The book cannot be deleted while loans reference it.",
                                Suggestion = "Please return all loans for this book before attempting to delete it."
                            }
                        };
                    }

                    // Generic foreign key constraint error
                    return new ExceptionResponse
                    {
                        Success = false,
                        ErrorCode = "CONSTRAINT_VIOLATION",
                        Message = "Cannot delete this record due to existing related data.",
                        Details = new { Reason = "Other records reference this item." }
                    };
                }

                // Generic DbUpdateException
                return new ExceptionResponse
                {
                    Success = false,
                    ErrorCode = "DATABASE_UPDATE_ERROR",
                    Message = "An error occurred while updating the database. Please try again.",
                    Details = new { InnerError = innerException?.Message }
                };
            }

            // Handle other exceptions
            return new ExceptionResponse
            {
                Success = false,
                ErrorCode = "INTERNAL_SERVER_ERROR",
                Message = "An unexpected error occurred. Please try again later.",
                Details = new { Exception = ex.GetType().Name }
            };
        }
    }
}
