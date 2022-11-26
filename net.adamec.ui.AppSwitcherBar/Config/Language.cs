using System.Collections.Generic;

namespace net.adamec.ui.AppSwitcherBar.Config;

/// <summary>
/// Translation settings
/// </summary>
public class Language : ILanguage
{
    /// <summary>
    /// Translations collection
    /// </summary>
    public Dictionary<string, string>? Translations { get; set; } = new();
}