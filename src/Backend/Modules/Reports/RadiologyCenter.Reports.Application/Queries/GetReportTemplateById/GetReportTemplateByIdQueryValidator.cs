using FluentValidation;
using RadiologyCenter.Reports.Application.Localization;

namespace RadiologyCenter.Reports.Application.Queries.GetReportTemplateById;

public class GetReportTemplateByIdQueryValidator : AbstractValidator<GetReportTemplateByIdQuery>
{
    public GetReportTemplateByIdQueryValidator()
    {
        RuleFor(x => x.TemplateId).NotEmpty().WithErrorCode(ErrorCodes.TemplateIdRequired);
    }
}