using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.BuildingBlocks.Application.Common;
using RadiologyCenter.BuildingBlocks.Domain.Pagination;
using RadiologyCenter.BuildingBlocks.Domain.Results;

using RadiologyCenter.Clinic.Extensions;
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

namespace RadiologyCenter.Clinic.Operations.Resources;

public class ReferralDoctorsOperations : ClinicOperations
{
    private readonly IMessageBus _bus;

    public ReferralDoctorsOperations(IMessageBus bus) => _bus = bus;

    [ClinicPermission(ReferralDoctorsReadCode)]
    [ClinicOperation("GET", "resources/referral-doctors/{id:guid}")]
    public async Task<ClinicResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReferralDoctorDto>>(new GetReferralDoctorByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ReferralDoctorsReadCode)]
    [ClinicOperation("POST", "resources/referral-doctors/all")]
    public async Task<ClinicResult> GetAllAsync([FromBody] QueryRequest request, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<PagedResult<ReferralDoctorDto>>>(new GetReferralDoctorsQuery(request), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ReferralDoctorsCreateCode)]
    [ClinicOperation("POST", "resources/referral-doctors")]
    public async Task<ClinicResult> CreateAsync([FromBody] CreateReferralDoctorCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<ReferralDoctorDto>>(command, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ReferralDoctorsUpdateCode)]
    [ClinicOperation("PUT", "resources/referral-doctors/{id:guid}")]
    public async Task<ClinicResult> UpdateAsync(Guid id, [FromBody] UpdateReferralDoctorCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(command with { ReferralDoctorId = id }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ReferralDoctorsUpdateCode)]
    [ClinicOperation("POST", "resources/referral-doctors/{id:guid}/activate")]
    public async Task<ClinicResult> ActivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new ActivateReferralDoctorCommand(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ReferralDoctorsUpdateCode)]
    [ClinicOperation("POST", "resources/referral-doctors/{id:guid}/deactivate")]
    public async Task<ClinicResult> DeactivateAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeactivateReferralDoctorCommand(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(ReferralDoctorsDeleteCode)]
    [ClinicOperation("DELETE", "resources/referral-doctors/{id:guid}")]
    public async Task<ClinicResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeleteReferralDoctorCommand(id), ct);
        return result.ToActionResult();
    }
}
