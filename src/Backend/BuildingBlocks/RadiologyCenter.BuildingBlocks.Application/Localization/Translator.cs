namespace RadiologyCenter.BuildingBlocks.Application.Localization;

/// <summary>
/// Ambient accessor for the current <see cref="ITranslator"/>.
/// Set once at startup via <c>Translator.Current = ...</c>.
/// </summary>
public static class Translator
{
    private static ITranslator? _current;

    public static ITranslator Current
    {
        get => _current ?? throw new InvalidOperationException(
            "ITranslator not registered. Set Translator.Current at startup.");
        set => _current = value ?? throw new ArgumentNullException(nameof(value));
    }

    public static string LocalizeCode(string code, string? fallbackMessage = null) => Current.TranslateCode(code, fallbackMessage);

    public static string LocalizeEnum(string typeName, string name) => Current.TranslateEnum(typeName, name);
}
