using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Commands.Policies.DeletePolicyDocument;

public class DeletePolicyDocumentCommandValidator : AbstractValidator<DeletePolicyDocumentCommand>
{
    public DeletePolicyDocumentCommandValidator()
    {
        RuleFor(x => x.PolicyId).NotEmpty().WithErrorCode(ErrorCodes.PolicyIdRequired);
        RuleFor(x => x.DocumentId).NotEmpty().WithErrorCode(ErrorCodes.DocumentIdRequired);
    }
}
