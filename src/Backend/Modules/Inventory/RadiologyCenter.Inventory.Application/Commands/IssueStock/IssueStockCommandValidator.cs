using FluentValidation;
using RadiologyCenter.Inventory.Application.Localization;

namespace RadiologyCenter.Inventory.Application.Commands.IssueStock;

public class IssueStockCommandValidator : AbstractValidator<IssueStockCommand>
{
    public IssueStockCommandValidator()
    {
        RuleFor(x => x.ItemId).NotEmpty().WithErrorCode(ErrorCodes.ItemIdRequired);
        RuleFor(x => x.Quantity).GreaterThan(0).WithErrorCode(ErrorCodes.StockQuantityMustBePositive);
        RuleFor(x => x.Reference).MaximumLength(100).WithErrorCode(ErrorCodes.StockReferenceTooLong).When(x => !string.IsNullOrWhiteSpace(x.Reference));
        RuleFor(x => x.Notes).MaximumLength(500).WithErrorCode(ErrorCodes.StockNotesTooLong).When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}
