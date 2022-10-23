namespace net.adamec.ui.AppSwitcherBar.Dto.Search;

/// <summary>
/// Search result item representing a header
/// </summary>
public class SearchResultItemHeader : SearchResultItem
{
    /// <summary>
    /// Flag that the search result item behaves as separator in UI (menu showing the results)
    /// </summary>
    public override bool IsSeparator =>true;

    /// <summary>
    /// CTOR - Header
    /// </summary>
    /// <param name="header">Header title</param>
    public SearchResultItemHeader(string header)
    {
        Title = header;
        Icon = null;
        ItemType = SearchResultItemTypeEnum.Header;
    }
}