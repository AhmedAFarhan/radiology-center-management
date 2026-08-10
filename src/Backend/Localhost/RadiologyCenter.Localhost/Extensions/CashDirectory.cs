using Microsoft.EntityFrameworkCore;
using RadiologyCenter.Cash.Application.Abstractions;
using RadiologyCenter.Identity.Infrastructure.Persistence;

namespace RadiologyCenter.Localhost.Extensions;

public class CashDirectory : ICashDirectory
{
    private readonly IdentityDbContext _identityDb;

    public CashDirectory(IdentityDbContext identityDb)
    {
        _identityDb = identityDb;
    }

    public async Task<IReadOnlyDictionary<Guid, string>> ResolveUserNamesAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0)
            return new Dictionary<Guid, string>();

        return await _identityDb.Users
            .Where(u => idList.Contains(u.Id))
            .Select(u => new { u.Id, u.FirstName, u.LastName })
            .ToListAsync(ct)
            .ContinueWith(t => t.Result.ToDictionary(
                u => u.Id,
                u => string.Join(' ', new[] { u.FirstName, u.LastName }.Where(part => !string.IsNullOrWhiteSpace(part)))));
    }
}