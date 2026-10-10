using RadiologyCenter.Clinic.Dispatching;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Insurance.Application.Commands.Companies.CreateInsuranceCompany;
using RadiologyCenter.Insurance.Application.Commands.Companies.DeleteInsuranceCompany;
using RadiologyCenter.Insurance.Application.Commands.Companies.UpdateInsuranceCompany;
using RadiologyCenter.Insurance.Application.DTOs;
using RadiologyCenter.Insurance.Application.Queries.Companies.GetInsuranceCompanies;
using RadiologyCenter.Insurance.Application.Queries.Companies.GetInsuranceCompanyById;

using RadiologyCenter.Clinic.Extensions;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Clinic.Operations.Insurance;

public class InsuranceCompaniesOperations : ClinicOperations
{
    private readonly IMessageBus _bus;

    public InsuranceCompaniesOperations(IMessageBus bus) => _bus = bus;

    [ClinicPermission(InsuranceCompaniesReadCode)]
    [ClinicOperation("GET", "insurance/companies/{id:guid}")]
    public async Task<ClinicResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<InsuranceCompanyDto>>(new GetInsuranceCompanyByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InsuranceCompaniesReadCode)]
    [ClinicOperation("GET", "insurance/companies")]
    public async Task<ClinicResult> GetAllAsync(CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<IReadOnlyList<InsuranceCompanyDto>>>(new GetInsuranceCompaniesQuery(), ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InsuranceCompaniesCreateCode)]
    [ClinicOperation("POST", "insurance/companies")]
    public async Task<ClinicResult> CreateAsync([FromBody] CreateInsuranceCompanyCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<InsuranceCompanyDto>>(command, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InsuranceCompaniesUpdateCode)]
    [ClinicOperation("PUT", "insurance/companies/{id:guid}")]
    public async Task<ClinicResult> UpdateAsync(Guid id, [FromBody] UpdateInsuranceCompanyCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<InsuranceCompanyDto>>(command with { Id = id }, ct);
        return result.ToActionResult();
    }

    [ClinicPermission(InsuranceCompaniesDeleteCode)]
    [ClinicOperation("DELETE", "insurance/companies/{id:guid}")]
    public async Task<ClinicResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeleteInsuranceCompanyCommand(id), ct);
        return result.ToActionResult();
    }
}