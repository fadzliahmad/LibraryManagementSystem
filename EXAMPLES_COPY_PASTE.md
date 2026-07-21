# 🧪 API Security - Copy & Paste Examples

Ready-to-use code examples. Just copy, paste, and run!

---

## 📋 Using cURL

### Public: Get All Books (No Auth)
```bash
curl http://localhost:5000/api/books
```

### Public: Get Specific Book (No Auth)
```bash
curl http://localhost:5000/api/books/1
```

### Protected: Create a Book (Admin)
```bash
curl -X POST http://localhost:5000/api/books \
  -H "X-API-Key: sk_admin_key_12345678901234567890" \
  -H "Content-Type: application/json" \
  -d '{
	"title": "Clean Code",
	"author": "Robert C. Martin",
	"isbn": "978-0132350884",
	"totalCopies": 5
  }'
```

### Protected: Create a Member (Librarian)
```bash
curl -X POST http://localhost:5000/api/members \
  -H "X-API-Key: sk_librarian_key_1234567890123456" \
  -H "Content-Type: application/json" \
  -d '{
	"name": "Jane Smith",
	"email": "jane@example.com",
	"phone": "555-9876"
  }'
```

### Protected: Update a Book (Admin)
```bash
curl -X PUT http://localhost:5000/api/books/1 \
  -H "X-API-Key: sk_admin_key_12345678901234567890" \
  -H "Content-Type: application/json" \
  -d '{
	"title": "Clean Code - Updated Edition",
	"author": "Robert C. Martin",
	"isbn": "978-0132350884"
  }'
```

### Protected: Delete a Book (Admin)
```bash
curl -X DELETE http://localhost:5000/api/books/1 \
  -H "X-API-Key: sk_admin_key_12345678901234567890"
```

### Protected: Create a Loan (Member)
```bash
curl -X POST http://localhost:5000/api/loans \
  -H "X-API-Key: sk_member_key_1234567890123456789" \
  -H "Content-Type: application/json" \
  -d '{
	"bookId": 1,
	"memberId": 1,
	"dueDate": "2024-02-15"
  }'
```

### Protected: Return a Loan (Member)
```bash
curl -X PUT http://localhost:5000/api/loans/1/return \
  -H "X-API-Key: sk_member_key_1234567890123456789"
```

---

## 🟦 Using PowerShell

### Public: Get All Books (No Auth)
```powershell
Invoke-RestMethod -Uri 'http://localhost:5000/api/books' -Method Get
```

### Protected: Create a Book (Admin)
```powershell
$headers = @{'X-API-Key' = 'sk_admin_key_12345678901234567890'}
$body = @{
	title = "Clean Code"
	author = "Robert C. Martin"
	isbn = "978-0132350884"
	totalCopies = 5
} | ConvertTo-Json

Invoke-RestMethod -Uri 'http://localhost:5000/api/books' `
  -Method Post `
  -Headers $headers `
  -Body $body `
  -ContentType 'application/json'
```

### Protected: Get All Members
```powershell
$headers = @{'X-API-Key' = 'sk_librarian_key_1234567890123456'}

Invoke-RestMethod -Uri 'http://localhost:5000/api/members' `
  -Method Get `
  -Headers $headers
```

### Protected: Create a Member (Librarian)
```powershell
$headers = @{'X-API-Key' = 'sk_librarian_key_1234567890123456'}
$body = @{
	name = "John Doe"
	email = "john@example.com"
	phone = "555-1234"
} | ConvertTo-Json

Invoke-RestMethod -Uri 'http://localhost:5000/api/members' `
  -Method Post `
  -Headers $headers `
  -Body $body `
  -ContentType 'application/json'
```

### Protected: Update a Member (Admin)
```powershell
$headers = @{'X-API-Key' = 'sk_admin_key_12345678901234567890'}
$body = @{
	name = "Jane Doe"
	email = "jane@example.com"
	phone = "555-5678"
} | ConvertTo-Json

Invoke-RestMethod -Uri 'http://localhost:5000/api/members/1' `
  -Method Put `
  -Headers $headers `
  -Body $body `
  -ContentType 'application/json'
```

### Protected: Delete a Member (Admin)
```powershell
$headers = @{'X-API-Key' = 'sk_admin_key_12345678901234567890'}

Invoke-RestMethod -Uri 'http://localhost:5000/api/members/1' `
  -Method Delete `
  -Headers $headers
```

### Protected: Create a Loan (Member)
```powershell
$headers = @{'X-API-Key' = 'sk_member_key_1234567890123456789'}
$body = @{
	bookId = 1
	memberId = 1
	dueDate = "2024-02-15"
} | ConvertTo-Json

Invoke-RestMethod -Uri 'http://localhost:5000/api/loans' `
  -Method Post `
  -Headers $headers `
  -Body $body `
  -ContentType 'application/json'
```

---

## 💻 Using C# / .NET HttpClient

### Basic Setup
```csharp
using System.Net.Http;
using System.Text.Json;

var client = new HttpClient();
var baseUrl = "http://localhost:5000";
```

### Get All Books (Public)
```csharp
var response = await client.GetAsync($"{baseUrl}/api/books");
var content = await response.Content.ReadAsStringAsync();
Console.WriteLine(content);
```

### Create Book (Admin)
```csharp
var apiKey = "sk_admin_key_12345678901234567890";
client.DefaultRequestHeaders.Add("X-API-Key", apiKey);

var bookData = new
{
	title = "Clean Code",
	author = "Robert C. Martin",
	isbn = "978-0132350884",
	totalCopies = 5
};

var json = JsonSerializer.Serialize(bookData);
var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

var response = await client.PostAsync($"{baseUrl}/api/books", content);
Console.WriteLine($"Status: {response.StatusCode}");
```

### Create Member (Librarian)
```csharp
var apiKey = "sk_librarian_key_1234567890123456";
client.DefaultRequestHeaders.Clear();
client.DefaultRequestHeaders.Add("X-API-Key", apiKey);

var memberData = new
{
	name = "John Smith",
	email = "john.smith@example.com",
	phone = "555-1234"
};

var json = JsonSerializer.Serialize(memberData);
var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

var response = await client.PostAsync($"{baseUrl}/api/members", content);
Console.WriteLine($"Status: {response.StatusCode}");
var result = await response.Content.ReadAsStringAsync();
Console.WriteLine(result);
```

### Create Loan (Member)
```csharp
var apiKey = "sk_member_key_1234567890123456789";
client.DefaultRequestHeaders.Clear();
client.DefaultRequestHeaders.Add("X-API-Key", apiKey);

var loanData = new
{
	bookId = 1,
	memberId = 1,
	dueDate = "2024-02-15"
};

var json = JsonSerializer.Serialize(loanData);
var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

var response = await client.PostAsync($"{baseUrl}/api/loans", content);
Console.WriteLine($"Status: {response.StatusCode}");
```

---

## 🟦 Using TypeScript/JavaScript (Fetch API)

### Get All Books (Public)
```typescript
const response = await fetch('http://localhost:5000/api/books');
const books = await response.json();
console.log(books);
```

### Create Book (Admin)
```typescript
const apiKey = 'sk_admin_key_12345678901234567890';

const response = await fetch('http://localhost:5000/api/books', {
  method: 'POST',
  headers: {
	'X-API-Key': apiKey,
	'Content-Type': 'application/json'
  },
  body: JSON.stringify({
	title: 'Clean Code',
	author: 'Robert C. Martin',
	isbn: '978-0132350884',
	totalCopies: 5
  })
});

const data = await response.json();
console.log(data);
```

### Create Member (Librarian)
```typescript
const apiKey = 'sk_librarian_key_1234567890123456';

const response = await fetch('http://localhost:5000/api/members', {
  method: 'POST',
  headers: {
	'X-API-Key': apiKey,
	'Content-Type': 'application/json'
  },
  body: JSON.stringify({
	name: 'Jane Smith',
	email: 'jane@example.com',
	phone: '555-9876'
  })
});

if (response.ok) {
  const member = await response.json();
  console.log('Member created:', member);
} else {
  console.error('Error:', response.statusText);
}
```

### Create Loan (Member)
```typescript
const apiKey = 'sk_member_key_1234567890123456789';

const response = await fetch('http://localhost:5000/api/loans', {
  method: 'POST',
  headers: {
	'X-API-Key': apiKey,
	'Content-Type': 'application/json'
  },
  body: JSON.stringify({
	bookId: 1,
	memberId: 1,
	dueDate: '2024-02-15'
  })
});

const loan = await response.json();
console.log(loan);
```

---

## 📮 Using Postman (Desktop App)

### Step 1: Create Request
- Click "+" → New Request
- Name: "Create Book"
- Method: POST
- URL: http://localhost:5000/api/books

### Step 2: Add Authentication Header
- Tab: "Headers"
- Key: `X-API-Key`
- Value: `sk_admin_key_12345678901234567890`

### Step 3: Add Body
- Tab: "Body"
- Select: "raw"
- Select: "JSON"
- Paste:
```json
{
  "title": "Clean Code",
  "author": "Robert C. Martin",
  "isbn": "978-0132350884",
  "totalCopies": 5
}
```

### Step 4: Send
- Click "Send" button
- View response

---

## 🌐 Using REST Client Extension (VS Code)

### Create `.rest` file
Save as `test.rest` in your workspace:

```rest
### Get all books (no auth)
GET http://localhost:5000/api/books

### Create book (with auth)
POST http://localhost:5000/api/books
X-API-Key: sk_admin_key_12345678901234567890
Content-Type: application/json

{
  "title": "Clean Code",
  "author": "Robert C. Martin",
  "isbn": "978-0132350884",
  "totalCopies": 5
}

### Create member
POST http://localhost:5000/api/members
X-API-Key: sk_librarian_key_1234567890123456
Content-Type: application/json

{
  "name": "John Doe",
  "email": "john@example.com",
  "phone": "555-1234"
}

### Create loan
POST http://localhost:5000/api/loans
X-API-Key: sk_member_key_1234567890123456789
Content-Type: application/json

{
  "bookId": 1,
  "memberId": 1,
  "dueDate": "2024-02-15"
}

### Update book
PUT http://localhost:5000/api/books/1
X-API-Key: sk_admin_key_12345678901234567890
Content-Type: application/json

{
  "title": "Clean Code - Updated",
  "author": "Robert C. Martin",
  "isbn": "978-0132350884"
}

### Delete book
DELETE http://localhost:5000/api/books/1
X-API-Key: sk_admin_key_12345678901234567890
```

Then click "Send Request" above each request in VS Code!

---

## 🧪 Testing Wrong API Key

### Should get 401 Unauthorized ❌
```bash
curl -X POST http://localhost:5000/api/books \
  -H "X-API-Key: wrong_key_12345678901234567890" \
  -H "Content-Type: application/json" \
  -d '{"title": "Test"}'
```

### Response:
```
HTTP/1.1 401 Unauthorized
Content-Type: application/json

{"type":"https://tools.ietf.org/html/rfc7231#section-6.3.1","title":"Unauthorized","status":401}
```

---

## 🔑 API Keys Reference

| Role | Key |
|------|-----|
| Admin | `sk_admin_key_12345678901234567890` |
| Librarian | `sk_librarian_key_1234567890123456` |
| Member (Read-Only) | `sk_member_key_1234567890123456789` |

---

**Pick your preferred tool and start testing!** 🚀
