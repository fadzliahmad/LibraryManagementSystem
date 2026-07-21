using LibraryManagementSystem.Core.Models;

namespace LibraryManagementSystem.Core.Interfaces;

public interface IBookRepository
{
    Task<IEnumerable<Book>> GetAllAsync();
    Task<Book?> GetByIdAsync(int id);
    Task<Book> AddAsync(Book book);
    Task UpdateAsync(Book book);
    Task DeleteAsync(Book book);
    Task<bool> HasActiveLoansAsync(int bookId);
}

public interface IMemberRepository
{
    Task<IEnumerable<Member>> GetAllAsync();
    Task<Member?> GetByIdAsync(int id);
    Task<Member> AddAsync(Member member);
    Task UpdateAsync(Member member);
    Task DeleteAsync(Member member);
    Task<bool> HasActiveLoansAsync(int memberId);
}

public interface ILoanRepository
{
    Task<IEnumerable<Loan>> GetAllAsync();
    Task<Loan?> GetByIdAsync(int id);
    Task<Loan> AddAsync(Loan loan);
    Task UpdateAsync(Loan loan);
    Task DeleteAsync(Loan loan);
}
