using System.Globalization;
using RadiologyCenter.Clinic.Dispatching;
using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Identity.Application.DTOs;
using RadiologyCenter.Identity.Domain;
using RadiologyCenter.Identity.Domain.Entities;
using RadiologyCenter.Identity.Infrastructure.Persistence;

using RadiologyCenter.Clinic.Extensions;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Clinic.Operations.Identity;

public class PermissionsOperations : ClinicOperations
{
    private const string FallbackLanguage = "en";

    private readonly IdentityDbContext _db;

    public PermissionsOperations(IdentityDbContext db) => _db = db;

    [ClinicPermission(RolesReadCode)]
    [ClinicOperation("GET", "permissions")]
    public async Task<ClinicResult> GetAllAsync(CancellationToken ct)
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