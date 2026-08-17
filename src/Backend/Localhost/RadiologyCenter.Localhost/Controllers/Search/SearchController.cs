using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Catalog.Infrastructure.Persistence;
using RadiologyCenter.Identity.Infrastructure.Persistence;
using RadiologyCenter.Insurance.Infrastructure.Persistence;
using RadiologyCenter.Inventory.Infrastructure.Persistence;
using RadiologyCenter.Localhost.Extensions;
using RadiologyCenter.Patients.Infrastructure.Persistence;
using RadiologyCenter.ResourceManagement.Infrastructure.Persistence;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Localhost.Controllers.Search;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    private const int DefaultLimit = 5;
    private const int MaxLimit = 10;

    private readonly PatientsDbContext _patients;
    private readonly ResourceManagementDbContext _resources;
    private readonly InventoryDbContext _inventory;
    private readonly InsuranceDbContext _insurance;
    private readonly IdentityDbContext _identity;
    private readonly CatalogDbContext _catalog;

    public SearchController(
        PatientsDbContext patients,
        ResourceManagementDbContext resources,
        InventoryDbContext inventory,
        InsuranceDbContext insurance,
        IdentityDbContext identity,
        CatalogDbContext catalog)
    {
        _patients = patients;
        _resources = resources;
        _inventory = inventory;
        _insurance = insurance;
        _identity = identity;
        _catalog = catalog;
    }

    [HttpGet]
    public async Task<IActionResult> SearchAsync([FromQuery] string? q, [FromQuery] int limit = DefaultLimit, CancellationToken ct = default)
    {
        var term = (q ?? string.Empty).Trim();
        if (term.Length < 2)
            return Result.Success<IReadOnlyList<GlobalSearchGroupDto>>(new List<GlobalSearchGroupDto>()).ToActionResult();

        var take = Math.Clamp(limit, 1, MaxLimit);
        var like = "%" + EscapeLike(term) + "%";

        // Run sequentially: several searches share the same scoped DbContext
        // (staff/referralDoctors, items/suppliers, companies/policies) and EF Core
        // forbids concurrent operations on one context instance.
        var groups = new List<GlobalSearchGroupDto>();

        if (Can(PatientsReadCode))
        {
            var g = await SearchPatientsAsync(like, take, ct);
            if (g.Items.Count > 0) groups.Add(g);
        }

        if (Can(StaffReadCode))
        {
            var g = await SearchStaffAsync(like, take, ct);
            if (g.Items.Count > 0) groups.Add(g);
        }

        if (Can(ReferralDoctorsReadCode))
        {
            var g = await SearchReferralDoctorsAsync(like, take, ct);
            if (g.Items.Count > 0) groups.Add(g);
        }

        if (Can(InventoryItemsReadCode))
        {
            var g = await SearchItemsAsync(like, take, ct);
            if (g.Items.Count > 0) groups.Add(g);
        }

        if (Can(InventorySuppliersReadCode))
        {
            var g = await SearchSuppliersAsync(like, take, ct);
            if (g.Items.Count > 0) groups.Add(g);
        }

        if (Can(InsuranceCompaniesReadCode))
        {
            var g = await SearchInsuranceCompaniesAsync(like, take, ct);
            if (g.Items.Count > 0) groups.Add(g);
        }

        if (Can(InsurancePoliciesReadCode))
        {
            var g = await SearchInsurancePoliciesAsync(like, take, ct);
            if (g.Items.Count > 0) groups.Add(g);
        }

        if (Can(UsersReadCode))
        {
            var g = await SearchUsersAsync(like, take, ct);
            if (g.Items.Count > 0) groups.Add(g);
        }

        if (Can(ExaminationsTypesManageCode))
        {
            var g = await SearchExaminationTypesAsync(like, take, ct);
            if (g.Items.Count > 0) groups.Add(g);
        }

        return Result.Success<IReadOnlyList<GlobalSearchGroupDto>>(groups).ToActionResult();
    }

    private bool Can(string permission)
        => User.HasClaim("isAdmin", "true") || User.HasClaim("permission", permission);

    private async Task<GlobalSearchGroupDto> SearchPatientsAsync(string like, int take, CancellationToken ct)
    {
        var rows = await _patients.Patients
            .Where(p => EF.Functions.Like(p.PatientCode, like)
                     || EF.Functions.Like(p.FirstName, like)
                     || EF.Functions.Like(p.MiddleName, like)
                     || EF.Functions.Like(p.LastName, like)
                     || EF.Functions.Like(p.PhoneNumber, like)
                     || EF.Functions.Like(p.NationalId, like))
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Take(take)
            .Select(p => new { p.Id, p.FirstName, p.MiddleName, p.LastName, p.PatientCode })
            .ToListAsync(ct);

        var items = rows
            .Select(p => new GlobalSearchItemDto(
                p.Id,
                JoinNames(p.FirstName, p.MiddleName, p.LastName),
                p.PatientCode))
            .ToList();

        return new GlobalSearchGroupDto("patient", items);
    }

    private async Task<GlobalSearchGroupDto> SearchStaffAsync(string like, int take, CancellationToken ct)
    {
        var rows = await _resources.Staff
            .Where(s => EF.Functions.Like(s.FirstName, like)
                     || EF.Functions.Like(s.MiddleName, like)
                     || EF.Functions.Like(s.LastName, like)
                     || EF.Functions.Like(s.PhoneNumber, like)
                     || EF.Functions.Like(s.LicenseNumber, like)
                     || EF.Functions.Like(s.Department, like)
                     || EF.Functions.Like(s.Specialization, like))
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .Take(take)
            .Select(s => new { s.Id, s.FirstName, s.MiddleName, s.LastName, s.PhoneNumber })
            .ToListAsync(ct);

        var items = rows
            .Select(s => new GlobalSearchItemDto(s.Id, JoinNames(s.FirstName, s.MiddleName, s.LastName), s.PhoneNumber))
            .ToList();

        return new GlobalSearchGroupDto("staff", items);
    }

    private async Task<GlobalSearchGroupDto> SearchReferralDoctorsAsync(string like, int take, CancellationToken ct)
    {
        var rows = await _resources.ReferralDoctors
            .Where(d => EF.Functions.Like(d.FirstName, like)
                     || EF.Functions.Like(d.MiddleName, like)
                     || EF.Functions.Like(d.LastName, like)
                     || EF.Functions.Like(d.Phone, like)
                     || EF.Functions.Like(d.Hospital, like)
                     || EF.Functions.Like(d.Specialization, like))
            .OrderBy(d => d.LastName)
            .ThenBy(d => d.FirstName)
            .Take(take)
            .Select(d => new { d.Id, d.FirstName, d.MiddleName, d.LastName, d.Phone })
            .ToListAsync(ct);

        var items = rows
            .Select(d => new GlobalSearchItemDto(d.Id, JoinNames(d.FirstName, d.MiddleName, d.LastName), d.Phone))
            .ToList();

        return new GlobalSearchGroupDto("referralDoctor", items);
    }

    private async Task<GlobalSearchGroupDto> SearchItemsAsync(string like, int take, CancellationToken ct)
    {
        var rows = await _inventory.Items
            .Where(i => EF.Functions.Like(i.Name, like) || EF.Functions.Like(i.Brand, like))
            .OrderBy(i => i.Name)
            .Take(take)
            .Select(i => new { i.Id, i.Name, i.Brand })
            .ToListAsync(ct);

        var items = rows
            .Select(i => new GlobalSearchItemDto(i.Id, i.Name, i.Brand))
            .ToList();

        return new GlobalSearchGroupDto("item", items);
    }

    private async Task<GlobalSearchGroupDto> SearchSuppliersAsync(string like, int take, CancellationToken ct)
    {
        var rows = await _inventory.Suppliers
            .Where(s => EF.Functions.Like(s.Name, like)
                     || EF.Functions.Like(s.ContactPerson, like)
                     || EF.Functions.Like(s.Phone, like)
                     || EF.Functions.Like(s.Email, like))
            .OrderBy(s => s.Name)
            .Take(take)
            .Select(s => new { s.Id, s.Name, s.Phone })
            .ToListAsync(ct);

        var items = rows
            .Select(s => new GlobalSearchItemDto(s.Id, s.Name, s.Phone))
            .ToList();

        return new GlobalSearchGroupDto("supplier", items);
    }

    private async Task<GlobalSearchGroupDto> SearchInsuranceCompaniesAsync(string like, int take, CancellationToken ct)
    {
        var rows = await _insurance.InsuranceCompanies
            .Where(c => EF.Functions.Like(c.Name, like)
                     || EF.Functions.Like(c.TaxId, like)
                     || EF.Functions.Like(c.Phone, like)
                     || EF.Functions.Like(c.Email, like))
            .OrderBy(c => c.Name)
            .Take(take)
            .Select(c => new { c.Id, c.Name, c.Phone })
            .ToListAsync(ct);

        var items = rows
            .Select(c => new GlobalSearchItemDto(c.Id, c.Name, c.Phone))
            .ToList();

        return new GlobalSearchGroupDto("insuranceCompany", items);
    }

    private async Task<GlobalSearchGroupDto> SearchInsurancePoliciesAsync(string like, int take, CancellationToken ct)
    {
        var rows = await _insurance.InsurancePolicies
            .Where(p => EF.Functions.Like(p.PolicyNumber, like))
            .OrderBy(p => p.PolicyNumber)
            .Take(take)
            .Select(p => new { p.Id, p.PolicyNumber })
            .ToListAsync(ct);

        var items = rows
            .Select(p => new GlobalSearchItemDto(p.Id, p.PolicyNumber, null))
            .ToList();

        return new GlobalSearchGroupDto("insurancePolicy", items);
    }

    private async Task<GlobalSearchGroupDto> SearchUsersAsync(string like, int take, CancellationToken ct)
    {
        var rows = await _identity.Users
            .Where(u => EF.Functions.Like(u.UserName, like)
                     || EF.Functions.Like(u.FirstName, like)
                     || EF.Functions.Like(u.LastName, like)
                     || EF.Functions.Like(u.Email, like)
                     || EF.Functions.Like(u.PhoneNumber, like))
            .OrderBy(u => u.UserName)
            .Take(take)
            .Select(u => new { u.Id, u.UserName, u.FirstName, u.LastName, u.Email })
            .ToListAsync(ct);

        var items = rows
            .Select(u => new GlobalSearchItemDto(
                u.Id,
                string.IsNullOrWhiteSpace($"{u.FirstName} {u.LastName}".Trim())
                    ? u.UserName
                    : $"{u.FirstName} {u.LastName}".Trim(),
                u.UserName))
            .ToList();

        return new GlobalSearchGroupDto("user", items);
    }

    private async Task<GlobalSearchGroupDto> SearchExaminationTypesAsync(string like, int take, CancellationToken ct)
    {
        var rows = await _catalog.ExaminationTypes
            .Where(t => EF.Functions.Like(t.Name, like)
                     || EF.Functions.Like(t.Code, like)
                     || EF.Functions.Like(t.BodyPart, like))
            .OrderBy(t => t.Name)
            .Take(take)
            .Select(t => new { t.Id, t.Name, t.Code })
            .ToListAsync(ct);

        var items = rows
            .Select(t => new GlobalSearchItemDto(t.Id, t.Name, t.Code))
            .ToList();

        return new GlobalSearchGroupDto("examinationType", items);
    }

    private static string JoinNames(string first, string? middle, string last)
        => string.Join(' ', new[] { first, middle, last }.Where(s => !string.IsNullOrWhiteSpace(s)));

    private static string EscapeLike(string term)
    {
        var sb = new StringBuilder(term.Length + 8);
        foreach (var ch in term)
        {
            if (ch is '%' or '_' or '[' or ']' or '\\')
                sb.Append('\\').Append(ch);
            else
                sb.Append(ch);
        }

        return sb.ToString();
    }
}

public sealed record GlobalSearchItemDto(Guid Id, string Title, string? Subtitle);

public sealed record GlobalSearchGroupDto(string EntityType, IReadOnlyList<GlobalSearchItemDto> Items);