namespace RadiologyCenter.Insurance.Application.Commands.Companies.CreateInsuranceCompany;

public record CreateInsuranceCompanyCommand(
    string Name,
    string? TaxId = null,
    string? Address = null,
    string? Phone = null,
    string? Email = null) : ICommand;