using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Commands.Policies.DeletePolicyDocument;

public class DeletePolicyDocumentCommandValidator : AbstractValidator<DeletePolicyDocumentCommand>
{
    public DeletePolicyDocumentCommandValidator()
    {
        RuleFor(x => x.PolicyId).NotEmpty();
        RuleFor(x => x.DocumentId).NotEmpty();
    }
}