using LibraryManagementSystem.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<AppLog> AppLogs => Set<AppLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasIndex(b => b.Isbn).IsUnique();
            entity.Property(b => b.Title).IsRequired().HasMaxLength(200);
            entity.Property(b => b.Author).IsRequired().HasMaxLength(150);
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasIndex(m => m.Email).IsUnique();
            entity.Property(m => m.FullName).IsRequired().HasMaxLength(150);
        });

        modelBuilder.Entity<Loan>(entity =>
        {
            entity.HasOne(l => l.Book)
                  .WithMany(b => b.Loans)
                  .HasForeignKey(l => l.BookId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(l => l.Member)
                  .WithMany(m => m.Loans)
                  .HasForeignKey(l => l.MemberId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        base.OnModelCreating(modelBuilder);
    }
}
