using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using RadiologyCenter.BuildingBlocks.Application.Abstractions;
using RadiologyCenter.BuildingBlocks.Application.Common;

namespace RadiologyCenter.BuildingBlocks.Infrastructure.Services;

public class CurrentUserService : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor) =>
        _httpContextAccessor = httpContextAccessor;

    public string? Id =>
        _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? _httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value;

    public string? Name =>
        _httpContextAccessor.HttpContext?.User.Identity?.Name;

    public string? TimeZoneId =>
        _httpContextAccessor.HttpContext?.User.FindFirst("timezone")?.Value ?? TimezoneConstants.DefaultTimezone;

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}
