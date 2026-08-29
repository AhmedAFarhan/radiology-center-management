using FluentValidation;
using RadiologyCenter.Inventory.Application.Commands.Common;
using RadiologyCenter.Inventory.Application.Localization;

namespace RadiologyCenter.Inventory.Application.Commands.UpdateItem;

public class UpdateItemCommandValidator : ItemValidatorBase<UpdateItemCommand>
{
    public UpdateItemCommandValidator()
    {
        RuleFor(x => x.ItemId).NotEmpty().WithErrorCode(ErrorCodes.ItemIdRequired);
    }
}
