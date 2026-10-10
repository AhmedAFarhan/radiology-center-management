using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Insurance.Application.DTOs;
using RadiologyCenter.Insurance.Application.Queries.Stats.GetInsuranceStats;

using RadiologyCenter.Clinic.Extensions;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Clinic.Operations.Insurance;

public class InsuranceStatsOperations : ClinicOperations
{
    private readonly IMessageBus _bus;

    public InsuranceStatsOperations(IMessageBus bus) => _bus = bus;

    [ClinicPermission(InsuranceClaimsReadCode)]
    [ClinicOperation("GET", "insurance/stats")]
    public async Task<ClinicResult> GetStatsAsync(CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<InsuranceStatsDto>>(new GetInsuranceStatsQuery(), ct);
        return result.ToActionResult();
    }
}