using FluentValidation;

namespace RadiologyCenter.Insurance.Application.Commands.PreAuthorizations.UploadPreAuthorizationDocument;

public class UploadPreAuthorizationDocumentCommandValidator : AbstractValidator<UploadPreAuthorizationDocumentCommand>
{
    public UploadPreAuthorizationDocumentCommandValidator()
    {
        RuleFor(x => x.PreAuthorizationId).NotEmpty();
        RuleFor(x => x.Type).NotEmpty();
        RuleFor(x => x.FileName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.ContentType).NotEmpty();
        RuleFor(x => x.SizeInBytes).GreaterThan(0);
        RuleFor(x => x.Content).NotNull();
    }
}