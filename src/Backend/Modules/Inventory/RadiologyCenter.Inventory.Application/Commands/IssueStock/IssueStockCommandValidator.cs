using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Inventory.Application.Commands.IssueStock;

public class IssueStockCommandValidator : AbstractValidator<IssueStockCommand>
{
    public IssueStockCommandValidator()
    {
        RuleFor(x => x.ItemId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.Quantity).GreaterThan(0).WithErrorCode(SharedCodes.Shared.ValueMustBePositive);
        RuleFor(x => x.Reference).MaximumLength(100).WithErrorCode(SharedCodes.Shared.TextTooLong).When(x => !string.IsNullOrWhiteSpace(x.Reference));
        RuleFor(x => x.Notes).MaximumLength(500).WithErrorCode(SharedCodes.Shared.TextTooLong).When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}
