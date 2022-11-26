using System;

namespace net.adamec.ui.AppSwitcherBar.Dto.Search;

/// <summary>
/// Search result item representing a non-application (document) registered in windows AppsFolder
/// </summary>
public class SearchResultItemInstalledDoc : SearchResultItemWithRef<InstalledApplication>
{
    /// <summary>
    /// CTOR - Installed Document
    /// </summary>
    /// <param name="installedNonApp">Installed "document" info</param>
    /// <param name="launchAction">Action to be run when the search result is launched</param>
    public SearchResultItemInstalledDoc(InstalledApplication installedNonApp, Action<InstalledApplication>? launchAction):base(installedNonApp, launchAction)
    {
        Title = installedNonApp.Name;
        Icon = installedNonApp.IconSource;
        ItemType = SearchResultItemTypeEnum.InstalledDocument;
    }
}