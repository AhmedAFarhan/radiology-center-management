using RadiologyCenter.Desktop.Models;

namespace RadiologyCenter.Desktop.Features.Identity.Services;

public sealed class IdentityService : CrudServiceBase
{
    private const string UsersRes = "api/users";
    private const string RolesRes = "api/roles";

    public IdentityService(ApiClient api) : base(api) { }

    public Task<PagedResult<UserDto>> GetUsersPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => FetchPageAsync<UserDto>(UsersRes, searchTerm, sortBy, sortDescending, pageNumber, pageSize, ct);

    public Task<UserDto> GetUserByIdAsync(string id, CancellationToken ct = default)
        => FetchByIdAsync<UserDto>(UsersRes, id, ct);

    public Task CreateUserAsync(CreateUserInput input, CancellationToken ct = default)
        => CreateEntityAsync<object>(UsersRes, input, ct);

    public Task UpdateUserProfileAsync(string id, UpdateUserProfileInput input, CancellationToken ct = default)
        => Api.PutAsync<object>($"{UsersRes}/{id}/profile", input, ct);

    public Task ActivateUserAsync(string id, CancellationToken ct = default)
        => SetEntityActiveAsync(UsersRes, id, true, ct);

    public Task DeactivateUserAsync(string id, CancellationToken ct = default)
        => SetEntityActiveAsync(UsersRes, id, false, ct);

    public Task LockUserAsync(string id, DateTimeOffset lockoutEnd, CancellationToken ct = default)
        => Api.SendAsync($"{UsersRes}/{id}/lock", new { lockoutEnd }, ct);

    public Task UnlockUserAsync(string id, CancellationToken ct = default)
        => Api.SendAsync($"{UsersRes}/{id}/unlock", ct: ct);

    public Task ResetPasswordAsync(string id, string newPassword, CancellationToken ct = default)
        => Api.SendAsync($"{UsersRes}/{id}/reset-password", new { newPassword }, ct);

    public Task UpdateUserRolesAsync(string id, List<string> roleIds, CancellationToken ct = default)
        => Api.PutAsync<object>($"{UsersRes}/{id}/roles", new UpdateUserRolesInput { RoleIds = roleIds }, ct);

    public Task<PagedResult<RoleDto>> GetRolesPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
        => FetchPageAsync<RoleDto>(RolesRes, searchTerm, sortBy, sortDescending, pageNumber, pageSize, ct);

    public Task<RoleDto> GetRoleByIdAsync(string id, CancellationToken ct = default)
        => FetchByIdAsync<RoleDto>(RolesRes, id, ct);

    public Task CreateRoleAsync(CreateRoleInput input, CancellationToken ct = default)
        => CreateEntityAsync<object>(RolesRes, input, ct);

    public Task UpdateRoleAsync(string id, UpdateRoleInput input, CancellationToken ct = default)
        => UpdateEntityAsync(RolesRes, id, input, ct);

    public Task AddPermissionToRoleAsync(string id, string permissionCode, CancellationToken ct = default)
        => Api.SendAsync($"{RolesRes}/{id}/permissions", new { permissionCode }, ct);

    public Task RemovePermissionFromRoleAsync(string id, string permissionCode, CancellationToken ct = default)
        => Api.SendDeleteAsync($"{RolesRes}/{id}/permissions/{permissionCode}", ct);

    public Task<IReadOnlyList<PermissionDto>> GetPermissionsAsync(CancellationToken ct = default)
        => Api.GetAsync<IReadOnlyList<PermissionDto>>("api/permissions", ct);
}
