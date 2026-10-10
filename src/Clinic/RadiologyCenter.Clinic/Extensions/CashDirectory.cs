using Microsoft.EntityFrameworkCore;
using RadiologyCenter.Cash.Application.Abstractions;
using RadiologyCenter.Identity.Infrastructure.Persistence;

namespace RadiologyCenter.Clinic.Extensions;

public class CashDirectory : ICashDirectory
{
    private readonly IdentityDbContext _identityDb;
    private readonly IConfiguration _configuration;

    public CashDirectory(IdentityDbContext identityDb,IConfiguration configuration)
    {
        _identityDb = identityDb;
        _configuration=configuration;
    }

    public async Task<IReadOnlyDictionary<Guid, string>> ResolveUserNamesAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0)
            return new Dictionary<Guid, string>();
        if(_configuration.GetValue<bool>("DatabaseAuthorization:Required"))
        {
            var users=await _identityDb.Database.SqlQuery<CashUser>($"SELECT Id,FirstName,LastName FROM [Security].[CashUsers]").ToListAsync(ct);
            return users.Where(u=>idList.Contains(u.Id)).ToDictionary(u=>u.Id,u=>u.FirstName+" "+u.LastName);
        }

        return await _identityDb.Users
            .Where(u => idList.Contains(u.Id))
            .Select(u => new { u.Id, u.FirstName, u.LastName })
            .ToListAsync(ct)
            .ContinueWith(t => t.Result.ToDictionary(
                u => u.Id,
                u => string.Join(' ', new[] { u.FirstName, u.LastName }.Where(part => !string.IsNullOrWhiteSpace(part)))));
    }
}
public sealed record CashUser(Guid Id,string FirstName,string LastName);
