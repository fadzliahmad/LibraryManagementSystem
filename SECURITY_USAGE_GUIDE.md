# API Security Usage Guide

## Overview
Your Library Management System API now uses **API Key Authentication** with role-based access control. Only write operations (POST, PUT, DELETE) require authentication.

---

## 🔑 Available API Keys

Three sample API keys have been configured in `appsettings.json`:

| API Key | Role | Permissions | Use Case |
|---------|------|-------------|----------|
| `sk_admin_key_12345678901234567890` | Admin | Full CRUD (Create, Read, Update, Delete) | Administrator operations |
| `sk_librarian_key_1234567890123456` | Librarian | Full CRUD (Create, Read, Update, Delete) | Librarian operations |
| `sk_member_key_1234567890123456789` | Member | Read-only | Member browsing |

---

## 🚀 Quick Start

### 1. **Public Endpoints (No Authentication Required)**

These GET endpoints are public and can be accessed without an API key:

```bash
# Get all books
GET http://localhost:5000/api/books

# Get a specific book
GET http://localhost:5000/api/books/{id}

# Get all members
GET http://localhost:5000/api/members

# Get a specific member
GET http://localhost:5000/api/members/{id}

# Get all loans
GET http://localhost:5000/api/loans

# Get a specific loan
GET http://localhost:5000/api/loans/{id}
```

### 2. **Protected Endpoints (Authentication Required)**

All write operations require an API key. Pass it in the `X-API-Key` header.

---

## 📝 How to Make Authenticated Requests

### Option A: Using cURL

```bash
# Create a new book (Admin role)
curl -X POST http://localhost:5000/api/books \
  -H "X-API-Key: sk_admin_key_12345678901234567890" \
  -H "Content-Type: application/json" \
  -d '{
	"title": "Clean Code",
	"author": "Robert C. Martin",
	"isbn": "978-0132350884",
	"totalCopies": 5
  }'

# Update a book
curl -X PUT http://localhost:5000/api/books/1 \
  -H "X-API-Key: sk_librarian_key_1234567890123456" \
  -H "Content-Type: application/json" \
  -d '{
	"title": "Clean Code (Updated)",
	"author": "Robert C. Martin",
	"isbn": "978-0132350884"
  }'

# Delete a book
curl -X DELETE http://localhost:5000/api/books/1 \
  -H "X-API-Key: sk_admin_key_12345678901234567890"
```

### Option B: Using PowerShell

```powershell
# Create a new book
$headers = @{
	'X-API-Key' = 'sk_admin_key_12345678901234567890'
	'Content-Type' = 'application/json'
}

$body = @{
	title = "Clean Code"
	author = "Robert C. Martin"
	isbn = "978-0132350884"
	totalCopies = 5
} | ConvertTo-Json

Invoke-RestMethod -Uri 'http://localhost:5000/api/books' `
  -Method Post `
  -Headers $headers `
  -Body $body
```

### Option C: Using Postman

1. Create a new POST request
2. Enter the URL: `http://localhost:5000/api/books`
3. Go to the **Headers** tab
4. Add a new header:
   - **Key**: `X-API-Key`
   - **Value**: `sk_admin_key_12345678901234567890`
5. Go to the **Body** tab, select **raw** and **JSON**
6. Enter your JSON data
7. Click **Send**

### Option D: Using Swagger UI

1. Open Swagger UI in your browser: `http://localhost:5000/`
2. Click the **Authorize** button (lock icon) at the top
3. In the **ApiKey** dialog, enter your API key: `sk_admin_key_12345678901234567890`
4. Click **Authorize**
5. Now try any protected endpoint - the API key will be automatically included

---

## 📚 Full Endpoint Reference

### Books Controller

| Method | Endpoint | Requires Auth | Role |
|--------|----------|---------------|------|
| GET | `/api/books` | ❌ | Public |
| GET | `/api/books/{id}` | ❌ | Public |
| POST | `/api/books` | ✅ | Admin, Librarian |
| PUT | `/api/books/{id}` | ✅ | Admin, Librarian |
| DELETE | `/api/books/{id}` | ✅ | Admin, Librarian |

**Example: Create a Book**
```http
POST /api/books HTTP/1.1
Host: localhost:5000
X-API-Key: sk_admin_key_12345678901234567890
Content-Type: application/json

{
  "title": "The Pragmatic Programmer",
  "author": "Andrew Hunt",
  "isbn": "978-0201616224",
  "totalCopies": 3
}
```

---

### Members Controller

| Method | Endpoint | Requires Auth | Role |
|--------|----------|---------------|------|
| GET | `/api/members` | ❌ | Public |
| GET | `/api/members/{id}` | ❌ | Public |
| POST | `/api/members` | ✅ | Admin, Librarian |
| PUT | `/api/members/{id}` | ✅ | Admin, Librarian |
| DELETE | `/api/members/{id}` | ✅ | Admin, Librarian |

**Example: Register a New Member**
```http
POST /api/members HTTP/1.1
Host: localhost:5000
X-API-Key: sk_librarian_key_1234567890123456
Content-Type: application/json

{
  "name": "John Doe",
  "email": "john@example.com",
  "phone": "555-1234"
}
```

---

### Loans Controller

| Method | Endpoint | Requires Auth | Role |
|--------|----------|---------------|------|
| GET | `/api/loans` | ❌ | Public |
| GET | `/api/loans/{id}` | ❌ | Public |
| POST | `/api/loans` | ✅ | Admin, Librarian, Member |
| PUT | `/api/loans/{id}/return` | ✅ | Admin, Librarian, Member |
| DELETE | `/api/loans/{id}` | ✅ | Admin, Librarian |

**Example: Create a Loan (Borrow a Book)**
```http
POST /api/loans HTTP/1.1
Host: localhost:5000
X-API-Key: sk_member_key_1234567890123456789
Content-Type: application/json

{
  "bookId": 1,
  "memberId": 1,
  "dueDate": "2024-02-15"
}
```

**Example: Return a Loan**
```http
PUT /api/loans/1/return HTTP/1.1
Host: localhost:5000
X-API-Key: sk_member_key_1234567890123456789
```

---

## ❌ Error Responses

### Missing API Key
```
Status: 401 Unauthorized
Response: "Invalid or expired API key."
```

### Invalid API Key
```
Status: 401 Unauthorized
Response: "Invalid or expired API key."
```

### Insufficient Permissions (Member trying to create)
```
Status: 401 Unauthorized
Response: "Invalid or expired API key."
```
*Note: Members can only READ, not WRITE*

### Malformed Request
```
Status: 400 Bad Request
Response: {
  "error": "The Name field is required."
}
```

---

## 🔧 Adding New API Keys

Edit `appsettings.json`:

```json
"ApiKeys": [
  {
	"Key": "sk_your_custom_key_here",
	"Name": "My Custom Key",
	"Role": "Admin",
	"IsActive": true,
	"CreatedAt": "2024-01-01T00:00:00Z",
	"ExpiresAt": null
  }
]
```

**Role Options**: `Admin`, `Librarian`, or `Member`

---

## 🔐 Permission Matrix

| Action | Admin | Librarian | Member |
|--------|-------|-----------|--------|
| View Books | ✅ | ✅ | ✅ |
| Create Books | ✅ | ✅ | ❌ |
| Update Books | ✅ | ✅ | ❌ |
| Delete Books | ✅ | ✅ | ❌ |
| View Members | ✅ | ✅ | ✅ |
| Create Members | ✅ | ✅ | ❌ |
| Update Members | ✅ | ✅ | ❌ |
| Delete Members | ✅ | ✅ | ❌ |
| Borrow Books | ✅ | ✅ | ✅ |
| Return Books | ✅ | ✅ | ✅ |
| Delete Loans | ✅ | ✅ | ❌ |

---

## 💡 Testing Workflow

### Step 1: Test Public Endpoints
```bash
curl http://localhost:5000/api/books
# Should return 200 with list of books
```

### Step 2: Try Protected Endpoint Without Key
```bash
curl -X POST http://localhost:5000/api/books \
  -H "Content-Type: application/json" \
  -d '{"title":"Test"}'
# Should return 401 Unauthorized
```

### Step 3: Use Admin Key for Protected Endpoint
```bash
curl -X POST http://localhost:5000/api/books \
  -H "X-API-Key: sk_admin_key_12345678901234567890" \
  -H "Content-Type: application/json" \
  -d '{
	"title": "Test Book",
	"author": "Test Author",
	"isbn": "123456789",
	"totalCopies": 1
  }'
# Should return 201 Created
```

### Step 4: Try Member Key for Write Operation
```bash
curl -X POST http://localhost:5000/api/books \
  -H "X-API-Key: sk_member_key_1234567890123456789" \
  -H "Content-Type: application/json" \
  -d '{"title":"Test"}'
# Should return 401 (Members can't write)
```

---

## 🧪 Using Swagger UI for Testing

1. **Start your API**: The app loads with `dotnet run`
2. **Open Browser**: Navigate to `http://localhost:5000/`
3. **Click Authorize**: Enter your API key when prompted
4. **Make Requests**: Swagger UI will automatically include the `X-API-Key` header

---

## 📌 Important Notes

- ✅ **All GET endpoints are public** - No authentication needed
- ✅ **All POST/PUT/DELETE endpoints are protected** - API key required
- ✅ **Member role is read-only** - Can view data and borrow books, but can't manage library
- ✅ **API keys are case-sensitive** - Make sure you copy them exactly
- ✅ **API keys are validated on every request** - So they can't be expired or inactive
- ⚠️ **Don't commit real API keys to version control** - Use environment variables for production
- ⚠️ **Current setup loads from config file** - For production, consider storing in a database

---

## 🚀 Next Steps (Production Recommendations)

1. **Use Environment Variables**: Move API keys to environment variables instead of `appsettings.json`
2. **Store Keys in Database**: For real applications, store API keys in a database with hashing
3. **Add Key Expiration**: Implement automatic key expiration
4. **Add Rate Limiting**: Prevent abuse with rate limiting
5. **Use HTTPS**: Always use HTTPS in production (not HTTP)
6. **Log Requests**: Log authentication failures for security monitoring

---

## 🆘 Troubleshooting

**Q: Getting "Invalid or expired API key" even with correct key?**
- Ensure the key matches exactly (case-sensitive)
- Check that you're using `X-API-Key` header (not `Authorization`)
- Verify the key is set as `IsActive: true` in appsettings.json

**Q: Can't see the Authorize button in Swagger?**
- Make sure the API is running
- Refresh the Swagger page (hard refresh: Ctrl+Shift+R)
- Check your browser console for errors

**Q: Getting 401 when trying to create with Member key?**
- This is expected! Members are read-only
- Use Admin or Librarian key for write operations

**Q: Want to test without API key temporarily?**
- Comment out `[Authorize]` attributes in controllers (not recommended for production)
- Or create a public test key with appropriate role

---

## 📞 Need Help?

Refer to the authentication handler at:
- `src/LibraryManagementSystem.Api/Authentication/ApiKeyAuthenticationHandler.cs`

Or the service implementation at:
- `src/LibraryManagementSystem.Infrastructure/Services/ApiKeyService.cs`
