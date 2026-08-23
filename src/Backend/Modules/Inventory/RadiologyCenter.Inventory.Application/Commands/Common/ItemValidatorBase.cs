using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Inventory.Domain.Enumerations;
using ErrorCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Inventory.Application.Commands.Common;

public abstract class ItemValidatorBase<T> : AbstractValidator<T> where T : IItemFields
{
    protected ItemValidatorBase()
    {
        RuleFor(x => x.Name).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired).MaximumLength(200).WithErrorCode(ErrorCodes.Shared.TextTooLong);
        RuleFor(x => x.Category).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired).IsEnumerationMember<ItemCategory, T>("Category");
        RuleFor(x => x.Unit).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired).IsEnumerationMember<UnitType, T>("Unit");
        RuleFor(x => x.Brand).MaximumLength(200).WithErrorCode(ErrorCodes.Shared.TextTooLong).When(x => !string.IsNullOrWhiteSpace(x.Brand));
        RuleFor(x => x.ReorderLevel).GreaterThanOrEqualTo(0).WithErrorCode(ErrorCodes.Shared.CannotBeNegative);
        RuleFor(x => x.ReorderQuantity).GreaterThanOrEqualTo(0).WithErrorCode(ErrorCodes.Shared.CannotBeNegative);
        RuleFor(x => x.StorageInstructions).MaximumLength(500).WithErrorCode(ErrorCodes.Shared.TextTooLong).When(x => !string.IsNullOrWhiteSpace(x.StorageInstructions));
    }
}
