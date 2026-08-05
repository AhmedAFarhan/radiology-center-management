using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Examinations.Application.DTOs;
using RadiologyCenter.Examinations.Application.Queries.GetFinancialAnalytics;
using RadiologyCenter.Examinations.Application.Queries.GetFinancialExams;
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Localhost.Controllers.Analytics;

[ApiController]
[Route("api/analytics")]
public class AnalyticsController : ControllerBase
{
    private readonly IMessageBus _bus;

    public AnalyticsController(IMessageBus bus) => _bus = bus;

    [HasPermission(AnalyticsReadCode)]
    [HttpGet("financial")]
    public async Task<IActionResult> GetFinancialAsync(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<FinancialAnalyticsDto>>(new GetFinancialAnalyticsQuery(from, to), ct);
        return result.ToActionResult();
    }

    [HasPermission(AnalyticsReadCode)]
    [HttpGet("financial/exams")]
    public async Task<IActionResult> GetFinancialExamsAsync(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<IReadOnlyList<FinancialExamRowDto>>>(new GetFinancialExamsQuery(from, to), ct);
        return result.ToActionResult();
    }
}