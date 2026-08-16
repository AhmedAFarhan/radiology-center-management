using FluentValidation;
using RadiologyCenter.Inventory.Application.Localization;

namespace RadiologyCenter.Inventory.Application.Commands.ReceivePurchaseOrder;

public class ReceivePurchaseOrderCommandValidator : AbstractValidator<ReceivePurchaseOrderCommand>
{
    public ReceivePurchaseOrderCommandValidator()
    {
        RuleFor(x => x.PurchaseOrderId).NotEmpty();
        RuleFor(x => x.Lines).NotEmpty().WithErrorCode(ErrorCodes.ReceiveItemsRequired);
        RuleFor(x => x.Lines).Must(HaveNoDuplicateItems)
            .WithErrorCode(ErrorCodes.ReceiveDuplicateItems);
        RuleForEach(x => x.Lines).ChildRules(line =>
        {
            line.RuleFor(i => i.ItemId).NotEmpty();
            line.RuleFor(i => i.Quantity).GreaterThan(0);
            line.RuleFor(i => i.LotNumber).NotEmpty().MaximumLength(100);
            line.RuleFor(i => i.ExpiryDate).Must(e => e is null || e.Value.Date >= DateTime.UtcNow.Date)
                .WithErrorCode(ErrorCodes.ExpiryDatePast);
        });
    }

    private static bool HaveNoDuplicateItems(List<ReceivePurchaseOrderLineInput>? lines) =>
        lines is null || lines.Select(i => i.ItemId).Distinct().Count() == lines.Count;
}
