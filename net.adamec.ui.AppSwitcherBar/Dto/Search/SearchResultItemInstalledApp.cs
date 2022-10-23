using System;

namespace net.adamec.ui.AppSwitcherBar.Dto.Search;

/// <summary>
/// Search result item representing an installed application
/// </summary>
public class SearchResultItemInstalledApp : SearchResultItemWithRef<InstalledApplication>
{
    /// <summary>
    /// CTOR - Installed application
    /// </summary>
    /// <param name="installedApp">Installed application info</param>
    /// <param name="launchAction">Action to be run when the search result is launched</param>
    public SearchResultItemInstalledApp(InstalledApplication installedApp, Action<InstalledApplication>? launchAction):base(installedApp, launchAction)
    {
        Title = installedApp.Name;
        Icon = installedApp.IconSource;
        ItemType = SearchResultItemTypeEnum.InstalledApplication;
    }
}