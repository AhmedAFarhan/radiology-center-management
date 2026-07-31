using FluentValidation;
using RadiologyCenter.Inventory.Domain.Enumerations;

namespace RadiologyCenter.Inventory.Application.Commands.UpdateItem;

public class UpdateItemCommandValidator : AbstractValidator<UpdateItemCommand>
{
    public UpdateItemCommandValidator()
    {
        RuleFor(x => x.ItemId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Category).NotEmpty().Must(IsValidCategory)
            .WithMessage("Category must be one of: ContrastMedia, Drug, MedicalSupply, Consumable.");
        RuleFor(x => x.Unit).NotEmpty().Must(IsValidUnit)
            .WithMessage("Unit must be one of: Piece, Box, Bottle, Vial, Ampoule, Pack, Tube, Roll, Sheet, Kit.");
        RuleFor(x => x.Brand).MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.Brand));
        RuleFor(x => x.ReorderLevel).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ReorderQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.StorageInstructions).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.StorageInstructions));
    }

    private static bool IsValidCategory(string category) =>
        ItemCategory.GetAll<ItemCategory>().Any(c => c.Name.Equals(category, StringComparison.OrdinalIgnoreCase));

    private static bool IsValidUnit(string unit) =>
        UnitType.GetAll<UnitType>().Any(u => u.Name.Equals(unit, StringComparison.OrdinalIgnoreCase));
}
