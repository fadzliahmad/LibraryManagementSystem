using LibraryManagementSystem.Core.Interfaces;
using LibraryManagementSystem.Core.Models;
using LibraryManagementSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Infrastructure.Repositories;

public class BookRepository : IBookRepository
{
    private readonly AppDbContext _context;
    public BookRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Book>> GetAllAsync() =>
        await _context.Books.AsNoTracking().ToListAsync();

    public async Task<Book?> GetByIdAsync(int id) =>
        await _context.Books.FindAsync(id);

    public async Task<Book> AddAsync(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return book;
    }

    public async Task UpdateAsync(Book book)
    {
        _context.Books.Update(book);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Book book)
    {
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasActiveLoansAsync(int bookId) =>
        await _context.Loans.AnyAsync(l => l.BookId == bookId && l.Status == LoanStatus.Borrowed);
}

public class MemberRepository : IMemberRepository
{
    private readonly AppDbContext _context;
    public MemberRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Member>> GetAllAsync() =>
        await _context.Members.AsNoTracking().ToListAsync();

    public async Task<Member?> GetByIdAsync(int id) =>
        await _context.Members.FindAsync(id);

    public async Task<Member> AddAsync(Member member)
    {
        _context.Members.Add(member);
        await _context.SaveChangesAsync();
        return member;
    }

    public async Task UpdateAsync(Member member)
    {
        _context.Members.Update(member);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Member member)
    {
        _context.Members.Remove(member);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasActiveLoansAsync(int memberId) =>
        await _context.Loans.AnyAsync(l => l.MemberId == memberId && l.Status == LoanStatus.Borrowed);
}

public class LoanRepository : ILoanRepository
{
    private readonly AppDbContext _context;
    public LoanRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Loan>> GetAllAsync() =>
        await _context.Loans
            .Include(l => l.Book)
            .Include(l => l.Member)
            .AsNoTracking()
            .ToListAsync();

    public async Task<Loan?> GetByIdAsync(int id) =>
        await _context.Loans
            .Include(l => l.Book)
            .Include(l => l.Member)
            .FirstOrDefaultAsync(l => l.Id == id);

    public async Task<Loan> AddAsync(Loan loan)
    {
        _context.Loans.Add(loan);
        await _context.SaveChangesAsync();
        return loan;
    }

    public async Task UpdateAsync(Loan loan)
    {
        _context.Loans.Update(loan);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Loan loan)
    {
        _context.Loans.Remove(loan);
        await _context.SaveChangesAsync();
    }
}
