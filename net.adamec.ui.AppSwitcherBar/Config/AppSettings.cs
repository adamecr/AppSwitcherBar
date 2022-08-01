using System;
using System.Collections.Generic;
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
        /// Flag whether to show the application in the task bar (default is true)
        /// </summary>
        public bool ShowInTaskbar { get; set; } = true;
        /// <summary>
        /// Startup dock mode of the application (default is <see cref="AppBarDockMode.Bottom"/>
        /// </summary>
        public AppBarDockMode AppBarDock { get; set; } = AppBarDockMode.Bottom;
        /// <summary>
        /// The width of application bar when docked vertically (default is 200)
        /// </summary>
        public int AppBarDockedWidth { get; set; } = 200;
        /// <summary>
        /// The height of application bar when docked horizontally (default is 100)
        /// </summary>
        public int AppBarDockedHeight { get; set; } = 100;
        /// <summary>
        /// Minimal width of application bar (default is 100)
        /// </summary>
        public int AppBarMinWidth { get; set; } = 100;
        /// <summary>
        /// Minimal height of application bar  (default is 50)
        /// </summary>
        public int AppBarMinHeight { get; set; } = 50;
        /// <summary>
        /// Flag whether to redraw the application bar on resize drag (true) or just when the drag is completed (false) (default is true)
        /// </summary>
        public bool AppBarResizeRedrawOnDrag { get; set; } = true;
        /// <summary>
        /// Width of the button representing the (task bar) window (default is 200)
        /// </summary>
        public int AppBarButtonWidth { get; set; } = 200;
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
        /// Flag whether the application will invert the white icons (default is true)
        /// </summary>
        public bool InvertWhiteIcons { get; set; } = true;

        /// <summary>
        /// Limits the number of items in single category of JumpList (not applied to Tasks) (default is 10)
        /// </summary>
        public int JumpListCategoryLimit { get; set; } = 10;

        /// <summary>
        /// Flag whether the application will allow user to set/reset AppSwitcherBar to run on Windows startup (default is true)
        /// </summary>
        public bool AllowRunOnWindowsStartup { get; set; } = true;
    }



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
        /// Flag whether the application will invert the white icons
        /// </summary>
        bool InvertWhiteIcons { get; }

        /// <summary>
        /// Limits the number of items in single category of JumpList (not applied to Tasks)
        /// </summary>
        public int JumpListCategoryLimit { get; }

        /// <summary>
        /// Flag whether the application will allow user to set/reset AppSwitcherBar to run on Windows startup
        /// </summary>
        bool AllowRunOnWindowsStartup { get; }
    }
}
