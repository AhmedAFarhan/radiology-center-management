using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RadiologyCenter.Catalog.Domain.Entities;
using RadiologyCenter.Catalog.Infrastructure.Persistence;
using RadiologyCenter.Identity.Domain.Entities;
using RadiologyCenter.Identity.Infrastructure.Persistence;
using RadiologyCenter.Insurance.Domain.Entities;
using RadiologyCenter.Insurance.Infrastructure.Persistence;
using RadiologyCenter.Inventory.Domain.Entities;
using RadiologyCenter.Inventory.Infrastructure.Persistence;
using RadiologyCenter.Patients.Domain.Entities;
using RadiologyCenter.Patients.Infrastructure.Persistence;
using RadiologyCenter.ResourceManagement.Domain.Entities;
using RadiologyCenter.ResourceManagement.Infrastructure.Persistence;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Localhost.Services.GlobalSearch;

public sealed record GlobalSearchItemDto(Guid Id, string Title, string? Subtitle, int Score);

public sealed record GlobalSearchGroupDto(string EntityType, IReadOnlyList<GlobalSearchItemDto> Items, int Count);

public sealed class GlobalSearchService
{
    private const int DefaultLimit = 5;
    private const int MaxLimit = 10;
    private const int CandidateFactor = 4;
    private const int MaxCandidates = 50;

    private readonly IServiceScopeFactory _scopeFactory;

    public GlobalSearchService(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;

    public async Task<IReadOnlyList<GlobalSearchGroupDto>> SearchAsync(ClaimsPrincipal user, string? q, int limit, CancellationToken ct)
    {
        var term = (q ?? string.Empty).Trim();
        if (term.Length < 2)
            return Array.Empty<GlobalSearchGroupDto>();

        var take = Math.Clamp(limit, 1, MaxLimit);
        var words = SearchTerm.Parse(term);

        var tasks = new List<Task<GlobalSearchGroupDto>>();

        if (Can(user, PatientsReadCode))
            tasks.Add(SearchPatientsAsync(words, take, ct));

        if (Can(user, StaffReadCode))
            tasks.Add(SearchStaffAsync(words, take, ct));

        if (Can(user, ReferralDoctorsReadCode))
            tasks.Add(SearchReferralDoctorsAsync(words, take, ct));

        if (Can(user, InventoryItemsReadCode))
            tasks.Add(SearchItemsAsync(words, take, ct));

        if (Can(user, InventorySuppliersReadCode))
            tasks.Add(SearchSuppliersAsync(words, take, ct));

        if (Can(user, InsuranceCompaniesReadCode))
            tasks.Add(SearchInsuranceCompaniesAsync(words, take, ct));

        if (Can(user, InsurancePoliciesReadCode))
            tasks.Add(SearchInsurancePoliciesAsync(words, take, ct));

        if (Can(user, UsersReadCode))
            tasks.Add(SearchUsersAsync(words, take, ct));

        if (Can(user, ExaminationsTypesManageCode))
            tasks.Add(SearchExaminationTypesAsync(words, take, ct));

        // Each search runs on its own DbContext scope, so parallel execution is safe.
        var groups = await Task.WhenAll(tasks);

        return groups
            .Where(g => g.Items.Count > 0)
            .OrderBy(g => OrderIndex(g.EntityType))
            .ToList();
    }

    private static bool Can(ClaimsPrincipal user, string permission)
        => user.HasClaim("isAdmin", "true") || user.HasClaim("permission", permission);

    private static int OrderIndex(string entityType)
        => entityType switch
        {
            "patient" => 0,
            "staff" => 1,
            "referralDoctor" => 2,
            "item" => 3,
            "supplier" => 4,
            "insuranceCompany" => 5,
            "insurancePolicy" => 6,
            "user" => 7,
            "examinationType" => 8,
            _ => 9,
        };

    private async Task<GlobalSearchGroupDto> SearchPatientsAsync(SearchTerm term, int take, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PatientsDbContext>();
        var filter = SearchPredicates.Build<Patient>(term,
            p => p.PatientCode, p => p.FirstName, p => p.MiddleName, p => p.LastName, p => p.PhoneNumber, p => p.NationalId);

        var query = db.Patients.Where(filter);
        var count = await query.CountAsync(ct);

        var rows = await query
            .OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
            .Take(Math.Min(take * CandidateFactor, MaxCandidates))
            .Select(p => new PatientRow(p.Id, p.FirstName, p.MiddleName, p.LastName, p.PatientCode))
            .ToListAsync(ct);

        var items = rows
            .Select(r => new ScoredItem(
                r.Id,
                JoinNames(r.FirstName, r.MiddleName, r.LastName),
                r.PatientCode,
                term.Score(r.FirstName, r.MiddleName, r.LastName, r.PatientCode)))
            .OrderByDescending(x => x.Score).ThenBy(x => x.Title)
            .Take(take)
            .Select(x => new GlobalSearchItemDto(x.Id, x.Title, x.Subtitle, x.Score))
            .ToList();

        return new GlobalSearchGroupDto("patient", items, count);
    }

    private async Task<GlobalSearchGroupDto> SearchStaffAsync(SearchTerm term, int take, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ResourceManagementDbContext>();
        var filter = SearchPredicates.Build<Staff>(term,
            s => s.FirstName, s => s.MiddleName, s => s.LastName, s => s.PhoneNumber, s => s.LicenseNumber, s => s.Department, s => s.Specialization);

        var query = db.Staff.Where(filter);
        var count = await query.CountAsync(ct);

        var rows = await query
            .OrderBy(s => s.LastName).ThenBy(s => s.FirstName)
            .Take(Math.Min(take * CandidateFactor, MaxCandidates))
            .Select(s => new StaffRow(s.Id, s.FirstName, s.MiddleName, s.LastName, s.PhoneNumber))
            .ToListAsync(ct);

        var items = rows
            .Select(r => new ScoredItem(
                r.Id,
                JoinNames(r.FirstName, r.MiddleName, r.LastName),
                r.PhoneNumber,
                term.Score(r.FirstName, r.MiddleName, r.LastName, r.PhoneNumber)))
            .OrderByDescending(x => x.Score).ThenBy(x => x.Title)
            .Take(take)
            .Select(x => new GlobalSearchItemDto(x.Id, x.Title, x.Subtitle, x.Score))
            .ToList();

        return new GlobalSearchGroupDto("staff", items, count);
    }

    private async Task<GlobalSearchGroupDto> SearchReferralDoctorsAsync(SearchTerm term, int take, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ResourceManagementDbContext>();
        var filter = SearchPredicates.Build<ReferralDoctor>(term,
            d => d.FirstName, d => d.MiddleName, d => d.LastName, d => d.Phone, d => d.Hospital, d => d.Specialization);

        var query = db.ReferralDoctors.Where(filter);
        var count = await query.CountAsync(ct);

        var rows = await query
            .OrderBy(d => d.LastName).ThenBy(d => d.FirstName)
            .Take(Math.Min(take * CandidateFactor, MaxCandidates))
            .Select(d => new ReferralDoctorRow(d.Id, d.FirstName, d.MiddleName, d.LastName, d.Phone))
            .ToListAsync(ct);

        var items = rows
            .Select(r => new ScoredItem(
                r.Id,
                JoinNames(r.FirstName, r.MiddleName, r.LastName),
                r.Phone,
                term.Score(r.FirstName, r.MiddleName, r.LastName, r.Phone)))
            .OrderByDescending(x => x.Score).ThenBy(x => x.Title)
            .Take(take)
            .Select(x => new GlobalSearchItemDto(x.Id, x.Title, x.Subtitle, x.Score))
            .ToList();

        return new GlobalSearchGroupDto("referralDoctor", items, count);
    }

    private async Task<GlobalSearchGroupDto> SearchItemsAsync(SearchTerm term, int take, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
        var filter = SearchPredicates.Build<Item>(term, i => i.Name, i => i.Brand);

        var query = db.Items.Where(filter);
        var count = await query.CountAsync(ct);

        var rows = await query
            .OrderBy(i => i.Name)
            .Take(Math.Min(take * CandidateFactor, MaxCandidates))
            .Select(i => new ItemRow(i.Id, i.Name, i.Brand))
            .ToListAsync(ct);

        var items = rows
            .Select(r => new ScoredItem(r.Id, r.Name, r.Brand, term.Score(r.Name, r.Brand)))
            .OrderByDescending(x => x.Score).ThenBy(x => x.Title)
            .Take(take)
            .Select(x => new GlobalSearchItemDto(x.Id, x.Title, x.Subtitle, x.Score))
            .ToList();

        return new GlobalSearchGroupDto("item", items, count);
    }

    private async Task<GlobalSearchGroupDto> SearchSuppliersAsync(SearchTerm term, int take, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
        var filter = SearchPredicates.Build<Supplier>(term, s => s.Name, s => s.ContactPerson, s => s.Phone, s => s.Email);

        var query = db.Suppliers.Where(filter);
        var count = await query.CountAsync(ct);

        var rows = await query
            .OrderBy(s => s.Name)
            .Take(Math.Min(take * CandidateFactor, MaxCandidates))
            .Select(s => new SupplierRow(s.Id, s.Name, s.Phone))
            .ToListAsync(ct);

        var items = rows
            .Select(r => new ScoredItem(r.Id, r.Name, r.Phone, term.Score(r.Name, r.Phone)))
            .OrderByDescending(x => x.Score).ThenBy(x => x.Title)
            .Take(take)
            .Select(x => new GlobalSearchItemDto(x.Id, x.Title, x.Subtitle, x.Score))
            .ToList();

        return new GlobalSearchGroupDto("supplier", items, count);
    }

    private async Task<GlobalSearchGroupDto> SearchInsuranceCompaniesAsync(SearchTerm term, int take, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<InsuranceDbContext>();
        var filter = SearchPredicates.Build<InsuranceCompany>(term, c => c.Name, c => c.TaxId, c => c.Phone, c => c.Email);

        var query = db.InsuranceCompanies.Where(filter);
        var count = await query.CountAsync(ct);

        var rows = await query
            .OrderBy(c => c.Name)
            .Take(Math.Min(take * CandidateFactor, MaxCandidates))
            .Select(c => new InsuranceCompanyRow(c.Id, c.Name, c.Phone))
            .ToListAsync(ct);

        var items = rows
            .Select(r => new ScoredItem(r.Id, r.Name, r.Phone, term.Score(r.Name, r.Phone)))
            .OrderByDescending(x => x.Score).ThenBy(x => x.Title)
            .Take(take)
            .Select(x => new GlobalSearchItemDto(x.Id, x.Title, x.Subtitle, x.Score))
            .ToList();

        return new GlobalSearchGroupDto("insuranceCompany", items, count);
    }

    private async Task<GlobalSearchGroupDto> SearchInsurancePoliciesAsync(SearchTerm term, int take, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<InsuranceDbContext>();
        var filter = SearchPredicates.Build<InsurancePolicy>(term, p => p.PolicyNumber);

        var query = db.InsurancePolicies.Where(filter);
        var count = await query.CountAsync(ct);

        var rows = await query
            .OrderBy(p => p.PolicyNumber)
            .Take(Math.Min(take * CandidateFactor, MaxCandidates))
            .Select(p => new InsurancePolicyRow(p.Id, p.PolicyNumber))
            .ToListAsync(ct);

        var items = rows
            .Select(r => new ScoredItem(r.Id, r.PolicyNumber, null, term.Score(r.PolicyNumber)))
            .OrderByDescending(x => x.Score).ThenBy(x => x.Title)
            .Take(take)
            .Select(x => new GlobalSearchItemDto(x.Id, x.Title, x.Subtitle, x.Score))
            .ToList();

        return new GlobalSearchGroupDto("insurancePolicy", items, count);
    }

    private async Task<GlobalSearchGroupDto> SearchUsersAsync(SearchTerm term, int take, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        var filter = SearchPredicates.Build<User>(term, u => u.UserName, u => u.FirstName, u => u.LastName, u => u.Email, u => u.PhoneNumber);

        var query = db.Users.Where(filter);
        var count = await query.CountAsync(ct);

        var rows = await query
            .OrderBy(u => u.UserName)
            .Take(Math.Min(take * CandidateFactor, MaxCandidates))
            .Select(u => new UserRow(u.Id, u.UserName, u.FirstName, u.LastName, u.Email))
            .ToListAsync(ct);

        var items = rows
            .Select(r => new ScoredItem(
                r.Id,
                FullUserName(r.FirstName, r.LastName, r.UserName),
                r.UserName,
                term.Score(r.FirstName, r.LastName, r.UserName, r.Email)))
            .OrderByDescending(x => x.Score).ThenBy(x => x.Title)
            .Take(take)
            .Select(x => new GlobalSearchItemDto(x.Id, x.Title, x.Subtitle, x.Score))
            .ToList();

        return new GlobalSearchGroupDto("user", items, count);
    }

    private async Task<GlobalSearchGroupDto> SearchExaminationTypesAsync(SearchTerm term, int take, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        var filter = SearchPredicates.Build<ExaminationType>(term, t => t.Name, t => t.Code, t => t.BodyPart);

        var query = db.ExaminationTypes.Where(filter);
        var count = await query.CountAsync(ct);

        var rows = await query
            .OrderBy(t => t.Name)
            .Take(Math.Min(take * CandidateFactor, MaxCandidates))
            .Select(t => new ExaminationTypeRow(t.Id, t.Name, t.Code))
            .ToListAsync(ct);

        var items = rows
            .Select(r => new ScoredItem(r.Id, r.Name, r.Code, term.Score(r.Name, r.Code)))
            .OrderByDescending(x => x.Score).ThenBy(x => x.Title)
            .Take(take)
            .Select(x => new GlobalSearchItemDto(x.Id, x.Title, x.Subtitle, x.Score))
            .ToList();

        return new GlobalSearchGroupDto("examinationType", items, count);
    }

    private static string JoinNames(string first, string? middle, string last)
        => string.Join(' ', new[] { first, middle, last }.Where(s => !string.IsNullOrWhiteSpace(s)));

    private static string FullUserName(string? first, string? last, string userName)
        => string.IsNullOrWhiteSpace($"{first} {last}".Trim()) ? userName : $"{first} {last}".Trim();

    private sealed record ScoredItem(Guid Id, string Title, string? Subtitle, int Score);

    private sealed record PatientRow(Guid Id, string FirstName, string? MiddleName, string LastName, string PatientCode);
    private sealed record StaffRow(Guid Id, string FirstName, string? MiddleName, string LastName, string? PhoneNumber);
    private sealed record ReferralDoctorRow(Guid Id, string FirstName, string? MiddleName, string LastName, string? Phone);
    private sealed record ItemRow(Guid Id, string Name, string? Brand);
    private sealed record SupplierRow(Guid Id, string Name, string? Phone);
    private sealed record InsuranceCompanyRow(Guid Id, string Name, string? Phone);
    private sealed record InsurancePolicyRow(Guid Id, string PolicyNumber);
    private sealed record UserRow(Guid Id, string UserName, string? FirstName, string? LastName, string? Email);
    private sealed record ExaminationTypeRow(Guid Id, string Name, string? Code);
}

/// <summary>Splits a raw query into words and handles Arabic normalization + relevance scoring.</summary>
internal sealed class SearchTerm
{
    private readonly IReadOnlyList<string> _rawWords;

    private SearchTerm(IReadOnlyList<string> rawWords) => _rawWords = rawWords;

    public static SearchTerm Parse(string term)
        => new(term.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));

    public IReadOnlyList<string> RawWords => _rawWords;

    /// <summary>All LIKE patterns (raw + normalized variant) for a given word, already escaped and wrapped in %.</summary>
    public IReadOnlyList<string> WordPatterns(string word)
    {
        var raw = $"%{EscapeLike(word)}%";
        var normalized = ArabicSearch.Normalize(word);
        var norm = $"%{EscapeLike(normalized)}%";
        return normalized == word ? new[] { raw } : new[] { raw, norm };
    }

    public int Score(params string?[] haystacks)
    {
        var total = 0;
        foreach (var haystack in haystacks)
        {
            if (string.IsNullOrWhiteSpace(haystack))
                continue;

            var normalized = ArabicSearch.Normalize(haystack).ToLowerInvariant();
            foreach (var rawWord in _rawWords)
            {
                var word = ArabicSearch.Normalize(rawWord).ToLowerInvariant();
                if (string.IsNullOrEmpty(word))
                    continue;

                if (string.Equals(normalized, word, StringComparison.Ordinal))
                    total += 40;
                else if (normalized.StartsWith(word, StringComparison.Ordinal))
                    total += 30;
                else if (StartsWithWord(normalized, word))
                    total += 20;
                else if (normalized.Contains(word, StringComparison.Ordinal))
                    total += 10;
            }
        }

        return total;
    }

    private static bool StartsWithWord(string text, string word)
        => text.Length > word.Length && text[0] == ' ' && text.StartsWith(" " + word, StringComparison.Ordinal)
           || text.IndexOf(" " + word, StringComparison.Ordinal) >= 0;

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

/// <summary>Builds a translatable EF predicate: every word must match, each word matches any field via any pattern variant.</summary>
internal static class SearchPredicates
{
    public static Expression<Func<TEntity, bool>> Build<TEntity>(SearchTerm term, params Expression<Func<TEntity, string?>>[] fields)
    {
        var likeMethod = typeof(DbFunctionsExtensions).GetMethods()
            .First(m => m.Name == nameof(DbFunctionsExtensions.Like)
                && m.GetParameters() is { Length: 3 } p
                && p[1].ParameterType == typeof(string)
                && p[2].ParameterType == typeof(string));

        var dbFunctions = Expression.Constant(EF.Functions);
        var parameter = Expression.Parameter(typeof(TEntity), "e");
        var rebinder = new ParameterRebinder(parameter);

        Expression? result = null;

        foreach (var rawWord in term.RawWords)
        {
            Expression? wordExpr = null;

            foreach (var field in fields)
            {
                Expression? fieldExpr = null;

                foreach (var pattern in term.WordPatterns(rawWord))
                {
                    var likeCall = Expression.Call(
                        likeMethod,
                        dbFunctions,
                        rebinder.Visit(field.Body),
                        Expression.Constant(pattern));
                    fieldExpr = fieldExpr is null ? likeCall : Expression.OrElse(fieldExpr, likeCall);
                }

                if (fieldExpr is null)
                    continue;

                wordExpr = wordExpr is null ? fieldExpr : Expression.OrElse(wordExpr, fieldExpr);
            }

            if (wordExpr is null)
                continue;

            result = result is null ? wordExpr : Expression.AndAlso(result, wordExpr);
        }

        if (result is null)
            return _ => true;

        return Expression.Lambda<Func<TEntity, bool>>(result, parameter);
    }

    private sealed class ParameterRebinder : ExpressionVisitor
    {
        private readonly ParameterExpression _target;

        public ParameterRebinder(ParameterExpression target) => _target = target;

        protected override Expression VisitParameter(ParameterExpression node)
            => node.Type == _target.Type ? _target : base.VisitParameter(node);
    }
}

/// <summary>Arabic text normalization: strips diacritics and folds common letter variants.</summary>
internal static class ArabicSearch
{
    public static string Normalize(string text)
    {
        var sb = new StringBuilder(text.Length);

        foreach (var ch in text)
        {
            var c = ch;
            if (c >= '\u064B' && c <= '\u0652' || c == '\u0670' || c == '\u0640')
                continue;

            c = c switch
            {
                '\u0622' or '\u0623' or '\u0625' or '\u0671' => '\u0627', // آ أ إ ٱ → ا
                '\u0649' => '\u064A',                                     // ى → ي
                '\u0629' => '\u0647',                                     // ة → ه
                _ => c,
            };

            sb.Append(c);
        }

        return sb.ToString();
    }
}
