using LibraryManagementSystem.Core.Models;

namespace LibraryManagementSystem.Infrastructure.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext context)
    {
        if (context.Books.Any()) return; // already seeded

        var books = new List<Book>
        {
            new() { Title = "Clean Code", Author = "Robert C. Martin", Isbn = "9780132350884", Genre = "Software Engineering", PublishedYear = 2008, TotalCopies = 3, AvailableCopies = 3 },
            new() { Title = "The Pragmatic Programmer", Author = "Andrew Hunt", Isbn = "9780135957059", Genre = "Software Engineering", PublishedYear = 2019, TotalCopies = 2, AvailableCopies = 2 },
            new() { Title = "Designing Data-Intensive Applications", Author = "Martin Kleppmann", Isbn = "9781449373320", Genre = "Systems Design", PublishedYear = 2017, TotalCopies = 2, AvailableCopies = 2 }
        };

        var members = new List<Member>
        {
            new() { FullName = "Ahmad Fadzli", Email = "ahmad@gmail.com", PhoneNumber = "01111111111", MembershipDate = DateTime.UtcNow, IsActive = true },
            new() { FullName = "Siti Aisyah", Email = "siti.aisyah@example.com", PhoneNumber = "0123456789", MembershipDate = DateTime.UtcNow, IsActive = true }
        };

        context.Books.AddRange(books);
        context.Members.AddRange(members);
        context.SaveChanges();
    }
}
