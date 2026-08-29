using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Commands.PreAuthorizations.UploadPreAuthorizationDocument;

public class UploadPreAuthorizationDocumentCommandValidator : AbstractValidator<UploadPreAuthorizationDocumentCommand>
{
    public UploadPreAuthorizationDocumentCommandValidator()
    {
        RuleFor(x => x.PreAuthorizationId).NotEmpty().WithErrorCode(ErrorCodes.PreAuthorizationIdRequired);
        RuleFor(x => x.Type).NotEmpty().WithErrorCode(ErrorCodes.DocumentTypeRequired);
        RuleFor(x => x.FileName).NotEmpty().WithErrorCode(ErrorCodes.FileNameRequired).MaximumLength(255).WithErrorCode(ErrorCodes.FileNameTooLong);
        RuleFor(x => x.ContentType).NotEmpty().WithErrorCode(ErrorCodes.ContentTypeRequired);
        RuleFor(x => x.SizeInBytes).GreaterThan(0).WithErrorCode(ErrorCodes.SizeMustBePositive);
        RuleFor(x => x.Content).NotNull().WithErrorCode(ErrorCodes.ContentRequired);
    }
}
