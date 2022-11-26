using System;

namespace net.adamec.ui.AppSwitcherBar.Dto.AppList;

/// <summary>
/// App list installed item
/// </summary>
public class AppListInstalledItem : AppListItem
{
    /// <summary>
    /// Flag whether the app list item is executable
    /// </summary>
    public override bool IsExecutable => true;

    /// <summary>
    /// Reference to installed item
    /// </summary>
    public InstalledApplication InstalledItem { get; }

    /// <summary>
    /// Flag whether the app list item is in folder
    /// </summary>
    public bool IsInFolder { get; }

    /// <summary>
    /// Action to be run when the app list item is launched
    /// </summary>
    private Action<InstalledApplication>? LaunchAction { get;  }


    /// <summary>
    /// CTOR
    /// </summary>
    /// <param name="installedItem">Reference to installed item</param>
    /// <param name="isInFolder">Flag whether the app list item is in folder</param>
    /// <param name="launchAction">Action to be run when the app list item is launched</param>
    public AppListInstalledItem(InstalledApplication installedItem, bool isInFolder, Action<InstalledApplication>? launchAction)
    {
        InstalledItem = installedItem;
        LaunchAction = launchAction;
        Title = installedItem.Name;
        Icon = installedItem.IconSource;
        ItemType = AppListItemTypeEnum.InstalledItem;
        IsInFolder = isInFolder;
    }

    /// <summary>
    /// Action to be run when the app list item is "executed" from UI
    /// </summary>
    /// <param name="parameter">Command parameter - not used</param>
    protected override void OnCommand(object? parameter)
    {
        Launch();
    }

    /// <summary>
    /// Launch the app list item
    /// </summary>
    public void Launch()
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (InstalledItem != null) LaunchAction?.Invoke(InstalledItem);
    }
}
