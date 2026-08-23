using FluentValidation;
using RadiologyCenter.Inventory.Application.Commands.Common;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Inventory.Application.Commands.UpdateItem;

public class UpdateItemCommandValidator : ItemValidatorBase<UpdateItemCommand>
{
    public UpdateItemCommandValidator()
    {
        RuleFor(x => x.ItemId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}
