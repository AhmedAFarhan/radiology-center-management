using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Identity.Application.DTOs;
using RadiologyCenter.Identity.Domain;
using RadiologyCenter.Identity.Domain.Entities;
using RadiologyCenter.Identity.Infrastructure.Persistence;
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Localhost.Controllers.Identity;

[ApiController]
[Route("api/permissions")]
public class PermissionsController : ControllerBase
{
    private const string FallbackLanguage = "en";

    private readonly IdentityDbContext _db;

    public PermissionsController(IdentityDbContext db) => _db = db;

    [HasPermission(RolesReadCode)]
    [HttpGet]
    public async Task<IActionResult> GetAllAsync(CancellationToken ct)
    {
        var language = ResolveLanguage();

        var permissions = await _db.Permissions
            .Include(p => p.Translations)
            .OrderBy(p => p.Group)
            .ThenBy(p => p.Name)
            .ToListAsync(ct);

        var items = permissions
            .Select(p => ToDto(p, language))
            .ToList();

        return Result.Success<IReadOnlyList<PermissionDto>>(items).ToActionResult();
    }

    private static PermissionDto ToDto(Permission permission, string language)
    {
        var translation = permission.GetTranslation(language)
            ?? permission.GetTranslation(FallbackLanguage);

        return new PermissionDto(
            permission.Code,
            translation?.Name ?? permission.Name,
            translation?.Description ?? permission.Description,
            translation?.Group ?? permission.Group);
    }

    private static string ResolveLanguage()
    {
        var name = CultureInfo.CurrentUICulture.Name;
        return name.Split('-')[0].ToLowerInvariant();
    }
}