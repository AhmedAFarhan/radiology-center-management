namespace RadiologyCenter.BuildingBlocks.Application.Localization;

/// <summary>
/// Ambient accessor for the current <see cref="ITranslator"/>.
/// Allows static mapping helpers and middleware to resolve localized text
/// without constructor injection. Set once at startup.
/// </summary>
public static class Translator
{
    private static ITranslator _current = NullTranslator.Instance;

    public static ITranslator Current
    {
        get => _current;
        set => _current = value ?? NullTranslator.Instance;
    }

    public static string Localize(string message) => Current.TranslateMessage(message);

    public static string LocalizeCode(string? code, string? fallbackMessage = null) =>
        Current.TranslateCode(code, fallbackMessage);

    public static string LocalizeEnum(string typeName, string name) => Current.TranslateEnum(typeName, name);
}

/// <summary>
/// Identity translator used before startup registration and as a safe fallback.
/// </summary>
public sealed class NullTranslator : ITranslator
{
    public static readonly NullTranslator Instance = new();

    private NullTranslator() { }

    public string TranslateMessage(string message) => message;

    public string TranslateCode(string? code, string? fallbackMessage = null) => fallbackMessage ?? code ?? string.Empty;

    public string TranslateEnum(string typeName, string name) => name;
}