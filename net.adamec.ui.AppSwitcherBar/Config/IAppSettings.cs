using System;
using System.Collections.Generic;
using System.Windows;
using net.adamec.ui.AppSwitcherBar.AppBar;
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace net.adamec.ui.AppSwitcherBar.Config;

/// <summary>
/// Application settings (read only interface)
/// </summary>
public interface IAppSettings
{
    /// <summary>
    /// Flag whether to show the application in the task bar
    /// </summary>
    bool ShowInTaskbar { get; }
    /// <summary>
    /// Startup dock mode of the application
    /// </summary>
    AppBarDockMode AppBarDock { get; }

    /// <summary>
    /// Flag whether to auto-size the application bar
    /// </summary>
    bool AppBarAutoSize { get; }
    /// <summary>
    /// The width of application bar when docked vertically
    /// </summary>
    int AppBarDockedWidth { get; }
    /// <summary>
    /// The height of application bar when docked horizontally
    /// </summary>
    int AppBarDockedHeight { get; }
    /// <summary>
    /// Minimal width of application bar
    /// </summary>
    int AppBarMinWidth { get; }
    /// <summary>
    /// Minimal height of application bar
    /// </summary>
    int AppBarMinHeight { get; }

    /// <summary>
    /// Flag whether to redraw the application bar on resize drag (true) or just when the drag is completed (false)
    /// </summary>
    bool AppBarResizeRedrawOnDrag { get; }

    /// <summary>
    /// Width of the button representing the (task bar) window
    /// </summary>
    int AppBarButtonWidth { get; }

    /// <summary>
    /// Minimal width ration for the button representing the (task bar) window when auto size 
    /// </summary>
    double AppBarButtonMinWidthRatio { get; }

    /// <summary>
    /// Height of the button representing the (task bar) window
    /// </summary>
    int AppBarButtonHeight { get; }

    /// <summary>
    /// Margin of the button representing the (task bar) window
    /// </summary>
    Thickness AppBarButtonMargin { get; }

    /// <summary>
    /// Width of the window (task bar application) thumbnail
    /// </summary>
    int AppBarThumbnailWidth { get; }
    /// <summary>
    /// Height of the window (task bar application) thumbnail
    /// </summary>
    int AppBarThumbnailHeight { get; }

    /// <summary>
    /// Refresh interval (in milliseconds) when retrieving the list of windows (task bar applications)
    /// </summary>
    int RefreshWindowInfosIntervalMs { get; }

    /// <summary>
    /// Flag whether to check for window icon change during refresh or use the one retrieved for the first time
    /// </summary>
    bool CheckForIconChange { get; }

    /// <summary>
    /// Flag whether to check for AppId (AUMID) change during refresh or use the one retrieved for the first time
    /// </summary>
    bool CheckForAppIdChange { get; }

    /// <summary>
    /// Feature flags collection
    /// </summary>
    Dictionary<string, string>? FeatureFlags { get; }

    /// <summary>
    /// Returns the value of feature flag with given <paramref name="featureFlagName"/> or default(T)
    /// </summary>
    /// <typeparam name="T">Required <see cref="Type"/> of the feature flag value</typeparam>
    /// <param name="featureFlagName">Name of the feature flag</param>
    /// <returns>Value of feature flag with given <paramref name="featureFlagName"/> or default(T)</returns>
    T? FeatureFlag<T>(string featureFlagName);

    /// <summary>
    /// Returns the value of feature flag with given <paramref name="featureFlagName"/> or <paramref name="defaultValue"/>
    /// </summary>
    /// <typeparam name="T">Required <see cref="Type"/> of the feature flag value</typeparam>
    /// <param name="featureFlagName">Name of the feature flag</param>
    /// <param name="defaultValue">Value to be returned when the <paramref name="featureFlagName"/> doesn't exist of can't be converted to <typeparamref name="T"/></param>
    /// <returns>Value of feature flag with given <paramref name="featureFlagName"/> or <paramref name="defaultValue"/></returns>
    T? FeatureFlag<T>(string featureFlagName, T? defaultValue);

    /// <summary>
    /// Application IDs collection
    /// </summary>
    Dictionary<string, string>? AppIds { get; }

    /// <summary>
    /// Get the dictionary of known appIds based on <see cref="AppIds"/> field - the key is expanded path to executable, value is the appId
    /// </summary>
    /// <returns>Dictionary of appIds (empty is no <see cref="AppIds"/> are presented)</returns>
    Dictionary<string, string> GetKnowAppIds();

    /// <summary>
    /// Flag whether the application will invert the white icons
    /// </summary>
    bool InvertWhiteIcons { get; }

    /// <summary>
    /// Limits the number of items in single category of JumpList (not applied to Tasks)
    /// </summary>
    public int JumpListCategoryLimit { get; }

    /// <summary>
    /// Flag whether to extract the links (.lnk) from JumpLists to temporary files and read properties from them. Otherwise the in-memory processing is used
    /// </summary>
    public bool JumpListUseTempFiles { get; }

    /// <summary>
    /// Flag whether the application will allow user to set/reset AppSwitcherBar to run on Windows startup
    /// </summary>
    bool AllowRunOnWindowsStartup { get; }

    /// <summary>
    /// Flag whether the application will allow user to change the order of the buttons representing the (task bar) windows
    /// </summary>
    bool AllowAppBarButtonReorder { get; } 
   
    /// <summary>
    /// Flag whether to show buttons ("pins") for applications pinned to the task bar when they have no window open
    /// </summary>
    bool ShowPinnedApps { get; }

    /// <summary>
    /// Flag whether to hide the apps having single window only and not being pinned
    /// </summary>
    bool HideSingleWindowApps { get; }

    /// <summary>
    /// Flag whether to allow the search functionality
    /// </summary>
    bool AllowSearch { get; }

    /// <summary>
    /// Maximum number of items in single category when presenting the search results
    /// </summary>
    int SearchListCategoryLimit { get; }

    /// <summary>
    /// Width of the panel presenting the search results
    /// </summary>
    int SearchResultPanelWidth { get; }

    //---------------------------------------------------------------------

    /// <summary>
    /// Application settings that can be changed by user - used to save only!
    /// </summary>
    public UserSettings UserSettings { get; }

}