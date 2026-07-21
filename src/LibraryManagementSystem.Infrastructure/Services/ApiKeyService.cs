using LibraryManagementSystem.Core.Interfaces;
using LibraryManagementSystem.Core.Models;
using Microsoft.Extensions.Configuration;

namespace LibraryManagementSystem.Infrastructure.Services;

public class ApiKeyService : IApiKeyService
{
    private readonly IConfiguration _configuration;
    private readonly List<ApiKeyIdentity> _apiKeys;

    public ApiKeyService(IConfiguration configuration)
    {
        _configuration = configuration;
        _apiKeys = LoadApiKeysFromConfig();
    }

    public Task<ApiKeyIdentity?> ValidateApiKeyAsync(string apiKey)
    {
        var key = _apiKeys.FirstOrDefault(k => 
            k.ApiKey == apiKey && 
            k.IsActive && 
            (k.ExpiresAt == null || k.ExpiresAt > DateTime.UtcNow));

        return Task.FromResult(key);
    }

    public Task<IEnumerable<ApiKeyIdentity>> GetAllKeysAsync()
    {
        return Task.FromResult(_apiKeys.AsEnumerable());
    }

    public Task<ApiKeyIdentity> CreateApiKeyAsync(string name, UserRole role, DateTime? expiresAt = null)
    {
        var newKey = new ApiKeyIdentity
        {
            ApiKey = GenerateApiKey(),
            Name = name,
            Role = role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt
        };

        _apiKeys.Add(newKey);
        return Task.FromResult(newKey);
    }

    public Task<bool> RevokeApiKeyAsync(string apiKey)
    {
        var key = _apiKeys.FirstOrDefault(k => k.ApiKey == apiKey);
        if (key != null)
        {
            key.IsActive = false;
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public bool CanPerformAction(UserRole role, string action)
    {
        // Permission matrix: which roles can perform which actions
        var permissions = new Dictionary<UserRole, List<string>>
        {
            { UserRole.Admin, new List<string> { "create", "read", "update", "delete" } },
            { UserRole.Librarian, new List<string> { "create", "read", "update", "delete" } },
            { UserRole.Member, new List<string> { "read" } }
        };

        if (!permissions.TryGetValue(role, out var allowedActions))
            return false;

        return allowedActions.Contains(action.ToLower());
    }

    private List<ApiKeyIdentity> LoadApiKeysFromConfig()
    {
        var keys = new List<ApiKeyIdentity>();
        var apiKeysSection = _configuration.GetSection("ApiKeys");

        if (apiKeysSection.Exists())
        {
            foreach (var keyConfig in apiKeysSection.GetChildren())
            {
                var key = new ApiKeyIdentity
                {
                    ApiKey = keyConfig["Key"] ?? string.Empty,
                    Name = keyConfig["Name"] ?? string.Empty,
                    Role = Enum.Parse<UserRole>(keyConfig["Role"] ?? "Member"),
                    IsActive = bool.TryParse(keyConfig["IsActive"], out var isActive) ? isActive : true,
                    CreatedAt = DateTime.TryParse(keyConfig["CreatedAt"], out var createdAt) ? createdAt : DateTime.UtcNow,
                    ExpiresAt = DateTime.TryParse(keyConfig["ExpiresAt"], out var expiresAt) ? expiresAt : null
                };
                keys.Add(key);
            }
        }

        return keys;
    }

    private static string GenerateApiKey()
    {
        return $"sk_{Guid.NewGuid().ToString("N").Substring(0, 24)}";
    }
}
