using System.ComponentModel.DataAnnotations;
using LibraryManagementSystem.Core.Models;

namespace LibraryManagementSystem.Core.DTOs;

public class LoanDto
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public int MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public LoanStatus Status { get; set; }
}

public class CreateLoanDto
{
    [Required]
    public int BookId { get; set; }

    [Required]
    public int MemberId { get; set; }

    // Optional override; defaults to 14 days from loan date if not provided
    public DateTime? DueDate { get; set; }
}
