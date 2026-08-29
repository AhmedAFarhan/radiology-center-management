using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Commands.Policies.UploadPolicyDocument;

public class UploadPolicyDocumentCommandValidator : AbstractValidator<UploadPolicyDocumentCommand>
{
    public UploadPolicyDocumentCommandValidator()
    {
        RuleFor(x => x.PolicyId).NotEmpty().WithErrorCode(ErrorCodes.PolicyIdRequired);
        RuleFor(x => x.Type).NotEmpty().WithErrorCode(ErrorCodes.DocumentTypeRequired);
        RuleFor(x => x.FileName).NotEmpty().WithErrorCode(ErrorCodes.FileNameRequired).MaximumLength(255).WithErrorCode(ErrorCodes.FileNameTooLong);
        RuleFor(x => x.ContentType).NotEmpty().WithErrorCode(ErrorCodes.ContentTypeRequired);
        RuleFor(x => x.SizeInBytes).GreaterThan(0).WithErrorCode(ErrorCodes.SizeMustBePositive);
        RuleFor(x => x.Content).NotNull().WithErrorCode(ErrorCodes.ContentRequired);
    }
}
