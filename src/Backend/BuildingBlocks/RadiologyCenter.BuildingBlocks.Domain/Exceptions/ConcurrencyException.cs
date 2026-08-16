using RadiologyCenter.BuildingBlocks.Domain.Localization;

namespace RadiologyCenter.BuildingBlocks.Domain.Exceptions;

public class ConcurrencyException : DomainException
{
    public ConcurrencyException(string message) : base(message) { }

    public ConcurrencyException(string code, string message) : base(code, message) { }

    public ConcurrencyException(string message, Exception innerException)
        : base(MessageCodes.Shared.ConcurrencyConflict, message, innerException) { }

    public ConcurrencyException(string code, string message, Exception innerException)
        : base(code, message, innerException) { }
}