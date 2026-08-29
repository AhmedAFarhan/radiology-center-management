using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Inventory.Application.Localization;
using RadiologyCenter.Inventory.Domain.Enumerations;

namespace RadiologyCenter.Inventory.Application.Commands.Common;

public abstract class ItemValidatorBase<T> : AbstractValidator<T> where T : IItemFields
{
    protected ItemValidatorBase()
    {
        RuleFor(x => x.Name).NotEmpty().WithErrorCode(ErrorCodes.ItemNameRequired).MaximumLength(200).WithErrorCode(ErrorCodes.ItemNameTooLong);
        RuleFor(x => x.Category).NotEmpty().WithErrorCode(ErrorCodes.ItemCategoryRequired).IsEnumerationMember<ItemCategory, T>("Category");
        RuleFor(x => x.Unit).NotEmpty().WithErrorCode(ErrorCodes.ItemUnitRequired).IsEnumerationMember<UnitType, T>("Unit");
        RuleFor(x => x.Brand).MaximumLength(200).WithErrorCode(ErrorCodes.ItemBrandTooLong).When(x => !string.IsNullOrWhiteSpace(x.Brand));
        RuleFor(x => x.ReorderLevel).GreaterThanOrEqualTo(0).WithErrorCode(ErrorCodes.ReorderLevelCannotBeNegative);
        RuleFor(x => x.ReorderQuantity).GreaterThanOrEqualTo(0).WithErrorCode(ErrorCodes.ReorderQuantityCannotBeNegative);
        RuleFor(x => x.StorageInstructions).MaximumLength(500).WithErrorCode(ErrorCodes.StorageInstructionsTooLong).When(x => !string.IsNullOrWhiteSpace(x.StorageInstructions));
    }
}
