using FluentValidation;

namespace RadiologyCenter.Examinations.Application.Queries.GetExaminationTypeById;

public class GetExaminationTypeByIdQueryValidator : AbstractValidator<GetExaminationTypeByIdQuery>
{
    public GetExaminationTypeByIdQueryValidator() => RuleFor(x => x.Id).NotEmpty();
}
