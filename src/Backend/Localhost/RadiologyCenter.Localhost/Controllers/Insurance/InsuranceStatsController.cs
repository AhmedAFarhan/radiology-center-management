using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Insurance.Application.DTOs;
using RadiologyCenter.Insurance.Application.Queries.Stats.GetInsuranceStats;
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Localhost.Controllers.Insurance;

[ApiController]
[Route("api/insurance")]
public class InsuranceStatsController : ControllerBase
{
    private readonly IMessageBus _bus;

    public InsuranceStatsController(IMessageBus bus) => _bus = bus;

    [HasPermission(InsuranceClaimsReadCode)]
    [HttpGet("stats")]
    public async Task<IActionResult> GetStatsAsync(CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<InsuranceStatsDto>>(new GetInsuranceStatsQuery(), ct);
        return result.ToActionResult();
    }
}