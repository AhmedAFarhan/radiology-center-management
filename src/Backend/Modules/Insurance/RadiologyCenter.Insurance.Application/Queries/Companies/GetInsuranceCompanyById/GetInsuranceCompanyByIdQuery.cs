namespace RadiologyCenter.Insurance.Application.Queries.Companies.GetInsuranceCompanyById;

public record GetInsuranceCompanyByIdQuery(Guid CompanyId) : IQuery;