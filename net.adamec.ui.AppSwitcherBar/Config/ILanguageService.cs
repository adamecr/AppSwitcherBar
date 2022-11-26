namespace net.adamec.ui.AppSwitcherBar.Config;

/// <summary>
/// Language service providing the translations
/// </summary>
public interface ILanguageService
{
    /// <summary>
    /// Gets the translation for known text identified by <paramref name="key"/>
    /// </summary>
    /// <param name="key">Known text identifier</param>
    /// <returns>Translation for known text</returns>
    string? Translate(TranslationKeys key);

    /// <summary>
    /// Gets the translation for custom text if provided in language file
    /// </summary>
    /// <param name="source">Text to be translated</param>
    /// <returns>Translation for custom text if provided in language file otherwise the <paramref name="source"/> is returned</returns>
    string? TranslateCustom(string? source);
}