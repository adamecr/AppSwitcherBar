using net.adamec.ui.AppSwitcherBar.Dto;

namespace net.adamec.ui.AppSwitcherBar.Wpf;

/// <summary>
/// <see cref="AppWindowButton.BuildContextMenuCommand"/> parameters
/// </summary>
internal class BuildContextMenuCommandParams
{

    /// <summary>
    /// Application window button
    /// </summary>
    public AppWindowButton Button { get; }
    /// <summary>
    /// Application window information
    /// </summary>
    public WndInfo WindowInfo { get; }

    /// <summary>
    /// CTOR
    /// </summary>
    /// <param name="button">Application window button</param>
    /// <param name="windowInfo">Application window information</param>
    public BuildContextMenuCommandParams(AppWindowButton button, WndInfo windowInfo)
    {
        Button = button;
        WindowInfo = windowInfo;
    }
}