using System;

namespace net.adamec.ui.AppSwitcherBar.Dto.Search;

/// <summary>
/// Search result item representing a pinned application
/// </summary>
public class SearchResultItemPinnedApp : SearchResultItemWithRef<PinnedAppInfo>
{
    /// <summary>
    /// CTOR - Pinned application
    /// </summary>
    /// <param name="pinInfo">Pinned application information</param>
    /// <param name="launchAction">Action to be run when the search result is launched</param>
    public SearchResultItemPinnedApp(PinnedAppInfo pinInfo, Action<PinnedAppInfo>? launchAction):base(pinInfo,launchAction)
    {
        Title = pinInfo.Title;
        Icon = pinInfo.BitmapSource;
        ItemType = SearchResultItemTypeEnum.PinnedApplication;
    }
}