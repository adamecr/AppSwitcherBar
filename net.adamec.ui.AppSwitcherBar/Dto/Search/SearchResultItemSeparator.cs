namespace net.adamec.ui.AppSwitcherBar.Dto.Search;

/// <summary>
/// Search result item representing a separator
/// </summary>
public class SearchResultItemSeparator : SearchResultItem
{
    /// <summary>
    /// Flag that the search result item behaves as separator in UI (menu showing the results)
    /// </summary>
    public override bool IsSeparator => true;

    /// <summary>
    /// CTOR - Separator or more items flag
    /// </summary>
    public SearchResultItemSeparator()
    {
        Title = null;
        Icon = null;
        ItemType = SearchResultItemTypeEnum.Separator;
    }
}