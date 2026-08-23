using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Insurance.Application.Commands.Policies.DeletePolicyDocument;

public class DeletePolicyDocumentCommandValidator : AbstractValidator<DeletePolicyDocumentCommand>
{
    public DeletePolicyDocumentCommandValidator()
    {
        RuleFor(x => x.PolicyId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.DocumentId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
    }
}