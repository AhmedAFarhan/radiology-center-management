using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Features.Auth.Services;

public sealed class AuthService
{
    private readonly ApiClient _api;
    private readonly BusinessRuntimeService _business;
    private readonly AppAuthenticationStateProvider _authState;
    private readonly TokenStorage _tokenStorage;

    public AuthService(ApiClient api, AppAuthenticationStateProvider authState, TokenStorage tokenStorage, BusinessRuntimeService business)
    {
        _api = api;
        _business = business;
        _authState = authState;
        _tokenStorage = tokenStorage;
    }

    public async Task<bool> SignInAsync(string userName, string password, CancellationToken ct = default)
    {
        var tokens = _business.IsDevelopment
            ? await _api.PostAsync<TokenResult>("api/auth/login", new { userName, password }, ct)
            : await _business.SignInAsync(userName,password,ct);

        await _authState.SignInAsync(AuthTokens.From(tokens, userName));

        return tokens.MustChangePassword;
    }

    public async Task ChangePasswordAsync(string currentPassword, string newPassword, CancellationToken ct = default)
    {
        var tokens = _business.IsDevelopment
            ? await _api.PostAsync<TokenResult>("api/auth/change-password",new { currentPassword, newPassword },ct)
            : await _business.ChangePasswordAsync(currentPassword,newPassword,ct);

        var current = _tokenStorage.GetTokens();
        await _authState.SignInAsync(AuthTokens.From(tokens, current?.Username ?? string.Empty));
    }

    public async Task SignOutAsync(CancellationToken ct = default)
    {
        var tokens = _tokenStorage.GetTokens();
        if (tokens is not null && _business.IsDevelopment)
        {
            try
            {
                await _api.SendAsync("api/auth/logout", new { refreshToken = tokens.RefreshToken }, ct);
            }
            catch
            {
                // best-effort server-side revoke
            }
        }

        await _business.SignOutAsync();
        await _authState.SignOutAsync();
    }
}

