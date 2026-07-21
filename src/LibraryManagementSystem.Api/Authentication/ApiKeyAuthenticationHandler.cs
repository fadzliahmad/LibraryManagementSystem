using System.Security.Claims;
using System.Text.Encodings.Web;
using LibraryManagementSystem.Core.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LibraryManagementSystem.Api.Authentication;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationSchemeOptions>
{
    private readonly IApiKeyService _apiKeyService;
    private const string ApiKeyHeaderName = "X-API-Key";

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationSchemeOptions> options,
        ILoggerFactory loggerFactory,
        UrlEncoder encoder,
        IApiKeyService apiKeyService)
        : base(options, loggerFactory, encoder)
    {
        _apiKeyService = apiKeyService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyHeaderValues))
        {
            return AuthenticateResult.NoResult();
        }

        var apiKey = apiKeyHeaderValues.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return AuthenticateResult.NoResult();
        }

        var identity = await _apiKeyService.ValidateApiKeyAsync(apiKey);
        if (identity == null)
        {
            return AuthenticateResult.Fail("Invalid or expired API key.");
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, identity.ApiKey),
            new Claim(ClaimTypes.Name, identity.Name),
            new Claim(ClaimTypes.Role, identity.Role.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(claimsIdentity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}

public class ApiKeyAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
}
