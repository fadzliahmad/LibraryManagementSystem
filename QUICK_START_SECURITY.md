# 🔐 API Security - Quick Reference Card

## Instant Setup

Your API is already secured! Here's all you need to know:

---

## 3 Sample API Keys Ready to Use

```
Admin:      sk_admin_key_12345678901234567890
Librarian:  sk_librarian_key_1234567890123456
Member:     sk_member_key_1234567890123456789
```

---

## How to Authenticate

Add this header to your request:
```
X-API-Key: sk_admin_key_12345678901234567890
```

---

## Which Endpoints Need Auth?

| Need Auth? | Endpoints |
|-----------|-----------|
| ❌ NO | `GET /api/books`, `GET /api/members`, `GET /api/loans` |
| ✅ YES | `POST /api/books`, `PUT /api/books/{id}`, `DELETE /api/books/{id}` |
| ✅ YES | `POST /api/members`, `PUT /api/members/{id}`, `DELETE /api/members/{id}` |
| ✅ YES | `POST /api/loans`, `PUT /api/loans/{id}/return`, `DELETE /api/loans/{id}` |

---

## Quick Examples

### cURL - Get all books (no auth needed)
```bash
curl http://localhost:5000/api/books
```

### cURL - Create a book (auth needed)
```bash
curl -X POST http://localhost:5000/api/books \
  -H "X-API-Key: sk_admin_key_12345678901234567890" \
  -H "Content-Type: application/json" \
  -d '{
	"title": "New Book",
	"author": "Author Name",
	"isbn": "1234567890",
	"totalCopies": 5
  }'
```

### PowerShell - Create a member
```powershell
$headers = @{'X-API-Key' = 'sk_librarian_key_1234567890123456'}
$body = @{name='John'; email='john@example.com'; phone='555-1234'} | ConvertTo-Json

Invoke-RestMethod -Uri 'http://localhost:5000/api/members' `
  -Method Post -Headers $headers -Body $body -ContentType 'application/json'
```

### Postman
1. Create request
2. Add Header: `X-API-Key: sk_admin_key_12345678901234567890`
3. Set Body to JSON
4. Send!

### Swagger UI (Easiest!)
1. Go to `http://localhost:5000/`
2. Click the 🔒 **Authorize** button
3. Enter API key: `sk_admin_key_12345678901234567890`
4. Click buttons to test

---

## Who Can Do What?

```
Admin:      Can READ ✅ and WRITE ✅
Librarian:  Can READ ✅ and WRITE ✅
Member:     Can READ ✅ only (READ-ONLY)
```

---

## Wrong API Key?
```
Status: 401 Unauthorized
Message: "Invalid or expired API key."
```

---

## Add a New API Key?

Edit `appsettings.json`:
```json
{
  "Key": "sk_mykey_abc123456789012345",
  "Name": "My Custom Key",
  "Role": "Admin",
  "IsActive": true,
  "ExpiresAt": null
}
```

---

## Testing Checklist

- [ ] Can I GET `/api/books`? (should work without key)
- [ ] Can I POST to `/api/books` without key? (should fail 401)
- [ ] Can I POST to `/api/books` with Admin key? (should work 201)
- [ ] Can I POST to `/api/books` with Member key? (should fail 401)
- [ ] Does Swagger Authorize button work?

---

## Files You Need to Know

| File | Purpose |
|------|---------|
| `appsettings.json` | Contains your API keys |
| `src/LibraryManagementSystem.Api/Authentication/ApiKeyAuthenticationHandler.cs` | Validates API keys |
| `src/LibraryManagementSystem.Infrastructure/Services/ApiKeyService.cs` | Manages API key logic |

---

## Most Common Mistakes

1. ❌ Missing `X-API-Key` header → Use correct header name
2. ❌ Using wrong API key → Copy-paste exact key
3. ❌ Member role trying to write → Use Admin/Librarian key
4. ❌ Capital letters in header → Use `X-API-Key` not `x-api-key`

✅ Fix: Add header `X-API-Key: sk_admin_key_12345678901234567890` to write requests

---

## Need the Full Guide?

See: `SECURITY_USAGE_GUIDE.md` (detailed guide with all examples)

---

**You're all set! Your API is secure and ready to go!** 🚀
