using FluentValidation;

namespace RadiologyCenter.Inventory.Application.Commands.IssueStock;

public class IssueStockCommandValidator : AbstractValidator<IssueStockCommand>
{
    public IssueStockCommandValidator()
    {
        RuleFor(x => x.ItemId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.Reference).MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.Reference));
        RuleFor(x => x.Notes).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}
