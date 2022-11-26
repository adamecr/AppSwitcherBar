using System.Collections.Generic;
using net.adamec.ui.AppSwitcherBar.WpfExt;

namespace net.adamec.ui.AppSwitcherBar.Dto.AppList;

/// <summary>
/// Search result item representing a header
/// </summary>
public class AppListFolder : AppListItem
{
    /// <summary>
    /// Flag whether the app list item is executable
    /// </summary>
    public override bool IsExecutable => false;

    /// <summary>
    /// Flag whether the folder is expanded
    /// </summary>
    private bool isExpanded;

    /// <summary>
    /// Flag whether the folder is expanded
    /// </summary>
    public bool IsExpanded
    {
        get => isExpanded;
        set
        {
            if (isExpanded != value)
            {
                isExpanded=value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsCollapsed));
                SetItemsVisibility();
            }
        }
    }

    /// <summary>
    /// Flag whether the folder is collapsed
    /// </summary>
    public bool IsCollapsed
    {
        get => !isExpanded;
    }

    /// <summary>
    /// Folder items
    /// </summary>
    private readonly List<AppListItem> folderItems = new();

    /// <summary>
    /// CTOR 
    /// </summary>
    /// <param name="name">Folder title</param>
    public AppListFolder(string name)
    {
        Title = name;
        Icon = null; //Icon is set in XAML
        ItemType = AppListItemTypeEnum.Folder;
    }

    /// <summary>
    /// Adds an item into folder - this is used only for switching the visibility of items, not for items rendering!
    /// </summary>
    /// <param name="item">Item to be added</param>
    public void AddItem(AppListItem item)
    {
        folderItems.Add(item);
        item.IsVisible = isExpanded;
    }

    /// <summary>
    /// Sets the visibility of folder items
    /// </summary>
    private void SetItemsVisibility()
    {
        foreach (var item in folderItems)
        {
            item.IsVisible = isExpanded;
            (item.Command as RelayCommand)!.RaiseCanExecuteChanged();
        }
    }

    /// <summary>
    /// Returns the information whether the app list item can be "executed" from UI (see <see cref="OnCommand"/>)
    /// </summary>
    /// <param name="parameter">Command parameter - not used</param>
    /// <returns>Always true for folder</returns>
    protected override bool CanCommandExecute(object? parameter)
    {
        return true;
    }

    /// <summary>
    /// Action to be run when the app list item is "executed" from UI
    /// </summary>
    /// <param name="parameter">Command parameter - not used</param>
    protected override void OnCommand(object? parameter)
    {
        IsExpanded = !isExpanded;
    }
}