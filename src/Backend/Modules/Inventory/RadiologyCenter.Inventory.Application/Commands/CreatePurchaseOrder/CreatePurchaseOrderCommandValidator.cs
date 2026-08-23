using FluentValidation;
using RadiologyCenter.Inventory.Application.Localization;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Inventory.Application.Commands.CreatePurchaseOrder;

public class CreatePurchaseOrderCommandValidator : AbstractValidator<CreatePurchaseOrderCommand>
{
    public CreatePurchaseOrderCommandValidator()
    {
        RuleFor(x => x.SupplierId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.Items).NotEmpty().WithErrorCode(ErrorCodes.PurchaseOrderItemsRequired);
        RuleFor(x => x.Items).Must(HaveNoDuplicateItems)
            .WithErrorCode(ErrorCodes.PurchaseOrderDuplicateItems);
        RuleForEach(x => x.Items).ChildRules(line =>
        {
            line.RuleFor(i => i.ItemId).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired);
            line.RuleFor(i => i.QuantityOrdered).GreaterThan(0).WithErrorCode(SharedCodes.Shared.ValueMustBePositive);
            line.RuleFor(i => i.UnitCost).GreaterThanOrEqualTo(0).WithErrorCode(SharedCodes.Shared.CannotBeNegative);
        });
        RuleFor(x => x.Notes).MaximumLength(500).WithErrorCode(SharedCodes.Shared.TextTooLong).When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }

    private static bool HaveNoDuplicateItems(List<PurchaseOrderLineInput>? items) =>
        items is null || items.Select(i => i.ItemId).Distinct().Count() == items.Count;
}
