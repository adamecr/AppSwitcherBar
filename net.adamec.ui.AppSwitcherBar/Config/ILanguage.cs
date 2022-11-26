using System.Collections.Generic;

// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace net.adamec.ui.AppSwitcherBar.Config;

/// <summary>
/// Translation settings (read only interface)
/// </summary>
public interface ILanguage
{
    /// <summary>
    /// Translations collection
    /// </summary>
    Dictionary<string, string>? Translations { get; }
}