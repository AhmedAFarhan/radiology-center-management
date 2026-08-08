using Microsoft.AspNetCore.Mvc;
using RadiologyCenter.BuildingBlocks.Domain.Results;
using RadiologyCenter.Insurance.Application.Commands.Companies.CreateInsuranceCompany;
using RadiologyCenter.Insurance.Application.Commands.Companies.DeleteInsuranceCompany;
using RadiologyCenter.Insurance.Application.Commands.Companies.UpdateInsuranceCompany;
using RadiologyCenter.Insurance.Application.DTOs;
using RadiologyCenter.Insurance.Application.Queries.Companies.GetInsuranceCompanies;
using RadiologyCenter.Insurance.Application.Queries.Companies.GetInsuranceCompanyById;
using RadiologyCenter.Localhost.Authorization;
using RadiologyCenter.Localhost.Extensions;
using Wolverine;
using static RadiologyCenter.Identity.Domain.Permissions;

namespace RadiologyCenter.Localhost.Controllers.Insurance;

[ApiController]
[Route("api/insurance/companies")]
public class InsuranceCompaniesController : ControllerBase
{
    private readonly IMessageBus _bus;

    public InsuranceCompaniesController(IMessageBus bus) => _bus = bus;

    [HasPermission(InsuranceCompaniesReadCode)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<InsuranceCompanyDto>>(new GetInsuranceCompanyByIdQuery(id), ct);
        return result.ToActionResult();
    }

    [HasPermission(InsuranceCompaniesReadCode)]
    [HttpGet]
    public async Task<IActionResult> GetAllAsync(CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<IReadOnlyList<InsuranceCompanyDto>>>(new GetInsuranceCompaniesQuery(), ct);
        return result.ToActionResult();
    }

    [HasPermission(InsuranceCompaniesCreateCode)]
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateInsuranceCompanyCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<InsuranceCompanyDto>>(command, ct);
        return result.ToActionResult();
    }

    [HasPermission(InsuranceCompaniesUpdateCode)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateInsuranceCompanyCommand command, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result<InsuranceCompanyDto>>(command with { Id = id }, ct);
        return result.ToActionResult();
    }

    [HasPermission(InsuranceCompaniesDeleteCode)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var result = await _bus.InvokeAsync<Result>(new DeleteInsuranceCompanyCommand(id), ct);
        return result.ToActionResult();
    }
}