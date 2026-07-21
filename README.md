# Library Management System — Web API

A CRUD Web API built with ASP.NET Core 8 and Entity Framework Core, implementing a
library management system with Books, Members, and Loans.

## Architecture

The solution is split into three projects to keep concerns separated:

```
LibraryManagementSystem.Api             → Controllers, DI wiring, Swagger, middleware
LibraryManagementSystem.Core            → Domain models, DTOs, interfaces (no dependencies)
LibraryManagementSystem.Infrastructure  → EF Core DbContext, repositories, business services
```

- **Controllers** are thin — they validate the model state and delegate to services.
- **Services** hold the business rules (e.g. a book can't be borrowed with zero available
  copies; a book/member can't be deleted while it has an active loan).
- **Repositories** are a thin data-access layer over EF Core, so the business logic isn't
  coupled directly to `DbContext`.
- **DTOs** are used at the API boundary instead of exposing EF entities directly, to avoid
  over-posting issues and to keep the API contract stable independent of the DB schema.

## Entities

| Entity | Key fields |
|---|---|
| **Book** | Title, Author, ISBN (unique), Genre, PublishedYear, TotalCopies, AvailableCopies |
| **Member** | FullName, Email (unique), PhoneNumber, MembershipDate, IsActive |
| **Loan** | BookId, MemberId, LoanDate, DueDate, ReturnDate, Status (Borrowed/Returned/Overdue) |

## Business rules

- Borrowing a book decrements `AvailableCopies`; rejected if none are available.
- Returning a loan sets `ReturnDate`, marks it `Returned`, and increments `AvailableCopies`.
- A book or member with an active (unreturned) loan cannot be deleted.
- Loans default to a 14-day due date unless a custom due date is supplied.

## API Endpoints

**Books** — `/api/books`
- `GET /api/books` — list all
- `GET /api/books/{id}` — get one
- `POST /api/books` — create
- `PUT /api/books/{id}` — update
- `DELETE /api/books/{id}` — delete (blocked if active loans exist)

**Members** — `/api/members`
- `GET /api/members` — list all
- `GET /api/members/{id}` — get one
- `POST /api/members` — register
- `PUT /api/members/{id}` — update
- `DELETE /api/members/{id}` — delete (blocked if active loans exist)

**Loans** — `/api/loans`
- `GET /api/loans` — list all
- `GET /api/loans/{id}` — get one
- `POST /api/loans` — borrow a book
- `PUT /api/loans/{id}/return` — return a book
- `DELETE /api/loans/{id}` — delete a loan record

## Running locally

Requires the [.NET 8 SDK](https://dotnet.microsoft.com/download).

```bash
# from the repo root
dotnet restore
dotnet run --project src/LibraryManagementSystem.Api
```

This Project Uses SQL SERVER, execute the query to generate sql table schema.

Swagger UI opens at the root URL (e.g.
`https://localhost:5001/`) with all endpoints documented and testable.

The database is seeded with a few demo books and members on first startup.

### Switching to SQL Server

Update `ConnectionStrings:DefaultConnection` in `appsettings.json` and change
`options.UseSqlite(...)` to `options.UseSqlServer(...)` in `Program.cs`.

## Possible next steps

- Add pagination/filtering on `GET /api/books`
- Add a background job to flag overdue loans (`Status = Overdue`)
- Add authentication (JWT) and role-based access (Librarian vs Member)
- Add integration tests using `WebApplicationFactory`
