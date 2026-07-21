-- =====================================================
-- Library Management System - SQL Server Schema
-- =====================================================
-- This script creates the database schema for the Library Management System
-- supporting Books, Members, and Loans management.

-- Create the database if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'LibraryManagementSystem')
BEGIN
	CREATE DATABASE LibraryManagementSystem;
END;
GO

USE LibraryManagementSystem;
GO

-- =====================================================
-- Books Table
-- =====================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Books')
BEGIN
	CREATE TABLE [dbo].[Books]
	(
		[Id] INT PRIMARY KEY IDENTITY(1,1),
		[Title] NVARCHAR(200) NOT NULL,
		[Author] NVARCHAR(150) NOT NULL,
		[Isbn] NVARCHAR(MAX) NOT NULL,
		[Genre] NVARCHAR(MAX) NOT NULL,
		[PublishedYear] INT NOT NULL,
		[TotalCopies] INT NOT NULL,
		[AvailableCopies] INT NOT NULL
	);

	-- Create unique index on ISBN
	CREATE UNIQUE INDEX [IX_Books_Isbn] ON [dbo].[Books]([Isbn]);
END;
GO

-- =====================================================
-- Members Table
-- =====================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Members')
BEGIN
	CREATE TABLE [dbo].[Members]
	(
		[Id] INT PRIMARY KEY IDENTITY(1,1),
		[FullName] NVARCHAR(150) NOT NULL,
		[Email] NVARCHAR(MAX) NOT NULL,
		[PhoneNumber] NVARCHAR(MAX) NOT NULL,
		[MembershipDate] DATETIME2 NOT NULL,
		[IsActive] BIT NOT NULL
	);

	-- Create unique index on Email
	CREATE UNIQUE INDEX [IX_Members_Email] ON [dbo].[Members]([Email]);
END;
GO

-- =====================================================
-- Loans Table
-- =====================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Loans')
BEGIN
	CREATE TABLE [dbo].[Loans]
	(
		[Id] INT PRIMARY KEY IDENTITY(1,1),
		[BookId] INT NOT NULL,
		[MemberId] INT NOT NULL,
		[LoanDate] DATETIME2 NOT NULL,
		[DueDate] DATETIME2 NOT NULL,
		[ReturnDate] DATETIME2 NULL,
		[Status] INT NOT NULL,
		CONSTRAINT [FK_Loans_Books_BookId] FOREIGN KEY ([BookId]) 
			REFERENCES [dbo].[Books]([Id]) ON DELETE NO ACTION,
		CONSTRAINT [FK_Loans_Members_MemberId] FOREIGN KEY ([MemberId]) 
			REFERENCES [dbo].[Members]([Id]) ON DELETE NO ACTION
	);

	-- Create indexes on foreign keys for better query performance
	CREATE INDEX [IX_Loans_BookId] ON [dbo].[Loans]([BookId]);
	CREATE INDEX [IX_Loans_MemberId] ON [dbo].[Loans]([MemberId]);
END;
GO

-- =====================================================
-- Add Constraints to ensure data integrity
-- =====================================================

-- Ensure Books has positive copy counts
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Books_TotalCopies')
BEGIN
	ALTER TABLE [dbo].[Books]
	ADD CONSTRAINT [CK_Books_TotalCopies] 
	CHECK ([TotalCopies] >= 0);
END;
GO

IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Books_AvailableCopies')
BEGIN
	ALTER TABLE [dbo].[Books]
	ADD CONSTRAINT [CK_Books_AvailableCopies] 
	CHECK ([AvailableCopies] >= 0 AND [AvailableCopies] <= [TotalCopies]);
END;
GO

-- Ensure Member email is not empty
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Members_Email')
BEGIN
	ALTER TABLE [dbo].[Members]
	ADD CONSTRAINT [CK_Members_Email] 
	CHECK (LEN([Email]) > 0);
END;
GO

-- Ensure Loan dates are logical
IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Loans_DatesLogical')
BEGIN
	ALTER TABLE [dbo].[Loans]
	ADD CONSTRAINT [CK_Loans_DatesLogical] 
	CHECK ([DueDate] >= [LoanDate] AND ([ReturnDate] IS NULL OR [ReturnDate] >= [LoanDate]));
END;
GO

-- =====================================================
-- Sample Data (Optional - for demonstration)
-- =====================================================

-- Insert sample books
IF NOT EXISTS (SELECT 1 FROM [dbo].[Books])
BEGIN
	INSERT INTO [dbo].[Books] 
	([Title], [Author], [Isbn], [Genre], [PublishedYear], [TotalCopies], [AvailableCopies])
	VALUES
	(N'The Great Gatsby', N'F. Scott Fitzgerald', N'978-0-7432-7356-5', N'Fiction', 1925, 5, 5),
	(N'To Kill a Mockingbird', N'Harper Lee', N'978-0-06-112008-4', N'Fiction', 1960, 4, 4),
	(N'1984', N'George Orwell', N'978-0-451-52493-2', N'Dystopian Fiction', 1949, 6, 6),
	(N'Pride and Prejudice', N'Jane Austen', N'978-0-14-143951-8', N'Romance', 1813, 3, 3),
	(N'Brave New World', N'Aldous Huxley', N'978-0-06-085052-4', N'Science Fiction', 1932, 5, 5);

	PRINT 'Sample books inserted successfully.';
END;
GO

-- Insert sample members
IF NOT EXISTS (SELECT 1 FROM [dbo].[Members])
BEGIN
	INSERT INTO [dbo].[Members] 
	([FullName], [Email], [PhoneNumber], [MembershipDate], [IsActive])
	VALUES
	(N'John Smith', N'john.smith@example.com', N'+1-555-0101', CAST(GETUTCDATE() AS DATETIME2), 1),
	(N'Jane Doe', N'jane.doe@example.com', N'+1-555-0102', CAST(GETUTCDATE() AS DATETIME2), 1),
	(N'Robert Johnson', N'robert.johnson@example.com', N'+1-555-0103', CAST(GETUTCDATE() AS DATETIME2), 1),
	(N'Maria Garcia', N'maria.garcia@example.com', N'+1-555-0104', CAST(GETUTCDATE() AS DATETIME2), 1);

	PRINT 'Sample members inserted successfully.';
END;
GO

-- =====================================================
-- Useful Views for Reporting
-- =====================================================

-- View: Borrowed Books (currently on loan)
IF OBJECT_ID(N'dbo.vw_BorrowedBooks', N'V') IS NOT NULL
	DROP VIEW dbo.vw_BorrowedBooks;
GO

CREATE VIEW [dbo].[vw_BorrowedBooks]
AS
SELECT 
	l.[Id] AS LoanId,
	b.[Id] AS BookId,
	b.[Title],
	b.[Author],
	m.[Id] AS MemberId,
	m.[FullName],
	m.[Email],
	l.[LoanDate],
	l.[DueDate],
	CAST(DATEDIFF(DAY, GETUTCDATE(), l.[DueDate]) AS INT) AS DaysUntilDue,
	CASE 
		WHEN l.[DueDate] < GETUTCDATE() AND l.[ReturnDate] IS NULL THEN 'Overdue'
		WHEN l.[ReturnDate] IS NOT NULL THEN 'Returned'
		ELSE 'Active'
	END AS LoanStatus
FROM [dbo].[Loans] l
INNER JOIN [dbo].[Books] b ON l.[BookId] = b.[Id]
INNER JOIN [dbo].[Members] m ON l.[MemberId] = m.[Id];
GO

-- View: Book Availability Summary
IF OBJECT_ID(N'dbo.vw_BookAvailability', N'V') IS NOT NULL
	DROP VIEW dbo.vw_BookAvailability;
GO

CREATE VIEW [dbo].[vw_BookAvailability]
AS
SELECT 
	[Id],
	[Title],
	[Author],
	[Isbn],
	[Genre],
	[PublishedYear],
	[TotalCopies],
	[AvailableCopies],
	([TotalCopies] - [AvailableCopies]) AS CopiesOnLoan,
	CAST(ROUND(CAST([AvailableCopies] AS FLOAT) / CAST([TotalCopies] AS FLOAT) * 100, 2) AS DECIMAL(5, 2)) AS AvailabilityPercentage
FROM [dbo].[Books];
GO

-- View: Member Loan History
IF OBJECT_ID(N'dbo.vw_MemberLoanHistory', N'V') IS NOT NULL
	DROP VIEW dbo.vw_MemberLoanHistory;
GO

CREATE VIEW [dbo].[vw_MemberLoanHistory]
AS
SELECT 
	m.[Id] AS MemberId,
	m.[FullName],
	m.[Email],
	COUNT(l.[Id]) AS TotalLoans,
	SUM(CASE WHEN l.[ReturnDate] IS NULL THEN 1 ELSE 0 END) AS ActiveLoans,
	SUM(CASE WHEN l.[ReturnDate] IS NOT NULL THEN 1 ELSE 0 END) AS CompletedLoans
FROM [dbo].[Members] m
LEFT JOIN [dbo].[Loans] l ON m.[Id] = l.[MemberId]
GROUP BY m.[Id], m.[FullName], m.[Email];
GO

-- =====================================================
-- Useful Stored Procedures
-- =====================================================

-- Stored Procedure: Get Overdue Loans
IF OBJECT_ID(N'dbo.sp_GetOverdueLoans', N'P') IS NOT NULL
	DROP PROCEDURE dbo.sp_GetOverdueLoans;
GO

CREATE PROCEDURE [dbo].[sp_GetOverdueLoans]
AS
BEGIN
	SELECT 
		l.[Id],
		b.[Title],
		b.[Author],
		m.[FullName],
		m.[Email],
		l.[DueDate],
		CAST(DATEDIFF(DAY, l.[DueDate], GETUTCDATE()) AS INT) AS DaysOverdue
	FROM [dbo].[Loans] l
	INNER JOIN [dbo].[Books] b ON l.[BookId] = b.[Id]
	INNER JOIN [dbo].[Members] m ON l.[MemberId] = m.[Id]
	WHERE l.[ReturnDate] IS NULL 
		AND l.[DueDate] < CAST(GETUTCDATE() AS DATE)
	ORDER BY l.[DueDate] ASC;
END;
GO

-- Stored Procedure: Return a Book
IF OBJECT_ID(N'dbo.sp_ReturnBook', N'P') IS NOT NULL
	DROP PROCEDURE dbo.sp_ReturnBook;
GO

CREATE PROCEDURE [dbo].[sp_ReturnBook]
	@LoanId INT
AS
BEGIN
	BEGIN TRANSACTION;

	BEGIN TRY
		-- Update loan record
		UPDATE [dbo].[Loans]
		SET [ReturnDate] = GETUTCDATE(),
			[Status] = 0  -- Assuming 0 = Returned
		WHERE [Id] = @LoanId;

		-- Update book availability
		DECLARE @BookId INT;
		SELECT @BookId = [BookId] FROM [dbo].[Loans] WHERE [Id] = @LoanId;

		UPDATE [dbo].[Books]
		SET [AvailableCopies] = [AvailableCopies] + 1
		WHERE [Id] = @BookId;

		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION;
		THROW;
	END CATCH;
END;
GO

-- =====================================================
-- End of Script
-- =====================================================
PRINT 'Schema creation completed successfully!';
