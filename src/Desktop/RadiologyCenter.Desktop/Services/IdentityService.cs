using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Services;

public sealed class IdentityService
{
    private readonly ApiClient _api;

    public IdentityService(ApiClient api) => _api = api;

    public Task<PagedResult<UserDto>> GetUsersPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = new
        {
            pagination = new { pageNumber, pageSize },
            sortBy,
            sortDescending,
            searchTerm,
        };

        return _api.PostAsync<PagedResult<UserDto>>("api/users/all", query, ct);
    }

    public Task<UserDto> GetUserByIdAsync(string id, CancellationToken ct = default)
        => _api.GetAsync<UserDto>($"api/users/{id}", ct);

    public Task CreateUserAsync(CreateUserInput input, CancellationToken ct = default)
        => _api.PostAsync<object>("api/users", input, ct);

    public Task UpdateUserProfileAsync(string id, UpdateUserProfileInput input, CancellationToken ct = default)
        => _api.PutAsync<object>($"api/users/{id}/profile", input, ct);

    public Task ActivateUserAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/users/{id}/activate", ct: ct);

    public Task DeactivateUserAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/users/{id}/deactivate", ct: ct);

    public Task LockUserAsync(string id, DateTimeOffset lockoutEnd, CancellationToken ct = default)
        => _api.SendAsync($"api/users/{id}/lock", new { lockoutEnd }, ct);

    public Task UnlockUserAsync(string id, CancellationToken ct = default)
        => _api.SendAsync($"api/users/{id}/unlock", ct: ct);

    public Task ResetPasswordAsync(string id, string newPassword, CancellationToken ct = default)
        => _api.SendAsync($"api/users/{id}/reset-password", new { newPassword }, ct);

    public Task UpdateUserRolesAsync(string id, List<string> roleIds, CancellationToken ct = default)
        => _api.PutAsync<object>($"api/users/{id}/roles", new UpdateUserRolesInput { RoleIds = roleIds }, ct);

    public Task<PagedResult<RoleDto>> GetRolesPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = new
        {
            pagination = new { pageNumber, pageSize },
            sortBy,
            sortDescending,
            searchTerm,
        };

        return _api.PostAsync<PagedResult<RoleDto>>("api/roles/all", query, ct);
    }

    public Task<RoleDto> GetRoleByIdAsync(string id, CancellationToken ct = default)
        => _api.GetAsync<RoleDto>($"api/roles/{id}", ct);

    public Task CreateRoleAsync(CreateRoleInput input, CancellationToken ct = default)
        => _api.PostAsync<object>("api/roles", input, ct);

    public Task UpdateRoleAsync(string id, UpdateRoleInput input, CancellationToken ct = default)
        => _api.PutAsync<object>($"api/roles/{id}", input, ct);

    public Task AddPermissionToRoleAsync(string id, string permissionCode, CancellationToken ct = default)
        => _api.SendAsync($"api/roles/{id}/permissions", new { permissionCode }, ct);

    public Task RemovePermissionFromRoleAsync(string id, string permissionCode, CancellationToken ct = default)
        => _api.SendDeleteAsync($"api/roles/{id}/permissions/{permissionCode}", ct);

    public Task<IReadOnlyList<PermissionDto>> GetPermissionsAsync(CancellationToken ct = default)
        => _api.GetAsync<IReadOnlyList<PermissionDto>>("api/permissions", ct);
}
