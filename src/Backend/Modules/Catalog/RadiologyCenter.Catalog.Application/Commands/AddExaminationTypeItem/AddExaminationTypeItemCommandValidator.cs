using FluentValidation;

namespace RadiologyCenter.Catalog.Application.Commands.AddExaminationTypeItem;

public class AddExaminationTypeItemCommandValidator : AbstractValidator<AddExaminationTypeItemCommand>
{
    public AddExaminationTypeItemCommandValidator()
    {
        RuleFor(x => x.ExaminationTypeId).NotEmpty();
        RuleFor(x => x.ItemId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.Notes).MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Notes));
    }
}
