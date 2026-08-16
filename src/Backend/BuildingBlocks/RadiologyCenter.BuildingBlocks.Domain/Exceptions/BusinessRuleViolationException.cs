namespace RadiologyCenter.BuildingBlocks.Domain.Exceptions;

public class BusinessRuleViolationException : DomainException
{
    public string Rule { get; }

    public BusinessRuleViolationException(string rule, string message) : base(message)
    {
        Rule = rule;
    }

    public BusinessRuleViolationException(string message) : base(message)
    {
        Rule = GetType().Name;
    }

    public BusinessRuleViolationException(string rule, string code, string message) : base(code, message)
    {
        Rule = rule;
    }
}
