using FluentValidation;
using RadiologyCenter.BuildingBlocks.Application.Validation;
using RadiologyCenter.Inventory.Domain.Enumerations;

namespace RadiologyCenter.Inventory.Application.Commands.Common;

public abstract class ItemValidatorBase<T> : AbstractValidator<T> where T : IItemFields
{
    protected ItemValidatorBase()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Category).NotEmpty().IsEnumerationMember<ItemCategory, T>("Category");
        RuleFor(x => x.Unit).NotEmpty().IsEnumerationMember<UnitType, T>("Unit");
        RuleFor(x => x.Brand).MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.Brand));
        RuleFor(x => x.ReorderLevel).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ReorderQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.StorageInstructions).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.StorageInstructions));
    }
}
