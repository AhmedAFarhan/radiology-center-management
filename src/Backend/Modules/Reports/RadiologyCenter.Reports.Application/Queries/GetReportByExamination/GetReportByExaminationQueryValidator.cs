using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Reports.Application.Queries.GetReportByExamination;

public class GetReportByExaminationQueryValidator : AbstractValidator<GetReportByExaminationQuery>
{
    public GetReportByExaminationQueryValidator()
    {
        RuleFor(x => x.ExaminationId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}