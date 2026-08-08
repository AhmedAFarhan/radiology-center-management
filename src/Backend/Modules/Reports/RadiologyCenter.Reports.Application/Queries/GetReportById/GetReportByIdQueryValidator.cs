using FluentValidation;

namespace RadiologyCenter.Reports.Application.Queries.GetReportById;

public class GetReportByIdQueryValidator : AbstractValidator<GetReportByIdQuery>
{
    public GetReportByIdQueryValidator()
    {
        RuleFor(x => x.ReportId).NotEmpty();
    }
}