using FluentValidation;
using SharedCodes = RadiologyCenter.BuildingBlocks.Application.Localization.ErrorCodes;

namespace RadiologyCenter.Insurance.Application.Commands.Policies.UploadPolicyDocument;

public class UploadPolicyDocumentCommandValidator : AbstractValidator<UploadPolicyDocumentCommand>
{
    public UploadPolicyDocumentCommandValidator()
    {
        RuleFor(x => x.PolicyId).NotEmpty().WithErrorCode(SharedCodes.Shared.IdRequired);
        RuleFor(x => x.Type).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired);
        RuleFor(x => x.FileName).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired).MaximumLength(255).WithErrorCode(SharedCodes.Shared.TextTooLong);
        RuleFor(x => x.ContentType).NotEmpty().WithErrorCode(SharedCodes.Shared.FieldRequired);
        RuleFor(x => x.SizeInBytes).GreaterThan(0).WithErrorCode(SharedCodes.Shared.ValueMustBePositive);
        RuleFor(x => x.Content).NotNull().WithErrorCode(SharedCodes.Shared.FieldRequired);
    }
}