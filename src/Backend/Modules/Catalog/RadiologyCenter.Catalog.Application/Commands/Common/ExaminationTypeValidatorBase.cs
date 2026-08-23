using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Catalog.Domain.Enumerations;
using ErrorCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Catalog.Application.Commands.Common;

public abstract class ExaminationTypeValidatorBase<T> : AbstractValidator<T>
    where T : IExaminationTypeFields
{
    protected ExaminationTypeValidatorBase()
    {
        RuleFor(x => x.Name).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired).MaximumLength(200).WithErrorCode(ErrorCodes.Shared.TextTooLong);
        RuleFor(x => x.Modality).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired).IsEnumerationMember<Modality, T>("Modality");
        RuleFor(x => x.BodyPart).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired).MaximumLength(200).WithErrorCode(ErrorCodes.Shared.TextTooLong);
        RuleFor(x => x.StandardDurationMinutes).GreaterThanOrEqualTo(0).WithErrorCode(ErrorCodes.Shared.CannotBeNegative);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0).WithErrorCode(ErrorCodes.Shared.CannotBeNegative);
    }
}