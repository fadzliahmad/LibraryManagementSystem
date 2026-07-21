using LibraryManagementSystem.Core.DTOs;

namespace LibraryManagementSystem.Core.Interfaces;

public interface IBookService
{
    Task<IEnumerable<BookDto>> GetAllAsync();
    Task<BookDto?> GetByIdAsync(int id);
    Task<BookDto> CreateAsync(CreateBookDto dto);
    Task<bool> UpdateAsync(int id, UpdateBookDto dto);
    Task<(bool Success, string? Error)> DeleteAsync(int id);
}

public interface IMemberService
{
    Task<IEnumerable<MemberDto>> GetAllAsync();
    Task<MemberDto?> GetByIdAsync(int id);
    Task<MemberDto> CreateAsync(CreateMemberDto dto);
    Task<bool> UpdateAsync(int id, UpdateMemberDto dto);
    Task<(bool Success, string? Error)> DeleteAsync(int id);
}

public interface ILoanService
{
    Task<IEnumerable<LoanDto>> GetAllAsync();
    Task<LoanDto?> GetByIdAsync(int id);
    Task<(LoanDto? Loan, string? Error)> CreateAsync(CreateLoanDto dto);
    Task<(bool Success, string? Error)> ReturnAsync(int id);
    Task<bool> DeleteAsync(int id);
}
