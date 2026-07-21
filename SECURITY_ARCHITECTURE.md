# 🏗️ Security Architecture Overview

## System Flow Diagram

```
User Request
	↓
Add X-API-Key Header?
	├─ NO (GET request) → [Public Endpoints] ✅ Allowed
	│   └─ Response: Data (200 OK)
	│
	└─ YES (POST/PUT/DELETE request) ↓
		↓
	[ApiKeyAuthenticationHandler]
		├─ Extract header: X-API-Key
		├─ Call: IApiKeyService.ValidateApiKeyAsync()
		│
		├─ NO → 401 Unauthorized ❌
		│   └─ Response: "Invalid or expired API key"
		│
		└─ YES → Check Role
			├─ Admin     → Process Request ✅
			├─ Librarian → Process Request ✅
			└─ Member    → 401 Unauthorized ❌ (Read-only)
```

---

## Request Journey

### Example 1: GET /api/books (Public)
```
Request: GET /api/books
		 (No X-API-Key needed)
			↓
		[BooksController.GetAll()]
			↓
		[Database Query]
			↓
Response: 200 OK
		 [Array of books]
```

### Example 2: POST /api/books (Protected)
```
Request: POST /api/books
		 Header: X-API-Key: sk_admin_key_...
			↓
		[ApiKeyAuthenticationHandler]
			├─ Find key in config
			├─ Check: IsActive = true
			├─ Check: Not expired
			├─ Check: Valid Admin role
			↓
		[Authorize] attribute checks
			├─ User has valid identity
			├─ Role allows operation
			↓
		[BooksController.Create()]
			↓
		[Database Write]
			↓
Response: 201 Created
		 [New book data]
```

### Example 3: POST /api/books with Member Key (Blocked)
```
Request: POST /api/books
		 Header: X-API-Key: sk_member_key_...
			↓
		[ApiKeyAuthenticationHandler]
			├─ Find key in config
			├─ Check: IsActive = true ✅
			├─ Check: Not expired ✅
			├─ Check: Valid Member role ✅
			└─ Create identity with Member role
			↓
		[Authorize] attribute checks
			├─ User has valid identity ✅
			├─ Role allows write operation? ❌
			├─ Member can only READ
			↓
Response: 401 Unauthorized
		 ❌ Access Denied
```

---

## Architecture Layers

```
┌─────────────────────────────────────┐
│       API Controllers               │
│  (Books, Members, Loans)            │
│  [Authorize] on POST/PUT/DELETE      │
└────────────┬────────────────────────┘
			 ↓
┌─────────────────────────────────────┐
│   Authentication Handler            │
│  ~/Api/Authentication/              │
│  ApiKeyAuthenticationHandler.cs      │
│  • Extracts X-API-Key header        │
│  • Validates key format             │
│  • Creates ClaimsPrincipal          │
└────────────┬────────────────────────┘
			 ↓
┌─────────────────────────────────────┐
│   API Key Service                   │
│  ~/Infrastructure/Services/         │
│  ApiKeyService.cs                   │
│  • Loads keys from config           │
│  • Validates against database       │
│  • Checks expiration                │
│  • Manages permissions              │
└────────────┬────────────────────────┘
			 ↓
┌─────────────────────────────────────┐
│   Configuration                     │
│  appsettings.json                   │
│  • API keys with roles              │
│  • Expiration dates                 │
│  • Active status                    │
└─────────────────────────────────────┘
```

---

## Component Diagram

```
┌─────────────────────────────────────────┐
│    LibraryManagementSystem.Core         │
├─────────────────────────────────────────┤
│ • Models/UserRole.cs      [Enum]        │
│ • Models/ApiKeyIdentity.cs [Class]      │
│ • Interfaces/IApiKeyService.cs [Iface]  │
└─────────────────────────────────────────┘
		   ↑
		   │ Implements
		   │
┌─────────────────────────────────────────┐
│   LibraryManagementSystem.Infrastructure│
├─────────────────────────────────────────┤
│ • Services/ApiKeyService.cs             │
│   ├─ ValidateApiKeyAsync()              │
│   ├─ GetAllKeysAsync()                  │
│   ├─ CreateApiKeyAsync()                │
│   ├─ RevokeApiKeyAsync()                │
│   └─ CanPerformAction()                 │
└─────────────────────────────────────────┘
		   ↑
		   │ Uses
		   │
┌─────────────────────────────────────────┐
│     LibraryManagementSystem.Api         │
├─────────────────────────────────────────┤
│ • Authentication/                       │
│   ├─ ApiKeyAuthenticationHandler.cs     │
│   └─ ApiKeyAuthenticationSchemeOptions  │
│ • Controllers/                          │
│   ├─ BooksController.cs    [Authorize] │
│   ├─ MembersController.cs  [Authorize] │
│   └─ LoansController.cs    [Authorize] │
│ • Program.cs                            │
│   └─ .AddAuthentication()               │
│   └─ .AddAuthorization()                │
└─────────────────────────────────────────┘
```

---

## Permission Matrix Logic

```csharp
// Simplified logic from ApiKeyService.cs
public bool CanPerformAction(UserRole role, string action)
{
	var permissions = new Dictionary<UserRole, List<string>>
	{
		// Admin: Full access
		{ UserRole.Admin, new List<string> 
		  { "create", "read", "update", "delete" } },

		// Librarian: Full access
		{ UserRole.Librarian, new List<string> 
		  { "create", "read", "update", "delete" } },

		// Member: Read-only
		{ UserRole.Member, new List<string> 
		  { "read" } }  // Can only read, cannot write
	};

	return permissions[role].Contains(action.ToLower());
}
```

---

## Header Validation Flow

```
HTTP Request
	↓
Headers: {
  "X-API-Key": "sk_admin_key_12345678901234567890",
  "Content-Type": "application/json"
}
	↓
[ApiKeyAuthenticationHandler.HandleAuthenticateAsync()]
	↓
1. Extract header ✓
   value = "sk_admin_key_12345678901234567890"
	↓
2. Query appsettings.json
	↓
3. Find matching entry
	{
	  "Key": "sk_admin_key_12345678901234567890",
	  "Name": "Admin Key",
	  "Role": "Admin",
	  "IsActive": true,
	  "ExpiresAt": null
	}
	↓
4. Validate conditions
   ✓ Key matches
   ✓ IsActive == true
   ✓ ExpiresAt == null OR ExpiresAt > Now
	↓
5. Create Claims
   ClaimTypes.NameIdentifier: "sk_admin_key_12345678901234567890"
   ClaimTypes.Name: "Admin Key"
   ClaimTypes.Role: "Admin"
	↓
6. Create ClaimsPrincipal
	↓
7. Return AuthenticateResult.Success()
	↓
[Authorize] attribute
	├─ Checks: User has valid identity ✓
	└─ Processes request ✓
```

---

## Key Files and Responsibilities

```
📦 LibraryManagementSystem
│
├── 📁 src/LibraryManagementSystem.Core
│   ├── 📁 Models
│   │   ├── UserRole.cs .............. Enum with Admin, Librarian, Member
│   │   └── ApiKeyIdentity.cs ........ API key model with role & dates
│   └── 📁 Interfaces
│       └── IApiKeyService.cs ........ Interface for key validation
│
├── 📁 src/LibraryManagementSystem.Infrastructure
│   └── 📁 Services
│       └── ApiKeyService.cs ......... Validates and manages API keys
│
├── 📁 src/LibraryManagementSystem.Api
│   ├── 📁 Authentication
│   │   └── ApiKeyAuthenticationHandler.cs .. Handles X-API-Key header
│   ├── 📁 Controllers
│   │   ├── BooksController.cs ....... [Authorize] on write ops
│   │   ├── MembersController.cs ..... [Authorize] on write ops
│   │   └── LoansController.cs ....... [Authorize] on write ops
│   ├── Program.cs .................. Registers security middleware
│   └── appsettings.json ............ Contains API keys & roles
│
├── 📄 SECURITY_USAGE_GUIDE.md ....... Complete documentation
├── 📄 QUICK_START_SECURITY.md ....... Quick reference
└── 📄 EXAMPLES_COPY_PASTE.md ....... Ready-to-use examples
```

---

## Data Flow Example: Create Book

```
┌─ HTTP Client ─┐
│ POST /api/books
│ X-API-Key: sk_admin_key_...
│ { "title": "..." }
└────────────────┘
		│
		↓
┌─ BooksController ─┐
│ [Authorize]
│ Checks user has identity
│ Processes request
└────────────────────┘
		│
		↓
┌─ Create validation ─┐
│ Checks ApiKeyIdentity
│ Verifies role: Admin
│ Continues ✓
└──────────────────────┘
		│
		↓
┌─ Business Logic ─┐
│ IBookService
│ Creates entry
└─────────────────┘
		│
		↓
┌─ Database ─┐
│ Save new
│ book
└─────────────┘
		│
		↓
┌─ Response ─┐
│ 201 Created
│ Book object
└─────────────┘
```

---

## Security Levels

```
┌─────────────────────────────────────┐
│  Level 1: Header Validation         │
│  ✓ No X-API-Key? → Block            │
│  ✓ Invalid key? → Block             │
│  ✓ Expired key? → Block             │
└─────────────────────────────────────┘
		 ↓
┌─────────────────────────────────────┐
│  Level 2: Authentication            │
│  ✓ Key exists in config?            │
│  ✓ Key IsActive = true?             │
│  ✓ Not expired?                     │
└─────────────────────────────────────┘
		 ↓
┌─────────────────────────────────────┐
│  Level 3: Authorization             │
│  ✓ [Authorize] attribute present?   │
│  ✓ Is it a protected endpoint?      │
│  ✓ User has valid identity?         │
└─────────────────────────────────────┘
		 ↓
┌─────────────────────────────────────┐
│  Level 4: Permission Matrix         │
│  ✓ Role allows this action?         │
│  ✓ Is it CREATE, READ, DELETE?      │
│  ✓ Admin/Librarian can all ops      │
│  ✓ Member can only read             │
└─────────────────────────────────────┘
		 ↓
✅ REQUEST APPROVED & PROCESSED
```

---

## Configuration Structure

```json
appsettings.json:

{
  "ApiKeys": [
	{
	  "Key": "sk_admin_key_12345678901234567890",     // Unique identifier
	  "Name": "Admin Key",                             // Display name
	  "Role": "Admin",                                 // Permission level
	  "IsActive": true,                                // Can be revoked
	  "CreatedAt": "2024-01-01T00:00:00Z",            // Audit trail
	  "ExpiresAt": null                                // null = never expires
	}
  ]
}
```

---

## Complete Request/Response Examples

### ✅ Success: GET /api/books (Public)
```http
GET /api/books HTTP/1.1
Host: localhost:5000

HTTP/1.1 200 OK
Content-Type: application/json

[
  {
	"id": 1,
	"title": "Clean Code",
	"author": "Robert C. Martin",
	"isbn": "978-0132350884",
	"totalCopies": 5,
	"availableCopies": 3
  }
]
```

### ✅ Success: POST /api/books (Admin)
```http
POST /api/books HTTP/1.1
Host: localhost:5000
X-API-Key: sk_admin_key_12345678901234567890
Content-Type: application/json

{
  "title": "New Book",
  "author": "Author Name",
  "isbn": "1234567890",
  "totalCopies": 5
}

HTTP/1.1 201 Created
Content-Type: application/json

{
  "id": 2,
  "title": "New Book",
  "author": "Author Name",
  "isbn": "1234567890",
  "totalCopies": 5,
  "availableCopies": 5
}
```

### ❌ Fail: POST /api/books (No Key)
```http
POST /api/books HTTP/1.1
Host: localhost:5000
Content-Type: application/json

{
  "title": "New Book",
  "author": "Author Name"
}

HTTP/1.1 401 Unauthorized
Content-Type: application/json

"Invalid or expired API key."
```

### ❌ Fail: POST /api/books (Member Key)
```http
POST /api/books HTTP/1.1
Host: localhost:5000
X-API-Key: sk_member_key_1234567890123456789
Content-Type: application/json

{
  "title": "New Book",
  "author": "Author Name"
}

HTTP/1.1 401 Unauthorized
Content-Type: application/json

"Invalid or expired API key."
```

---

**This is the complete security architecture for your API!** 🔐
