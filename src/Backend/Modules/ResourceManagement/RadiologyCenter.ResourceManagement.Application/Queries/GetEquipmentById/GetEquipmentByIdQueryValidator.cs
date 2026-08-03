using FluentValidation;

namespace RadiologyCenter.ResourceManagement.Application.Queries.GetEquipmentById;

public class GetEquipmentByIdQueryValidator : AbstractValidator<GetEquipmentByIdQuery>
{
    public GetEquipmentByIdQueryValidator() => RuleFor(x => x.Id).NotEmpty();
}
