using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using net.adamec.ui.AppSwitcherBar.Config;
using net.adamec.ui.AppSwitcherBar.Dto;
using net.adamec.ui.AppSwitcherBar.ViewModel;
using net.adamec.ui.AppSwitcherBar.Win32.NativeClasses;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces;
using net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces.Extensions;
using net.adamec.ui.AppSwitcherBar.Win32.NativeMethods;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
using net.adamec.ui.AppSwitcherBar.Win32.Services.Shell.Properties;
using Wpf.Ui.Appearance;
using Wpf.Ui.Mvvm.Contracts;

namespace net.adamec.ui.AppSwitcherBar.Win32.Services.Pins
{
    /// <summary>
    /// Encapsulates the Windows task bar and start menu pinned apps functionality
    /// </summary>
    public class PinsService : IPinsService
    {
        #region Logging
        // ReSharper disable InconsistentNaming
        /// <summary>
        /// Logger used
        /// </summary>
        protected readonly ILogger Logger;
        // 6xxx - Pinned apps Service (69xx Errors/Exceptions)
        // 61xx -   Taskbar pinned apps
        // 62xx -   Start pinned apps
        /// <summary>
        /// Log definition options
        /// </summary>
        protected static readonly LogDefineOptions LogOptions = new() { SkipEnabledCheck = true };

        //----------------------------------------------
        // 6101 Processing Task bar pins start
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogTaskbarPinProcessingStart
        /// </summary>
        private static readonly Action<ILogger, Exception?> __LogTaskbarPinProcessingStartDefinition =

            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(6101, nameof(LogTaskbarPinProcessingStart)),
                "Task bar pins processing starts",
                LogOptions);

        /// <summary>
        /// Logs record (Information) when a Taskbar pins processing starts
        /// </summary>
        protected void LogTaskbarPinProcessingStart()
        {
            if (Logger.IsEnabled(LogLevel.Information))
            {
                __LogTaskbarPinProcessingStartDefinition(Logger, null);
            }
        }

        //----------------------------------------------
        // 6102 Processing Task bar pins end
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogTaskbarPinProcessingEnd
        /// </summary>
        private static readonly Action<ILogger, int, Exception?> __LogTaskbarPinProcessingEndDefinition =

            LoggerMessage.Define<int>(
                LogLevel.Information,
                new EventId(6102, nameof(LogTaskbarPinProcessingEnd)),
                "Task bar pins processing ends: {itemsCount} retrieved",
                LogOptions);

        /// <summary>
        /// Logs record (Information) when a Task bar pins processing ends
        /// </summary>
        /// <param name="itemsCount">Number of items retrieved</param>
        protected void LogTaskbarPinProcessingEnd(int itemsCount)
        {
            if (Logger.IsEnabled(LogLevel.Information))
            {
                __LogTaskbarPinProcessingEndDefinition(Logger, itemsCount, null);
            }
        }

        //----------------------------------------------
        // 6103 Got Taskbar Pinned application
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogGotTaskbarPinnedApplication
        /// </summary>
        private static readonly Action<ILogger, string, Exception?> __LogGotTaskbarPinnedApplicationDefinition =

            LoggerMessage.Define<string>(
                LogLevel.Debug,
                new EventId(6103, nameof(LogGotTaskbarPinnedApplication)),
                "Retrieved taskbar pinned application info: {pinnedAppInfo}",
                LogOptions);

        /// <summary>
        /// Logs record (Debug) when a Taskbar pinned application information is retrieved
        /// </summary>
        /// <param name="pinnedAppInfo">Information about pinned application</param>
        protected void LogGotTaskbarPinnedApplication(PinnedAppInfo pinnedAppInfo)
        {
            if (Logger.IsEnabled(LogLevel.Debug))
            {
                __LogGotTaskbarPinnedApplicationDefinition(Logger, pinnedAppInfo.ToString(), null);
            }
        }

        //----------------------------------------------
        // 6201 Processing Start menu pins start
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogStartPinProcessingStart
        /// </summary>
        private static readonly Action<ILogger, Exception?> __LogStartPinProcessingStartDefinition =

            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(6201, nameof(LogStartPinProcessingStart)),
                "Start menu pins processing starts",
                LogOptions);

        /// <summary>
        /// Logs record (Information) when a Start menu pins processing starts
        /// </summary>
        protected void LogStartPinProcessingStart()
        {
            if (Logger.IsEnabled(LogLevel.Information))
            {
                __LogStartPinProcessingStartDefinition(Logger, null);
            }
        }

        //----------------------------------------------
        // 6202 Processing Start menu pins end
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogStartPinProcessingEnd
        /// </summary>
        private static readonly Action<ILogger, int, Exception?> __LogStartPinProcessingEndDefinition =

            LoggerMessage.Define<int>(
                LogLevel.Information,
                new EventId(6202, nameof(LogStartPinProcessingEnd)),
                "Start menu pins processing ends: {itemsCount} retrieved",
                LogOptions);

        /// <summary>
        /// Logs record (Information) when a Start menu pins processing ends
        /// </summary>
        /// <param name="itemsCount">Number of items retrieved</param>
        protected void LogStartPinProcessingEnd(int itemsCount)
        {
            if (Logger.IsEnabled(LogLevel.Information))
            {
                __LogStartPinProcessingEndDefinition(Logger, itemsCount, null);
            }
        }

        //----------------------------------------------
        // 6203 Got Start menu pinned application
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogGotStartPinnedApplication
        /// </summary>
        private static readonly Action<ILogger, string, Exception?> __LogGotStartPinnedApplicationDefinition =

            LoggerMessage.Define<string>(
                LogLevel.Debug,
                new EventId(6203, nameof(LogGotStartPinnedApplication)),
                "Retrieved Start menu pinned application info: {pinnedAppInfo}",
                LogOptions);

        /// <summary>
        /// Logs record (Debug) when a Start menu pinned application information is retrieved
        /// </summary>
        /// <param name="pinnedAppInfo">Information about pinned application</param>
        protected void LogGotStartPinnedApplication(PinnedAppInfo pinnedAppInfo)
        {
            if (Logger.IsEnabled(LogLevel.Debug))
            {
                __LogGotStartPinnedApplicationDefinition(Logger, pinnedAppInfo.ToString(), null);
            }
        }

        //----------------------------------------------
        // 6901 PinList Exception
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogPinListException
        /// </summary>
        private static readonly Action<ILogger, Exception?> __LogPinListExceptionDefinition =
            LoggerMessage.Define(
                LogLevel.Warning,
                new EventId(6901, nameof(LogPinListException)),
                "Exception while processing the pinned applications",
                LogOptions);

        /// <summary>
        /// Logs record (Warning) of exception thrown when processing the pinned list
        /// </summary>
        /// <param name="ex">Exception thrown</param>
        protected void LogPinListException(Exception ex)
        {
            if (Logger.IsEnabled(LogLevel.Warning))
            {
                __LogPinListExceptionDefinition(Logger, ex);
            }
        }

        //----------------------------------------------
        // 6902 PinList source parsing error
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogPinListSourceParsingError
        /// </summary>
        private static readonly Action<ILogger, Exception?> __LogPinListSourceParsingErrorDefinition =
            LoggerMessage.Define(
                LogLevel.Warning,
                new EventId(6902, nameof(LogPinListSourceParsingError)),
                "Error while parsing the PinList source",
                LogOptions);

        /// <summary>
        /// Logs record (Warning) when there is a problem when reading the PinList source
        /// </summary>
        protected void LogPinListSourceParsingError(Exception ex)
        {
            if (Logger.IsEnabled(LogLevel.Warning))
            {
                __LogPinListSourceParsingErrorDefinition(Logger, ex);
            }
        }

        // ReSharper restore InconsistentNaming
        #endregion

        /// <summary>
        /// Property key for getting the title from links
        /// </summary>
        private static readonly PropertyKey TitlePropertyKey = new("9e5e05ac-1936-4a75-94f7-4704b8b01923", 0);

        /// <summary>
        /// Application settings
        /// </summary>
        protected readonly IAppSettings Settings;

        /// <summary>
        /// UI theme service
        /// </summary>
        protected readonly IThemeService ThemeService;


        /// <summary>
        /// Flag whether Start pinned apps are to be retrieved
        /// </summary>
        private readonly bool isStartPinsEnabled;

        /// <summary>
        /// Internal CTOR
        /// Directly used by <see cref="ViewModelLocator"/> when creating a design time instance.
        /// Internally called by public "DI bound" CTOR
        /// </summary>
        /// <param name="settings">Application setting</param>
        /// <param name="logger">Logger to be used</param>
        /// <param name="themeService">Application theme service</param>
        internal PinsService(IAppSettings settings, ILogger logger, IThemeService themeService)
        {
            Logger = logger;
            Settings = settings;
            ThemeService = themeService;
            isStartPinsEnabled = Settings.FeatureFlag(AppSettings.FF_EnableStartMenuPins, false);
        }

        /// <summary>
        /// CTOR used by DI
        /// </summary>
        /// <param name="options">Application settings configuration</param>
        /// <param name="logger">Logger used</param>
        /// <param name="themeService">Application theme service</param>
          // ReSharper disable once UnusedMember.Global
        public PinsService(IOptions<AppSettings> options, ILogger<PinsService> logger, IThemeService themeService)
            : this(options.Value, logger, themeService)
        {
            //used from DI - DI populates the parameters and the internal CTOR is called then
        }

        /// <summary>
        /// Refresh (reload) the taskbar pinned applications
        /// </summary>
        public PinnedAppInfo[] RefreshTaskbarPins()
        {
            return GetTaskbarPinnedApplications();
        }

        /// <summary>
        /// Refresh (reload) the Start pinned applications
        /// </summary>
        public PinnedAppInfo[] RefreshStartPins()
        {
            return GetStartPinnedApplications();
        }

        /// <summary>
        /// Gets the information about the applications pinned to the taskbar
        /// </summary>
        /// <returns>Array of information about the applications pinned to the taskbar</returns>
        private PinnedAppInfo[] GetTaskbarPinnedApplications()
        {
            if (!Settings.ShowPinnedApps) return Array.Empty<PinnedAppInfo>();

            var titlePropertyKey = new PropertyKey("9e5e05ac-1936-4a75-94f7-4704b8b01923", 0);
            LogTaskbarPinProcessingStart();

            var appInfos = new List<PinnedAppInfo>();
            var objType = Type.GetTypeFromCLSID(new Guid(Win32Consts.CLSID_TaskbanPin), false);
            if (objType == null) return Array.Empty<PinnedAppInfo>();

            var obj = Activator.CreateInstance(objType);
            if (obj is not IPinnedList3 pinnedList) return Array.Empty<PinnedAppInfo>();

            var hrs = pinnedList.EnumObjects(out var iel);
            if (!hrs.IsSuccess) return Array.Empty<PinnedAppInfo>();

            hrs = iel.Reset();
            if (!hrs.IsSuccess) return Array.Empty<PinnedAppInfo>();

            var order = 0;
            var iShellItem2Guid = new Guid(Win32Consts.IID_IShellItem2);
            do
            {
                hrs = iel.Next(1, out var pidl, out _);
                if (!hrs.IsS_OK) break; //S_FALSE is returned for end of enum, but it's also "success code", so explicit check needed here

                hrs = Shell32.SHCreateItemFromIDList(pidl, iShellItem2Guid, out var shellItem);
                if (!hrs.IsS_OK || shellItem == null) break;

                var shellProperties = shellItem.GetProperties();
                var type = shellProperties.IsStoreApp
                    ? PinnedAppInfo.PinnedAppTypeEnum.Package
                    : PinnedAppInfo.PinnedAppTypeEnum.Link;

                var title =
                    shellItem.GetPropertyValue<string>(titlePropertyKey) ??
                    shellItem.GetPropertyValue<string>(PropertyKey.PKEY_ItemNameDisplay) ??
                    "unknown";

                var appId = shellProperties.ApplicationUserModelId;
                if (appId == null && Settings.FeatureFlag<bool>(AppSettings.FF_UseApplicationResolver))
                {
                    //Try to get AppUserModelId using the win32 app resolver
                    appId = WndAndApp.GetWindowApplicationUserModelId(shellItem);
                }

                var executable = GetExecutableFromLinkProps(shellProperties);
                if (appId == null && executable != null)
                {
                    //appId can be an executable full path, ensure that known folders are transformed to their GUIDs
                    appId = Shell.Shell.ReplaceKnownFolderWithGuid(executable);
                }

                var appInfo = PrefetchRunInfo(new PinnedAppInfo(title, order, type, shellProperties, appId, executable, ThemeService.GetTheme() == ThemeType.Dark));
                appInfos.Add(appInfo);
                order++;
                Marshal.FreeCoTaskMem(pidl);
                LogGotTaskbarPinnedApplication(appInfo);
            } while (hrs.IsS_OK);

            LogTaskbarPinProcessingEnd(appInfos.Count);
            return appInfos.ToArray();
        }

        /// <summary>
        /// Gets the information about the applications pinned to the Start menu
        /// </summary>
        /// <returns>Array of information about the applications pinned to the Start menu</returns>
        private PinnedAppInfo[] GetStartPinnedApplications()
        {
            if (!isStartPinsEnabled) return Array.Empty<PinnedAppInfo>();


            LogStartPinProcessingStart();

            var appInfos = new List<PinnedAppInfo>();
            var objType = Type.GetTypeFromCLSID(new Guid(Win32Consts.CLSID_StartLayoutCmdlet), false);
            if (objType == null) return Array.Empty<PinnedAppInfo>();

            var obj = Activator.CreateInstance(objType);
            if (obj is not IStartLayoutCmdlet startLayoutCmdlet) return Array.Empty<PinnedAppInfo>();

            var startLayoutExportContent = string.Empty;


            var tempFile = Path.GetTempFileName();
            startLayoutCmdlet.ExportStartLayout(tempFile);
            if (File.Exists(tempFile))
            {
                startLayoutExportContent = File.ReadAllText(tempFile);

                try
                {
                    File.Delete(tempFile);
                }
                catch (Exception ex)
                {
                    //Just log it
                    LogPinListException(ex);
                }
            }

            if (!string.IsNullOrEmpty(startLayoutExportContent))
            {
                try
                {
                    if (startLayoutExportContent.StartsWith('{'))
                    {
                        //Win 11 - JSON file
                        ParseStartLayoutJson(startLayoutExportContent, appInfos);
                    }
                    else if (startLayoutExportContent.StartsWith('<'))
                    {
                        //Win 10 - XML file
                        ParseStartLayoutXml(startLayoutExportContent, appInfos);
                    }
                }
                catch (Exception ex)
                {
                    appInfos.Clear();
                    LogPinListSourceParsingError(ex);
                }
            }

            LogStartPinProcessingEnd(appInfos.Count);
            return appInfos.ToArray();
        }

        private static string ResolvePath(string path)
        {
            if (!path.StartsWith("%")) return path;

            foreach (DictionaryEntry v in Environment.GetEnvironmentVariables())
            {
                var key = v.Key.ToString();
                var val = v.Value?.ToString() ?? string.Empty;
                if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(val)) continue;
                key = $"%{key}%";
                if (path.StartsWith(key, StringComparison.InvariantCultureIgnoreCase))
                {
                    return path.Replace(key, val, StringComparison.InvariantCultureIgnoreCase);
                }
            }
            return path;
        }
        private void ParseStartLayoutJson(string content, List<PinnedAppInfo> appInfos)
        {
            var jDocument = JsonNode.Parse(content);
            if (jDocument?["pinnedList"] is not JsonArray jList) return;

            var order = 0;
            foreach (var jItem in jList)
            {
                var jLink = jItem?["desktopAppLink"]?.ToString();
                if (!string.IsNullOrEmpty(jLink))
                {
                    var linkPath = ResolvePath(jLink);
                    var shellItem = Shell.Shell.GetShellItemForPath(linkPath);
                    if (shellItem == null) continue;

                    var appInfo = GetAppInfoFromLink(shellItem, order);

                    appInfos.Add(appInfo);
                    order++;
                }
                else
                {
                    var jPackage = jItem?["packagedAppId"]?.ToString();
                    if (string.IsNullOrEmpty(jPackage)) continue;

                    var appInfo = GetAppInfoFromAppId(jPackage, order);

                    appInfos.Add(appInfo);
                    order++;
                }
            }
        }

        private void ParseStartLayoutXml(string content, List<PinnedAppInfo> appInfos)
        {
            var xDocument = XElement.Parse(content);
            var xStartLayout = xDocument.Descendants().FirstOrDefault(d => d.LocalName() == "StartLayout");
            if (xStartLayout == null) return;

            var columns = xStartLayout.AttributeInt("GroupCellWidth", 100); //used just as multiplicator, 100 is safe value if not found
            var xGroups = xStartLayout.Descendants().Where(d => d.LocalName() == "Group");

            var order = 0;

            foreach (var xGroup in xGroups)
            {
                var groupName = xGroup.Attribute("Name")?.Value ?? string.Empty;
                var xItems = xGroup.Descendants()
                    .Where(d => d.LocalName() == "DesktopApplicationTile" || d.LocalName() == "Tile")
                    .OrderBy(i => i.AttributeInt("Column", 0) + i.AttributeInt("Row", 0) * columns)
                    .ToArray();

                foreach (var xItem in xItems)
                {
                    var xLink = xItem.Attribute("DesktopApplicationLinkPath")?.Value;
                    if (xItem.LocalName() == "DesktopApplicationTile" && !string.IsNullOrEmpty(xLink))
                    {
                        var linkPath = ResolvePath(xLink);
                        var shellItem = Shell.Shell.GetShellItemForPath(linkPath);
                        if (shellItem == null) continue;

                        var appInfo = GetAppInfoFromLink(shellItem, order, groupName);
                        appInfos.Add(appInfo);
                        order++;
                    }
                    else
                    {
                        var xPackage = xItem.Attribute("AppUserModelID")?.ToString();
                        if (xItem.LocalName() != "DesktopApplicationTile" || string.IsNullOrEmpty(xPackage)) continue;

                        var appInfo = GetAppInfoFromAppId(xPackage, order, groupName);

                        appInfos.Add(appInfo);
                        order++;
                    }
                }
            }
        }

        private PinnedAppInfo PrefetchRunInfo(PinnedAppInfo pinnedApp)
        {
            if (!Settings.FeatureFlag(AppSettings.FF_EnableRunInfoFromWindowsPrefetch, false)) return pinnedApp;

            //Initialize run stats from Windows Prefetch
            if (pinnedApp.Executable != null)
            {
                pinnedApp.RunStats.UpdateRunInfo(Prefetch.GetPrefetchInfoNoHash(pinnedApp.Executable));
            }
            else if (pinnedApp.PinnedAppType == PinnedAppInfo.PinnedAppTypeEnum.Package && pinnedApp.AppId != null)
            {
                var installPath = Package.GetPackagePath(pinnedApp.AppId);
                if (installPath != null)
                    pinnedApp.RunStats.UpdateRunInfo(Prefetch.GetPrefetchInfoNoHash(installPath, pinnedApp.AppId));
            }

            return pinnedApp;
        }
        private PinnedAppInfo GetAppInfoFromLink(IShellItem2 shellItem, int order, string? folder = null)
        {
            var shellProperties = shellItem.GetProperties();
            var type = shellProperties.IsStoreApp
                ? PinnedAppInfo.PinnedAppTypeEnum.Package
                : PinnedAppInfo.PinnedAppTypeEnum.Link;

            var title =
                shellItem.GetPropertyValue<string>(TitlePropertyKey) ??
                shellItem.GetPropertyValue<string>(PropertyKey.PKEY_ItemNameDisplay) ??
                "unknown";

            var appId = shellProperties.ApplicationUserModelId;
            if (appId == null && Settings.FeatureFlag<bool>(AppSettings.FF_UseApplicationResolver))
            {
                //Try to get AppUserModelId using the win32 app resolver
                appId = WndAndApp.GetWindowApplicationUserModelId(shellItem);
            }

            var executable = GetExecutableFromLinkProps(shellProperties);
            if (appId == null && executable != null)
            {
                //appId can be an executable full path, ensure that known folders are transformed to their GUIDs
                appId = Shell.Shell.ReplaceKnownFolderWithGuid(executable);
            }

            var appInfo = PrefetchRunInfo(new PinnedAppInfo(title, order, type, shellProperties, appId, executable, ThemeService.GetTheme() == ThemeType.Dark, folder));
            return appInfo;
        }

        private PinnedAppInfo GetAppInfoFromAppId(string appId, int order, string? folder = null)
        {
            const PinnedAppInfo.PinnedAppTypeEnum type = PinnedAppInfo.PinnedAppTypeEnum.Package;

            var appInfo = PrefetchRunInfo(new PinnedAppInfo(appId, order, type, new ShellPropertiesSubset(), appId, null, ThemeService.GetTheme() == ThemeType.Dark, folder));
            return appInfo;
        }

        /// <summary>
        /// Returns the executable path from link properties
        /// </summary>
        /// <param name="shellProperties">Link properties to check</param>
        /// <returns>Executable path if extracted from link properties</returns>
        private static string? GetExecutableFromLinkProps(ShellPropertiesSubset shellProperties)
        {
            if (shellProperties.IsStoreApp) return shellProperties.ParsingPath;

            var executable = shellProperties.LinkTargetParsingPath;
            if (executable == null) return executable ?? shellProperties.ParsingPath;
            if (File.Exists(executable) && Path.GetExtension(executable).ToLowerInvariant() == ".exe") return executable;

            //sometimes the LinkTargetParsingPath doesn't contain executable. Probably some issue with app or task bar. Try to get executable from link then

            // ReSharper disable SuspiciousTypeConversion.Global
            if (shellProperties.ParsingPath == null ||
                !File.Exists(shellProperties.ParsingPath) ||
                new CShellLink() is not (IShellLinkW link and IPersistFile linkPersistFile))
                return executable;
            // ReSharper restore SuspiciousTypeConversion.Global
            linkPersistFile.Load(shellProperties.ParsingPath, 0);

            //get basic link information
            var sb = new StringBuilder(260);
            var data = new WIN32_FIND_DATAW();
            var ret = link.GetPath(sb, sb.Capacity, data, 0);
            executable = ret.IsSuccess && sb.Length > 0 ? sb.ToString() : null;

            return executable ?? shellProperties.ParsingPath;
        }
    }

    /// <summary>
    /// XML element extensions
    /// </summary>
    internal static class XElementExtensions
    {
        /// <summary>
        /// Gets the integer value of attribute 
        /// </summary>
        /// <param name="element">Element owning the attribute</param>
        /// <param name="name">Name of the attribute</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Integer value of attribute or default value if attribute doesn't exist or can't be parsed</returns>
        public static int AttributeInt(this XElement element, string name, int defaultValue)
        {
            var strVal = element.Attribute(name)?.Value;
            if (strVal == null) return defaultValue;

            return int.TryParse(strVal, out var intVal) ? intVal : defaultValue;
        }

        /// <summary>
        /// Gets the local name of the element
        /// </summary>
        /// <param name="element">Element</param>
        /// <returns>Local name of element</returns>
        public static string LocalName(this XElement element)
        {
            return element.Name.LocalName;
        }
    }
}

