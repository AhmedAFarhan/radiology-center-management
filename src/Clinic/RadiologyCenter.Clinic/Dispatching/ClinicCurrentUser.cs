using System.Security.Claims;
using RadiologyCenter.BuildingBlocks.Application.Abstractions;

namespace RadiologyCenter.Clinic.Dispatching;

public sealed class ClinicPrincipalAccessor
{
    private readonly AsyncLocal<ClaimsPrincipal?> _current = new();
    public ClaimsPrincipal? Current { get => _current.Value; set => _current.Value = value; }
}

public sealed class ClinicCurrentUser(ClinicPrincipalAccessor accessor) : ICurrentUser
{
    public ClaimsPrincipal Principal
    {
        get => accessor.Current ?? new ClaimsPrincipal(new ClaimsIdentity());
        internal set => accessor.Current = value;
    }
    public string? Id => Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Principal.FindFirst("sub")?.Value;
    public string? Name => Principal.FindFirst(ClaimTypes.Name)?.Value ?? Principal.FindFirst("unique_name")?.Value;
    public string? TimeZoneId => Principal.FindFirst("timezone")?.Value ?? "Africa/Cairo";
    public bool IsAuthenticated => Principal.Identity?.IsAuthenticated ?? false;
    public bool IsAdministrator => IsAuthenticated && Principal.HasClaim("isAdmin", "true");
    public bool HasPermission(string permissionCode) => IsAuthenticated && (IsAdministrator || Principal.HasClaim("permission", permissionCode));
}
