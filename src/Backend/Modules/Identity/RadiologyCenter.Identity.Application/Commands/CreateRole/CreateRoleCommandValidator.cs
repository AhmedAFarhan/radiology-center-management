using FluentValidation;
using ErrorCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Identity.Application.Commands.CreateRole;

public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator() => RuleFor(x => x.Name).NotEmpty().WithErrorCode(ErrorCodes.Shared.FieldRequired).MaximumLength(100).WithErrorCode(ErrorCodes.Shared.TextTooLong);
}
