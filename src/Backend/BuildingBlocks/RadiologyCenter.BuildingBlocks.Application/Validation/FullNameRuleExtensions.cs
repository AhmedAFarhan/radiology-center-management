using FluentValidation;
using RadiologyCenter.BuildingBlocks.Domain.Common;

namespace RadiologyCenter.BuildingBlocks.Application.Validation;

public static class FullNameRuleExtensions
{
    public static IRuleBuilderOptions<T, string> ContainsAtLeastTwoNameParts<T>(
        this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder.Must(PersonName.ContainsAtLeastTwoTokens)
            .WithMessage("Full name must contain at least a first name and a last name.");
}
