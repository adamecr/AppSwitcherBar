namespace net.adamec.ui.AppSwitcherBar.Dto.AppList;

/// <summary>
/// Search result item representing a header
/// </summary>
public class AppListCategory : AppListItem
{
    /// <summary>
    /// Flag whether the app list item is executable
    /// </summary>
    public override bool IsExecutable =>false;

    /// <summary>
    /// CTOR
    /// </summary>
    /// <param name="category">Category title</param>
    public AppListCategory(string category)
    {
        Title = category;
        Icon = null;
        ItemType = AppListItemTypeEnum.Category;
    }
}