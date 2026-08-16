namespace RadiologyCenter.BuildingBlocks.Application.Localization;

public interface ITranslator
{
    /// <summary>
    /// Translates a message by its English text (exact or {0}-template key).
    /// Legacy lookup used as a fallback for code-less messages.
    /// </summary>
    string TranslateMessage(string message);

    /// <summary>
    /// Translates a strongly-typed semantic error code. Falls back to
    /// <paramref name="fallbackMessage"/> (if provided), then to the code itself.
    /// </summary>
    string TranslateCode(string? code, string? fallbackMessage = null);

    string TranslateEnum(string typeName, string name);
}