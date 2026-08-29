using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Catalog.Application.Localization;
using RadiologyCenter.Catalog.Domain.Enumerations;

namespace RadiologyCenter.Catalog.Application.Commands.Common;

public abstract class ExaminationTypeValidatorBase<T> : AbstractValidator<T>
    where T : IExaminationTypeFields
{
    protected ExaminationTypeValidatorBase()
    {
        RuleFor(x => x.Name).NotEmpty().WithErrorCode(ErrorCodes.ExaminationTypeNameRequired).MaximumLength(200).WithErrorCode(ErrorCodes.ExaminationTypeNameTooLong);
        RuleFor(x => x.Modality).NotEmpty().WithErrorCode(ErrorCodes.ExaminationTypeModalityRequired).IsEnumerationMember<Modality, T>("Modality", ErrorCodes.ExaminationTypeModalityInvalid);
        RuleFor(x => x.BodyPart).NotEmpty().WithErrorCode(ErrorCodes.ExaminationTypeBodyPartRequired).MaximumLength(200).WithErrorCode(ErrorCodes.ExaminationTypeBodyPartTooLong);
        RuleFor(x => x.StandardDurationMinutes).GreaterThanOrEqualTo(0).WithErrorCode(ErrorCodes.ExaminationTypeDurationCannotBeNegative);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0).WithErrorCode(ErrorCodes.ExaminationTypePriceCannotBeNegative);
    }
}
