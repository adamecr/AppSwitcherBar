using System;

namespace net.adamec.ui.AppSwitcherBar.Dto.Search;

/// <summary>
/// Search result item representing an application window
/// </summary>
public class SearchResultItemWindow : SearchResultItemWithRef<WndInfo>
{
    /// <summary>
    /// CTOR - Application window
    /// </summary>
    /// <param name="wndInfo">Window information</param>
    /// <param name="launchAction">Action to be run when the search result is launched</param>
    public SearchResultItemWindow(WndInfo wndInfo, Action<WndInfo>? launchAction):base(wndInfo, launchAction)
    {
        Title = wndInfo.Title;
        Icon = wndInfo.BitmapSource;
        ItemType = SearchResultItemTypeEnum.Window;
    }
}