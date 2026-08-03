using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
using RadiologyCenter.ResourceManagement.Application.Commands.ActivateReferralDoctor;
using RadiologyCenter.ResourceManagement.Application.Commands.CreateReferralDoctor;
using RadiologyCenter.ResourceManagement.Application.Commands.DeactivateReferralDoctor;
using RadiologyCenter.ResourceManagement.Application.Commands.DeleteReferralDoctor;
using RadiologyCenter.ResourceManagement.Application.Commands.UpdateReferralDoctor;
using RadiologyCenter.ResourceManagement.Application.DTOs;
using RadiologyCenter.ResourceManagement.Application.Queries.GetReferralDoctorById;
using RadiologyCenter.ResourceManagement.Application.Queries.GetReferralDoctors;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Localhost.Controllers.Resources;

[ApiController]
[Route("api/resources/referral-doctors")]
public class ReferralDoctorsController : ControllerBase
{
    private readonly IMessageBus _bus;

    public ReferralDoctorsController(IMessageBus bus) => _bus = bus;

    [HasPermission(ReferralDoctorsReadCode)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReferralDoctorDto>>(new GetReferralDoctorByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(ReferralDoctorsReadCode)]
    [HttpPost("all")]
    public async Task<IActionResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<ReferralDoctorDto>>>(new GetReferralDoctorsQuery(request), ct);
        return result.ToActionResult();
    }

    [HasPermission(ReferralDoctorsCreateCode)]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateReferralDoctorCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReferralDoctorDto>>(command, ct);
        return result.ToActionResult();
    }

    [HasPermission(ReferralDoctorsUpdateCode)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateReferralDoctorCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { ReferralDoctorId = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(ReferralDoctorsUpdateCode)]
    [HttpPost("{id:guid}/activate")]
    public async Task<IActionResult> ActivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new ActivateReferralDoctorCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(ReferralDoctorsUpdateCode)]
    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeactivateReferralDoctorCommand(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(ReferralDoctorsDeleteCode)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeleteReferralDoctorCommand(id), ct);
        return result.ToActionResult();
    }
}
