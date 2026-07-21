using LibraryManagementSystem.Core.Models;

namespace LibraryManagementSystem.Core.Interfaces;

public interface IApiKeyService
{
    /// <summary>
    /// Validates an API key and returns the associated identity if valid.
    /// </summary>
    Task<ApiKeyIdentity?> ValidateApiKeyAsync(string apiKey);

    /// <summary>
    /// Gets all registered API keys (admin only).
    /// </summary>
    Task<IEnumerable<ApiKeyIdentity>> GetAllKeysAsync();

    /// <summary>
    /// Creates a new API key.
    /// </summary>
    Task<ApiKeyIdentity> CreateApiKeyAsync(string name, UserRole role, DateTime? expiresAt = null);

    /// <summary>
    /// Revokes an API key by marking it as inactive.
    /// </summary>
    Task<bool> RevokeApiKeyAsync(string apiKey);

    /// <summary>
    /// Checks if a user role can perform a specific action.
    /// </summary>
    bool CanPerformAction(UserRole role, string action);
}
