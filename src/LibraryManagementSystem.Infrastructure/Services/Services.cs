using LibraryManagementSystem.Core.DTOs;
using LibraryManagementSystem.Core.Interfaces;
using LibraryManagementSystem.Core.Models;

namespace LibraryManagementSystem.Infrastructure.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _repo;
    public BookService(IBookRepository repo) => _repo = repo;

    public async Task<IEnumerable<BookDto>> GetAllAsync()
    {
        var books = await _repo.GetAllAsync();
        return books.Select(ToDto);
    }

    public async Task<BookDto?> GetByIdAsync(int id)
    {
        var book = await _repo.GetByIdAsync(id);
        return book is null ? null : ToDto(book);
    }

    public async Task<BookDto> CreateAsync(CreateBookDto dto)
    {
        var book = new Book
        {
            Title = dto.Title,
            Author = dto.Author,
            Isbn = dto.Isbn,
            Genre = dto.Genre,
            PublishedYear = dto.PublishedYear,
            TotalCopies = dto.TotalCopies,
            AvailableCopies = dto.TotalCopies 
        };
        var created = await _repo.AddAsync(book);
        return ToDto(created);
    }

    public async Task<bool> UpdateAsync(int id, UpdateBookDto dto)
    {
        var book = await _repo.GetByIdAsync(id);
        if (book is null) return false;

        var onLoan = book.TotalCopies - book.AvailableCopies;

        book.Title = dto.Title;
        book.Author = dto.Author;
        book.Isbn = dto.Isbn;
        book.Genre = dto.Genre;
        book.PublishedYear = dto.PublishedYear;
        book.TotalCopies = dto.TotalCopies;
        book.AvailableCopies = Math.Max(0, dto.TotalCopies - onLoan);

        await _repo.UpdateAsync(book);
        return true;
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var book = await _repo.GetByIdAsync(id);
        if (book is null) return (false, "Book not found.");

        if (await _repo.HasActiveLoansAsync(id))
            return (false, "Cannot delete a book that has active loans.");

        await _repo.DeleteAsync(book);
        return (true, null);
    }

    private static BookDto ToDto(Book b) => new()
    {
        Id = b.Id,
        Title = b.Title,
        Author = b.Author,
        Isbn = b.Isbn,
        Genre = b.Genre,
        PublishedYear = b.PublishedYear,
        TotalCopies = b.TotalCopies,
        AvailableCopies = b.AvailableCopies
    };
}

public class MemberService : IMemberService
{
    private readonly IMemberRepository _repo;
    public MemberService(IMemberRepository repo) => _repo = repo;

    public async Task<IEnumerable<MemberDto>> GetAllAsync()
    {
        var members = await _repo.GetAllAsync();
        return members.Select(ToDto);
    }

    public async Task<MemberDto?> GetByIdAsync(int id)
    {
        var member = await _repo.GetByIdAsync(id);
        return member is null ? null : ToDto(member);
    }

    public async Task<MemberDto> CreateAsync(CreateMemberDto dto)
    {
        var member = new Member
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            MembershipDate = DateTime.UtcNow,
            IsActive = true
        };
        var created = await _repo.AddAsync(member);
        return ToDto(created);
    }

    public async Task<bool> UpdateAsync(int id, UpdateMemberDto dto)
    {
        var member = await _repo.GetByIdAsync(id);
        if (member is null) return false;

        member.FullName = dto.FullName;
        member.Email = dto.Email;
        member.PhoneNumber = dto.PhoneNumber;
        member.IsActive = dto.IsActive;

        await _repo.UpdateAsync(member);
        return true;
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var member = await _repo.GetByIdAsync(id);
        if (member is null) return (false, "Member not found.");

        if (await _repo.HasActiveLoansAsync(id))
            return (false, "Cannot delete a member that has active loans.");

        await _repo.DeleteAsync(member);
        return (true, null);
    }

    private static MemberDto ToDto(Member m) => new()
    {
        Id = m.Id,
        FullName = m.FullName,
        Email = m.Email,
        PhoneNumber = m.PhoneNumber,
        MembershipDate = m.MembershipDate,
        IsActive = m.IsActive
    };
}

public class LoanService : ILoanService
{
    private readonly ILoanRepository _loanRepo;
    private readonly IBookRepository _bookRepo;
    private readonly IMemberRepository _memberRepo;
    private const int DefaultLoanPeriodDays = 14;

    public LoanService(ILoanRepository loanRepo, IBookRepository bookRepo, IMemberRepository memberRepo)
    {
        _loanRepo = loanRepo;
        _bookRepo = bookRepo;
        _memberRepo = memberRepo;
    }

    public async Task<IEnumerable<LoanDto>> GetAllAsync()
    {
        var loans = await _loanRepo.GetAllAsync();
        return loans.Select(ToDto);
    }

    public async Task<LoanDto?> GetByIdAsync(int id)
    {
        var loan = await _loanRepo.GetByIdAsync(id);
        return loan is null ? null : ToDto(loan);
    }

    public async Task<(LoanDto? Loan, string? Error)> CreateAsync(CreateLoanDto dto)
    {
        var book = await _bookRepo.GetByIdAsync(dto.BookId);
        if (book is null) return (null, "Book not found.");

        var member = await _memberRepo.GetByIdAsync(dto.MemberId);
        if (member is null) return (null, "Member not found.");

        if (!member.IsActive) return (null, "Member is not active.");

        if (book.AvailableCopies <= 0) return (null, "No available copies for this book.");

        var loan = new Loan
        {
            BookId = dto.BookId,
            MemberId = dto.MemberId,
            LoanDate = DateTime.UtcNow,
            DueDate = dto.DueDate ?? DateTime.UtcNow.AddDays(DefaultLoanPeriodDays),
            Status = LoanStatus.Borrowed
        };

        book.AvailableCopies -= 1;
        await _bookRepo.UpdateAsync(book);

        var created = await _loanRepo.AddAsync(loan);
        created.Book = book;
        created.Member = member;

        return (ToDto(created), null);
    }

    public async Task<(bool Success, string? Error)> ReturnAsync(int id)
    {
        var loan = await _loanRepo.GetByIdAsync(id);
        if (loan is null) return (false, "Loan not found.");

        if (loan.Status == LoanStatus.Returned)
            return (false, "This loan has already been returned.");

        loan.Status = LoanStatus.Returned;
        loan.ReturnDate = DateTime.UtcNow;
        await _loanRepo.UpdateAsync(loan);

        var book = await _bookRepo.GetByIdAsync(loan.BookId);
        if (book is not null)
        {
            book.AvailableCopies = Math.Min(book.TotalCopies, book.AvailableCopies + 1);
            await _bookRepo.UpdateAsync(book);
        }

        return (true, null);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var loan = await _loanRepo.GetByIdAsync(id);
        if (loan is null) return false;

        await _loanRepo.DeleteAsync(loan);
        return true;
    }

    private static LoanDto ToDto(Loan l) => new()
    {
        Id = l.Id,
        BookId = l.BookId,
        BookTitle = l.Book?.Title ?? string.Empty,
        MemberId = l.MemberId,
        MemberName = l.Member?.FullName ?? string.Empty,
        LoanDate = l.LoanDate,
        DueDate = l.DueDate,
        ReturnDate = l.ReturnDate,
        Status = l.Status
    };
}
