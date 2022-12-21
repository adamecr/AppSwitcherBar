using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace net.adamec.ui.AppSwitcherBar.Config
{
    /// <summary>
    /// Language service providing the translations
    /// </summary>
    public class LanguageService : ILanguageService
    {
        /// <summary>
        /// Dictionary of translations by key
        /// </summary>
        private Dictionary<string, string> Translations { get; } = new();

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="translationOptions">Source of translation texts from configuration file</param>
        public LanguageService(IOptions<Language>? translationOptions)
        {
            InitDefaultTranslations();
            var language = translationOptions?.Value;
            if (language?.Translations != null)
            {
                foreach (var kv in language.Translations)
                {
                    SetTranslation(kv.Key, kv.Value);
                }
            }

            //The translations are also available in XAML as string resources
            //The key is prefixed with "Str". So it can be used as like {DynamicResource StrMenuPopupToggleTheme}
            var resources = Application.Current.Resources;
            foreach (var translation in Translations)
            {
                resources[$"Str{translation.Key}"] = translation.Value;
            }
        }

        /// <summary>
        /// Initialize default translations
        /// </summary>
        private void InitDefaultTranslations()
        {
            SetTranslation(TranslationKeys.MenuPopupHidePopup, "Hide popup");
            SetTranslation(TranslationKeys.MenuPopupSearch, "Search");
            SetTranslation(TranslationKeys.MenuPopupSettings, "Settings");
            SetTranslation(TranslationKeys.MenuPopupColors, "Colors");
            SetTranslation(TranslationKeys.MenuPopupApps, "Applications");
            SetTranslation(TranslationKeys.MenuPopupPins, "Pinned apps");
            SetTranslation(TranslationKeys.MenuPopupToggleDesktop, "Toggle desktop");
            SetTranslation(TranslationKeys.MenuPopupToggleTheme, "Toggle theme");
            SetTranslation(TranslationKeys.MenuPopupExit, "Exit app");
            SetTranslation(TranslationKeys.MenuPopupSettingsRefresh, "Refresh");
            SetTranslation(TranslationKeys.MenuPopupSettingsRunOnStartup, "Run on Win start");
            SetTranslation(TranslationKeys.MenuPopupSettingsAutoSize, "Auto size");

            SetTranslation(TranslationKeys.EdgeLeft, "Left");
            SetTranslation(TranslationKeys.EdgeRight, "Right");
            SetTranslation(TranslationKeys.EdgeTop, "Top");
            SetTranslation(TranslationKeys.EdgeBottom, "Bottom");

            SetTranslation(TranslationKeys.SearchCategoryWindows, "Windows");
            SetTranslation(TranslationKeys.SearchCategoryPinnedApps, "Pinned applications");
            SetTranslation(TranslationKeys.SearchCategoryInstalledApps, "Applications");
            SetTranslation(TranslationKeys.SearchCategoryInstalledDocs, "Document");

            SetTranslation(TranslationKeys.JumpListMenuCloseWindow, "Close window");
            SetTranslation(TranslationKeys.JumpListMenuCancel, "Cancel");
            SetTranslation(TranslationKeys.JumpListCategoryTasks, "Tasks");
            SetTranslation(TranslationKeys.JumpListCategoryPinned, "Pinned");
            SetTranslation(TranslationKeys.JumpListCategoryRecent, "Recent");
            SetTranslation(TranslationKeys.JumpListCategoryFrequent, "Frequent");
            
            SetTranslation(TranslationKeys.AudioIsDefault, "Default device");
            SetTranslation(TranslationKeys.AudioSetDefault, "Set as default device");
            SetTranslation(TranslationKeys.AudioIsCommDefault, "Default comm device");
        }

        /// <summary>
        /// Sets the translated text for given <paramref name="key"/>
        /// </summary>
        /// <param name="key">Known text key</param>
        /// <param name="translation">Translated text</param>
        private void SetTranslation(TranslationKeys key, string translation)
        {
            Translations[key.ToString()] = translation;
        }

        /// <summary>
        /// Sets the translated text for given <paramref name="key"/>
        /// </summary>
        /// <param name="key">Text key (either known or custom)</param>
        /// <param name="translation">Translated text</param>
        private void SetTranslation(string key, string translation)
        {
            Translations[key] = translation;
        }

        /// <summary>
        /// Gets the translation for known text identified by <paramref name="key"/>
        /// </summary>
        /// <param name="key">Known text identifier</param>
        /// <returns>Translation for known text</returns>
        public string? Translate(TranslationKeys key)
        {
            return Translations.TryGetValue(key.ToString(), out var translation) ? translation : null;
        }

        /// <summary>
        /// Gets the translation for custom text if provided in language file
        /// </summary>
        /// <param name="source">Text to be translated</param>
        /// <returns>Translation for custom text if provided in language file otherwise the <paramref name="source"/> is returned</returns>
        public string? TranslateCustom(string? source)
        {
            if (source == null) return null;

            var customKey = $"Custom{source.Replace(" ", "")}".ToLower();
            var customTranslation = Translations.Where(kv => kv.Key.ToLower() == customKey).ToArray();
            return customTranslation.Length == 1 ? customTranslation[0].Value : source;
        }
    }
}
