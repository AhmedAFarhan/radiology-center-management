using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Reports.Application.Queries.GetReportTemplateById;

public class GetReportTemplateByIdQueryValidator : AbstractValidator<GetReportTemplateByIdQuery>
{
    public GetReportTemplateByIdQueryValidator()
    {
        RuleFor(x => x.TemplateId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}