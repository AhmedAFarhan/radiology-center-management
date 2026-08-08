namespace RadiologyCenter.Insurance.Application.Commands.Companies.UpdateInsuranceCompany;

public record UpdateInsuranceCompanyCommand(
    Guid Id,
    string Name,
    string? TaxId = null,
    string? Address = null,
    string? Phone = null,
    string? Email = null) : ICommand;