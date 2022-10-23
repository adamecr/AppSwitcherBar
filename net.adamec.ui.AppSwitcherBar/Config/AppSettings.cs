using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using net.adamec.ui.AppSwitcherBar.AppBar;
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace net.adamec.ui.AppSwitcherBar.Config
{
    /// <summary>
    /// Application settings
    /// </summary>
    public class AppSettings : IAppSettings
    {
        /// <summary>
        /// Design time app settings
        /// </summary>
        public static AppSettings DesignTimeAppSettings { get; } = new()
        {
            AppBarAutoSize = false
        };

        /// <summary>
        /// Flag whether to show the application in the task bar (default is true)
        /// </summary>
        public bool ShowInTaskbar { get; set; } = true;
        /// <summary>
        /// Startup dock mode of the application (default is <see cref="AppBarDockMode.Bottom"/>
        /// </summary>
        public AppBarDockMode AppBarDock { get; set; } = AppBarDockMode.Bottom;

        /// <summary>
        /// Flag whether to auto-size the application bar (default is true)
        /// </summary>
        public bool AppBarAutoSize { get; set; } = true;
        /// <summary>
        /// The width of application bar when docked vertically (default is 170)
        /// </summary>
        public int AppBarDockedWidth { get; set; } = 170;
        /// <summary>
        /// The height of application bar when docked horizontally (default is 50)
        /// </summary>
        public int AppBarDockedHeight { get; set; } = 50;
        /// <summary>
        /// Minimal width of application bar (default is 170)
        /// </summary>
        public int AppBarMinWidth { get; set; } = 170;
        /// <summary>
        /// Minimal height of application bar  (default is 50)
        /// </summary>
        public int AppBarMinHeight { get; set; } = 50;
        /// <summary>
        /// Flag whether to redraw the application bar on resize drag (true) or just when the drag is completed (false) (default is true)
        /// </summary>
        public bool AppBarResizeRedrawOnDrag { get; set; } = true;
        /// <summary>
        /// Width of the button representing the (task bar) window (default is 150)
        /// </summary>
        public int AppBarButtonWidth { get; set; } = 150;

        /// <summary>
        /// Minimal width ration for the button representing the (task bar) window when auto size (default is 0.5)
        /// </summary>
        public double AppBarButtonMinWidthRatio { get; set; } = 0.5;

        /// <summary>
        /// Height of the button representing the (task bar) window (default is 26)
        /// </summary>
        public int AppBarButtonHeight { get; set; } = 26;

        /// <summary>
        /// Margin of the button representing the (task bar) window (default is 2)
        /// </summary>
        public Thickness AppBarButtonMargin { get; set; } = new(2);

        /// <summary>
        /// Width of the window (task bar application) thumbnail (default is 200)
        /// </summary>
        public int AppBarThumbnailWidth { get; set; } = 200;
        /// <summary>
        /// Height of the window (task bar application) thumbnail (default is 120)
        /// </summary>
        public int AppBarThumbnailHeight { get; set; } = 120;
        /// <summary>
        /// Refresh interval (in milliseconds) when retrieving the list of windows (task bar applications) (default is 200ms)
        /// </summary>
        public int RefreshWindowInfosIntervalMs { get; set; } = 200;

        /// <summary>
        /// Flag whether to check for window icon change during refresh or use the one retrieved for the first time (default is true)
        /// </summary>
        public bool CheckForIconChange { get; set; } = true;

        /// <summary>
        /// Flag whether to check for AppId (AUMID) change during refresh or use the one retrieved for the first time (default is false)
        /// </summary>
        public bool CheckForAppIdChange { get; set; } = false;

        /// <summary>
        /// Feature flags collection
        /// </summary>
        public Dictionary<string, string>? FeatureFlags { get; set; } = new();

        /// <summary>
        /// Returns the value of feature flag with given <paramref name="featureFlagName"/> or default(T)
        /// </summary>
        /// <typeparam name="T">Required <see cref="Type"/> of the feature flag value</typeparam>
        /// <param name="featureFlagName">Name of the feature flag</param>
        /// <returns>Value of feature flag with given <paramref name="featureFlagName"/> or default(T)</returns>
        public T? FeatureFlag<T>(string featureFlagName)
        {
            return FeatureFlag<T>(featureFlagName, default);
        }

        /// <summary>
        /// Returns the value of feature flag with given <paramref name="featureFlagName"/> or <paramref name="defaultValue"/>
        /// </summary>
        /// <typeparam name="T">Required <see cref="Type"/> of the feature flag value</typeparam>
        /// <param name="featureFlagName">Name of the feature flag</param>
        /// <param name="defaultValue">Value to be returned when the <paramref name="featureFlagName"/> doesn't exist of can't be converted to <typeparamref name="T"/></param>
        /// <returns>Value of feature flag with given <paramref name="featureFlagName"/> or <paramref name="defaultValue"/></returns>
        public T? FeatureFlag<T>(string featureFlagName, T? defaultValue)
        {
            if (FeatureFlags?.TryGetValue(featureFlagName, out var strValue) ?? false)
            {
                try
                {
                    return (T)Convert.ChangeType(strValue, typeof(T));
                }
                catch
                {
                    //do nothing - return default
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// Application IDs collection
        /// </summary>
        public Dictionary<string, string>? AppIds { get; set; } = new();

        /// <summary>
        /// Get the dictionary of known appIds based on <see cref="AppIds"/> field - the key is expanded path to executable, value is the appId
        /// </summary>
        /// <returns>Dictionary of appIds (empty is no <see cref="AppIds"/> are presented)</returns>
        public Dictionary<string, string> GetKnowAppIds()
        {
            var knownAppIds = new Dictionary<string, string>();
            if (AppIds != null)
            {
                foreach (var (executable, appId) in AppIds)
                {
                    var keyFullPath = Environment.ExpandEnvironmentVariables(executable).Replace('/', '\\').ToLowerInvariant();
                    knownAppIds[keyFullPath] = appId;

                    var keyFileOnly = Path.GetFileName(keyFullPath);
                    knownAppIds[keyFileOnly] = appId;
                }
            }
            //make sure the explorer is there!
            var explorerExecutable = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "explorer.exe").ToLowerInvariant();
            knownAppIds[explorerExecutable] = "Microsoft.Windows.Explorer";
            return knownAppIds;
        }

        /// <summary>
        /// Flag whether the application will invert the white icons (default is true)
        /// </summary>
        public bool InvertWhiteIcons { get; set; } = true;

        /// <summary>
        /// Limits the number of items in single category of JumpList (not applied to Tasks) (default is 10)
        /// </summary>
        public int JumpListCategoryLimit { get; set; } = 10;

        /// <summary>
        /// Flag whether to extract the links (.lnk) from JumpLists to temporary files and read properties from them. Otherwise the in-memory processing is used (default is false)
        /// </summary>
        public bool JumpListUseTempFiles { get; set; } = false;

        /// <summary>
        /// Flag whether the application will allow user to set/reset AppSwitcherBar to run on Windows startup (default is true)
        /// </summary>
        public bool AllowRunOnWindowsStartup { get; set; } = true;

        /// <summary>
        /// Flag whether the application will allow user to change the order of the buttons representing the (task bar) windows (default is true)
        /// </summary>
        public bool AllowAppBarButtonReorder { get; set; } = true;

        /// <summary>
        /// Flag whether to hide the apps having single window only and not being pinned (default is false)
        /// </summary>
        public bool HideSingleWindowApps { get; set; } = false;

        /// <summary>
        /// Flag whether to show buttons ("pins") for applications pinned to the task bar when they have no window open (default is true)
        /// </summary>
        public bool ShowPinnedApps { get; set; } = true;

        /// <summary>
        /// Flag whether to allow the search functionality (default is true)
        /// </summary>
        public bool AllowSearch { get; set; } = true;

        /// <summary>
        /// Maximum number of items in single category when presenting the search results (default is 5)
        /// </summary>
        public int SearchListCategoryLimit { get; set; } = 5;

        /// <summary>
        /// Width of the panel presenting the search results (default is 400)
        /// </summary>
        public int SearchResultPanelWidth { get; set; } = 400;

        //---------------------------------------------------------------------

        /// <summary>
        /// Application settings that can be changed by user in UI - used to save only!
        /// </summary>
        public UserSettings UserSettings { get; }

        /// <summary>
        /// CTOR
        /// </summary>
        public AppSettings()
        {
            UserSettings = new UserSettings(this);
        }
    }
}
