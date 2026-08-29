using FluentValidation;
using RadiologyCenter.Reports.Application.Localization;

namespace RadiologyCenter.Reports.Application.Queries.GetReportByExamination;

public class GetReportByExaminationQueryValidator : AbstractValidator<GetReportByExaminationQuery>
{
    public GetReportByExaminationQueryValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty().WithErrorCode(ErrorCodes.ExaminationIdRequired);
    }
}