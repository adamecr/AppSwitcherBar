namespace net.adamec.ui.AppSwitcherBar.Dto.Search;

/// <summary>
/// Search result item representing a more items available "flag"
/// </summary>
public class SearchResultItemMoreItems : SearchResultItem
{
    /// <summary>
    /// Flag that the search result item behaves as separator in UI (menu showing the results)
    /// </summary>
    public override bool IsSeparator => true;
    /// <summary>
    /// CTOR - Separator or more items flag
    /// </summary>
    public SearchResultItemMoreItems()
    {
        Title = null;
        Icon = null;
        ItemType = SearchResultItemTypeEnum.MoreItems;
    }
}