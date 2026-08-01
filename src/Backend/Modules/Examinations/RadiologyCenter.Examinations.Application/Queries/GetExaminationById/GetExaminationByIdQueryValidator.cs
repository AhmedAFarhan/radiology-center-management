using FluentValidation;

namespace RadiologyCenter.Examinations.Application.Queries.GetExaminationById;

public class GetExaminationByIdQueryValidator : AbstractValidator<GetExaminationByIdQuery>
{
    public GetExaminationByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
