using FluentValidation;

namespace RadiologyCenter.Inventory.Application.Commands.CreatePurchaseOrder;

public class CreatePurchaseOrderCommandValidator : AbstractValidator<CreatePurchaseOrderCommand>
{
    public CreatePurchaseOrderCommandValidator()
    {
        RuleFor(x => x.SupplierId).NotEmpty();
        RuleFor(x => x.Items).NotEmpty().WithMessage("A purchase order must contain at least one item.");
        RuleFor(x => x.Items).Must(HaveNoDuplicateItems)
            .WithMessage("An item can appear only once per purchase order.");
        RuleForEach(x => x.Items).ChildRules(line =>
        {
            line.RuleFor(i => i.ItemId).NotEmpty();
            line.RuleFor(i => i.QuantityOrdered).GreaterThan(0);
            line.RuleFor(i => i.UnitCost).GreaterThanOrEqualTo(0);
        });
        RuleFor(x => x.Notes).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }

    private static bool HaveNoDuplicateItems(List<PurchaseOrderLineInput>? items) =>
        items is null || items.Select(i => i.ItemId).Distinct().Count() == items.Count;
}
