using FluentValidation;
using RadiologyCenter.Inventory.Application.Commands.Common;

namespace RadiologyCenter.Inventory.Application.Commands.UpdateItem;

public class UpdateItemCommandValidator : ItemValidatorBase<UpdateItemCommand>
{
    public UpdateItemCommandValidator()
    {
        RuleFor(x => x.ItemId).NotEmpty();
    }
}
