using FluentValidation;

namespace RadiologyCenter.Reports.Application.Queries.GetReportByExamination;

public class GetReportByExaminationQueryValidator : AbstractValidator<GetReportByExaminationQuery>
{
    public GetReportByExaminationQueryValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty();
    }
}