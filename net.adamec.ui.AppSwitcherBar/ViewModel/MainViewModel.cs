using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using net.adamec.ui.AppSwitcherBar.Config;
using net.adamec.ui.AppSwitcherBar.Dto;
using net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces.Extensions;
using net.adamec.ui.AppSwitcherBar.Win32.Services;
using net.adamec.ui.AppSwitcherBar.Win32.Services.JumpLists;
using net.adamec.ui.AppSwitcherBar.Win32.Services.Shell;
using net.adamec.ui.AppSwitcherBar.Win32.Services.Shell.Properties;
using net.adamec.ui.AppSwitcherBar.WpfExt;
using Wpf.Ui.Appearance;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using static net.adamec.ui.AppSwitcherBar.Dto.PinnedAppInfo;
using MenuItem = System.Windows.Controls.MenuItem;
using RelayCommand = net.adamec.ui.AppSwitcherBar.WpfExt.RelayCommand;

// ReSharper disable StringLiteralTypo
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace net.adamec.ui.AppSwitcherBar.ViewModel
{
    /// <summary>
    /// The ViewModel for <see cref="MainWindow"/>.
    /// Encapsulates the data and logic related to "task bar applications/windows"
    ///  - pulling the list of them, switching the apps, presenting the thumbnails
    /// </summary>
    public partial class MainViewModel : INotifyPropertyChanged
    {

        /// <summary>
        /// Native handle (HWND) of the <see cref="MainWindow"/>
        /// </summary>
        private IntPtr mainWindowHwnd;
        /// <summary>
        /// Native handle of the application window thumbnail currently shown
        /// </summary>
        private IntPtr thumbnailHandle;

        /// <summary>
        /// <see cref="DispatcherTimer"/> used to periodically pull (refresh) the information about (open) application windows
        /// </summary>
        private readonly DispatcherTimer timer;

        /// <summary>
        /// Flag whether the <see cref="RefreshAllWindowsCollection"/> method is run for the first time
        /// </summary>
        private bool isFirstRun = true;


        /// <summary>
        /// Native handle of the last known foreground window
        /// </summary>
        private IntPtr lastForegroundWindow = IntPtr.Zero;

        /// <summary>
        /// Information about the applications installed in system
        /// </summary>
        private InstalledApplications InstalledApplications => backgroundDataService.InstalledApplications; //{ get; } = new();

        /// <summary>
        /// Information about the applications pinned in the taskbar
        /// </summary>
        internal PinnedAppInfo[] PinnedApplications { get; private set; } = Array.Empty<PinnedAppInfo>();

        /// <summary>
        /// Information about the known folder paths and GUIDs
        /// </summary>
        private StringGuidPair[] knownFolders = Array.Empty<StringGuidPair>();

        /// <summary>
        /// Dictionary of known AppIds from configuration containing pairs executable-appId (the key is in lower case)
        /// When built from configuration, the record (key) is created for full path from config and another one without a path (file name only) if applicable
        /// </summary>
        private readonly Dictionary<string, string> knownAppIds;

        /// <summary>
        /// Application settings
        /// </summary>
        public IAppSettings Settings { get; }


        /// <summary>
        /// Flag whether the app uses the dark theme 
        /// </summary>
        private bool isDarkTheme;

        /// <summary>
        /// Flag whether the app uses the dark theme  
        /// </summary>
        public bool IsDarkTheme
        {
            get => isDarkTheme;
            set
            {
                if (isDarkTheme != value)
                {
                    isDarkTheme = value;
                    OnPropertyChanged();
                }
            }
        }
        
        /// <summary>
        /// Array of the information about all monitors (displays)
        /// </summary>
        public MonitorInfo[] AllMonitors { get; }

        /// <summary>
        /// Application window buttons group manager
        /// </summary>
        public AppButtonManager ButtonManager { get; }

        /// <summary>
        /// Command requesting an "ad-hoc" refresh of the list of application windows (no param used)
        /// </summary>
        public ICommand RefreshWindowCollectionCommand { get; }
        /// <summary>
        /// Command requesting the toggle of the application window.
        /// Switch the application window to foreground or minimize it
        /// The command parameter is HWND of the application window
        /// </summary>
        public ICommand ToggleApplicationWindowCommand { get; }
        /// <summary>
        /// Command requesting the render of application window thumbnail into the popup
        /// The command parameter is fully populated <see cref="ThumbnailPopupCommandParams"/> object
        /// </summary>
        public ICommand ShowThumbnailCommand { get; }
        /// <summary>
        /// Command requesting to hide application window thumbnail (no param used)
        /// </summary>
        public ICommand HideThumbnailCommand { get; }

        /// <summary>
        /// Command requesting to build the context menu for application window button
        /// </summary>
        public ICommand BuildContextMenuCommand { get; }

        /// <summary>
        /// Command requesting to launch pinned application
        /// </summary>
        public ICommand LaunchPinnedAppCommand { get; }

        /// <summary>
        /// Flag whether the menu popup is active  
        /// </summary>
        private bool isInMenuPopup;
        /// <summary>
        /// Flag whether the menu popup is active 
        /// </summary>
        public bool IsInMenuPopup
        {
            get => isInMenuPopup;
            set
            {
                if (isInMenuPopup != value)
                {
                    isInMenuPopup = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// JumpList service to be used
        /// </summary>
        private readonly IJumpListService jumpListService;

        /// <summary>
        /// Language  service to be used
        /// </summary>
        private readonly ILanguageService languageService;

        /// <summary>
        /// Background data service to be used
        /// </summary>
        private readonly IBackgroundDataService backgroundDataService;


        /// <summary>
        /// Name of the Feature Flag for windows anonymization
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private const string FF_AnonymizeWindows = "AnonymizeWindows";

        /// <summary>
        /// Name of the Feature Flag for using the undocumented application resolver to get the app it
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public const string FF_UseApplicationResolver = "UseApplicationResolver";

        /// <summary>
        /// Name of the Feature Flag for keeping the menu popup open even when another app is active
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private const string FF_KeepMenuPopupOpen = "KeepMenuPopupOpen";

        /// <summary>
        /// Map used for simple anonymization
        /// </summary>
        /// 
        private readonly Dictionary<char, char> anonymizeMap = new();

        /// <summary>
        /// Internal CTOR
        /// Directly used by <see cref="ViewModelLocator"/> when creating a design time instance.
        /// Internally called by public "DI bound" CTOR
        /// </summary>
        /// <param name="settings">Application setting</param>
        /// <param name="logger">Logger to be used</param>
        /// <param name="jumpListService">JumpList service to be used</param>
        /// <param name="languageService">Language service to be used</param>
        /// <param name="backgroundDataService">Background Data service to be used</param>
        internal MainViewModel(IAppSettings settings, ILogger logger, IJumpListService jumpListService, ILanguageService languageService, IBackgroundDataService backgroundDataService)
        {
            this.logger = logger;
            this.jumpListService = jumpListService;
            this.languageService = languageService;
            this.backgroundDataService = backgroundDataService;

            Settings = settings;
            AllMonitors = Monitor.GetAllMonitors();
            ButtonManager = new AppButtonManager(Settings);

            RefreshWindowCollectionCommand = new RelayCommand(RefreshAllWindowsCollection);
            ToggleApplicationWindowCommand = new RelayCommand(ToggleApplicationWindow);
            ShowThumbnailCommand = new RelayCommand(ShowThumbnail);
            HideThumbnailCommand = new RelayCommand(HideThumbnail);
            BuildContextMenuCommand = new RelayCommand(BuildContextMenu);
            LaunchPinnedAppCommand = new RelayCommand(LaunchPinnedApp);

            knownAppIds = settings.GetKnowAppIds();

            if (settings.FeatureFlag(FF_AnonymizeWindows, false))
            {
                InitAnonymizeMap();
            }

            timer = new DispatcherTimer(DispatcherPriority.Render)
            {
                Interval = TimeSpan.FromMilliseconds(Settings.RefreshWindowInfosIntervalMs)
            };
            timer.Tick += (_, _) => { if (!ButtonManager.IsBusy) RefreshAllWindowsCollection(false); };
           
            this.languageService = languageService;
        }


        /// <summary>
        /// CTOR used by DI
        /// </summary>
        /// <param name="options">Application settings configuration</param>
        /// <param name="logger">Logger to be used</param>
        /// <param name="languageService">Language service to be used</param>
        /// <param name="jumpListService">JumpList service to be used</param>
        /// <param name="backgroundDataService">Background Data service to be used</param>
        // ReSharper disable once UnusedMember.Global
        public MainViewModel(IOptions<AppSettings> options, ILogger<MainViewModel> logger, IJumpListService jumpListService, ILanguageService languageService, IBackgroundDataService backgroundDataService)
            : this(options.Value, logger, jumpListService,languageService,backgroundDataService)
        {
            //used from DI - DI populates the parameters and the internal CTOR is called then
        }


        /// <summary>
        /// (Late) initialize the view model.
        /// Registers the native handle (HWND) of the <see cref="MainWindow"/> and
        /// starts the <see cref="timer"/> used to periodically populate the information about application windows
        /// </summary>
        /// <param name="mainWndHwnd">Native handle (HWND) of the <see cref="MainWindow"/></param>
        public void Init(IntPtr mainWndHwnd)
        {
            mainWindowHwnd = mainWndHwnd;

            //init theme
            ThemeType theme;
            switch (Settings.StartupTheme)
            {
                case StartupThemeEnum.System:
                    {
                        var systemTheme = Theme.GetSystemTheme();
                        var isSystemThemeDark = systemTheme is SystemThemeType.Dark or SystemThemeType.CapturedMotion or SystemThemeType.Glow;
                        theme = isSystemThemeDark ? ThemeType.Dark : ThemeType.Light;
                        break;
                    }
                case StartupThemeEnum.Light:
                    theme = ThemeType.Light;
                    break;
                case StartupThemeEnum.Dark:
                default:
                    theme = ThemeType.Dark;
                    break;
            }

            Theme.Apply(theme, BackgroundType.None, true, true);
            IsDarkTheme = Theme.GetAppTheme() == ThemeType.Dark;

            //start app windows refresh timer
            timer.Start();
        }



        /// <summary>
        /// Initialize character map for simple anonymization
        /// </summary>
        private void InitAnonymizeMap()
        {
            var rnd = new Random(DateTime.Now.GetHashCode());
            for (var c = 'a'; c <= 'z'; c++)
            {
                anonymizeMap[c] = (char)('a' + rnd.Next(26));
            }
            for (var c = 'A'; c <= 'Z'; c++)
            {
                anonymizeMap[c] = (char)('A' + rnd.Next(26));
            }
            for (var c = '0'; c <= '9'; c++)
            {
                anonymizeMap[c] = (char)('0' + rnd.Next(26));
            }
            foreach (var c in " -.:/")
            {
                anonymizeMap[c] = (char)('a' + rnd.Next(26));
            }
        }

        /// <summary>
        /// Simple anonymize given string
        /// </summary>
        /// <param name="s">string to anonymize</param>
        /// <param name="saltInt">anonymization salt</param>
        /// <returns>anonymized string</returns>
        private string? Anonymize(string? s, int saltInt)
        {
            if (s == null) return null;
            var salt = saltInt.ToString();
            while (salt.Length < s.Length)
            {
                salt += salt;
            }

            var retVal = string.Empty;
            for (var i = 0; i < s.Length; i++)
            {
                var c = s[i];
                var cs = (char)(c + salt[i]);
                retVal += anonymizeMap.TryGetValue(cs, out var a) ? a : anonymizeMap.TryGetValue(c, out a) ? a : c;
            }

            return retVal;
        }


        /// <summary>
        /// Pulls the information about available application windows and updates <see cref="ButtonManager"/> window collection.
        /// </summary>
        /// <param name="hardRefresh">When the parameter is bool and true, it forces the hard refresh.
        /// The <see cref="ButtonManager"/>window collection is cleared first and the background data are refreshed on hard refresh.
        ///  Otherwise just the window collection is updated</param>
        internal void RefreshAllWindowsCollection(object? hardRefresh)
        {
            var isHardRefresh = (hardRefresh is bool b || bool.TryParse(hardRefresh?.ToString(), out b)) && b;

            if (isFirstRun)
            {
                isHardRefresh = true;
                isFirstRun = false;
            }

            if (isHardRefresh)
            {
                //get known folders
                knownFolders = Shell.GetKnownFolders();
                //get information about pinned applications
                PinnedApplications = jumpListService.GetPinnedApplications(knownFolders);

                ButtonManager.BeginHardRefresh(PinnedApplications);

                //Refresh also init data
                backgroundDataService.Refresh();
            }
            else
            {
                //begin update of windows collection
                ButtonManager.BeginUpdate();
            }

            //Retrieve the current foreground window
            var foregroundWindow = WndAndApp.GetForegroundWindow();
            if (foregroundWindow != mainWindowHwnd)
            {
                lastForegroundWindow = foregroundWindow; //"filter out" the main window as being the foreground one to proper handle the toggle

                if (IsInMenuPopup && !Settings.FeatureFlag(FF_KeepMenuPopupOpen, false)) IsInMenuPopup = false; //cancel popup/search when other app get's focus 
            }

            //Enum windows
            WndAndApp.EnumVisibleWindows(
                mainWindowHwnd,
                hwnd => ButtonManager[hwnd],
                (hwnd, wnd, caption, threadId, processId, ptrProcess) =>
                {
                    //caption anonymization
                    if (Settings.FeatureFlag<bool>(FF_AnonymizeWindows))
                    {
                        var appName =
                            InstalledApplications.GetInstalledApplicationFromAppId(wnd?.AppId ?? string.Empty)?.Name ??
                            InstalledApplications.GetInstalledApplicationFromExecutable(wnd?.Executable ?? string.Empty)?.Name;

                        if (caption != appName)
                        {
                            caption = (string.IsNullOrEmpty(appName) ? "" : $"{appName} - ") + Anonymize(caption, hwnd.ToInt32());
                        }
                    }

                    if (caption.ToLower().Contains("task ma"))
                    {

                    }

                    //Check whether it's a "new" application window or a one already existing in the ButtonManager
                    if (wnd == null)
                    {
                        //app executable 
                        var executable = WndAndApp.GetProcessExecutable(ptrProcess);
                        //new window
                        wnd = new WndInfo(hwnd, caption, threadId, processId, executable);

                        //Try to get AppUserModelId using the win32 app resolver, no need to wait for background data
                        if (Settings.FeatureFlag<bool>(FF_UseApplicationResolver))
                            wnd.AppId = WndAndApp.GetWindowApplicationUserModelId(hwnd);
                    }
                    else
                    {
                        //existing (known) window
                        wnd.MarkToKeep(); //reset the "remove from collection" flag
                        wnd.Title = caption; //update the title
                    }

                    wnd.IsForeground = hwnd == lastForegroundWindow; //check whether the window is foreground window (will be highlighted in UI)


                    if ((wnd.AppId is null || Settings.CheckForAppIdChange) && backgroundDataService.BackgroundDataRetrieved) // || isHardRefresh - not needed as wnd will be new with AppId =null
                    {
                        string? appUserModelId = null;

                        //Try to get AppUserModelId using the win32 app resolver
                        if (Settings.FeatureFlag<bool>(FF_UseApplicationResolver))
                        {
                            appUserModelId = WndAndApp.GetWindowApplicationUserModelId(hwnd);
                        }

                        //Try to get AppUserModelId from window - for windows that explicitly define the AppId 
                        if (appUserModelId == null)
                        {
                            var store = Shell.GetPropertyStoreForWindow(wnd.Hwnd);
                            if (store != null)
                            {
                                var hr = store.GetCount(out var c);
                                if (hr.IsSuccess && c > 0)
                                {
                                    //try to get AppUserModelId property 
                                    appUserModelId = store.GetPropertyValue<string>(PropertyKey.PKEY_AppUserModel_ID);
                                    var shellProperties = store.GetProperties();
                                    wnd.ShellProperties = shellProperties;
                                }
                            }
                        }

                        //Try the app ids from configuration
                        //It must contain the record for (shell) explorer (done in CTOR) as it will not work properly for explorer windows without this hack
                        if (appUserModelId == null && wnd.Executable != null &&
                            (knownAppIds.TryGetValue(wnd.Executable.ToLowerInvariant(), out var appId) ||
                             knownAppIds.TryGetValue(Path.GetFileName(wnd.Executable.ToLowerInvariant()), out appId)))
                        {
                            appUserModelId = appId;
                        }


                        //try to get AppUserModelId from process if not "at window"
                        appUserModelId ??= WndAndApp.GetProcessApplicationUserModelId(ptrProcess);

                        if (appUserModelId == null && !string.IsNullOrEmpty(wnd.Executable))
                        {
                            //try to get from installed app (identified by executable) or use executable as fallback
                            appUserModelId = InstalledApplications.GetAppIdFromExecutable(wnd.Executable, out var _);
                        }

                        wnd.AppId = appUserModelId;
                    }



                    if (Settings.CheckForIconChange || isHardRefresh)
                    {
                        //Try to retrieve the window icon
                        wnd.BitmapSource = WndAndApp.GetWindowIcon(hwnd);

                        if (wnd.BitmapSource == null)
                        {
                            //try to get icon from installed application
                            if (!string.IsNullOrEmpty(wnd.AppId))
                            {
                                wnd.BitmapSource = InstalledApplications.GetInstalledApplicationFromAppId(wnd.AppId)?.IconSource;
                            }
                        }

                        wnd.BitmapSource = InvertBitmapIfApplicable(wnd.BitmapSource);
                    }

                    //Add new window to button manager and the button windows collections
                    if (wnd.ChangeStatus == WndInfo.ChangeStatusEnum.New)
                    {
                        ButtonManager.Add(wnd);
                    }

                    if (isHardRefresh)
                    {
                        LogEnumeratedWindowInfo(wnd.ToString());
                    }

                }); //enum visible windows

            ButtonManager.EndUpdate();

        }

        /// <summary>
        /// Inverts the bitmap when in the light scheme, <see cref="IAppSettings.InvertWhiteIcons"/> is set and the bitmap is white pixels only or
        /// in the dark scheme, <see cref="IAppSettings.InvertBlackIcons"/> is set and the bitmap is black pixels only
        /// </summary>
        /// <param name="bitmapSource"></param>
        /// <returns></returns>
        private BitmapSource? InvertBitmapIfApplicable(BitmapSource? bitmapSource)
        {
            if (bitmapSource == null) return null;
            if (Settings.InvertWhiteIcons && !IsDarkTheme) return Resource.InvertBitmapIfWhiteOnly(bitmapSource);
            if (Settings.InvertBlackIcons && IsDarkTheme) return Resource.InvertBitmapIfBlackOnly(bitmapSource);
            return bitmapSource;
        }

        /// <summary>
        /// Switch the application window with given <paramref name="hwnd"/> to foreground or minimize it.
        /// </summary>
        /// <remarks>
        /// The function doesn't throw any exception when the handle is invalid, it just ignores it end "silently" returns
        /// </remarks>
        /// <param name="hwnd">Native handle (HWND) of the application window</param>
        private void ToggleApplicationWindow(object? hwnd)
        {
            if (hwnd is not IntPtr hWndIntPtr || hWndIntPtr == IntPtr.Zero) return; //invalid command parameter, do nothing
            ToggleApplicationWindow(hWndIntPtr, false);
        }

        /// <summary>
        /// Switch the application window with given <paramref name="hwnd"/> to foreground or minimize it (if <paramref name="forceActivate"/> is not set).
        /// </summary>
        /// <remarks>
        /// The function doesn't throw any exception when the handle is invalid, it just ignores it end "silently" returns
        /// </remarks>
        /// <param name="hwnd">Native handle (HWND) of the application window</param>
        /// <param name="forceActivate">When the flag is set, the window is always activated. When it's false and the window is foreground already, it's minimized</param>
        public void ToggleApplicationWindow(IntPtr hwnd, bool forceActivate)
        {
            if (hwnd == IntPtr.Zero) return; //invalid command parameter, do nothing

            //got the handle, get the window information
            var wnd = ButtonManager[hwnd];
            if (wnd is null) return; //unknown window, do nothing

            if (wnd.IsForeground && !forceActivate)
            {
                //it's a foreground window - minimize it and return
                WndAndApp.MinimizeWindow(hwnd);
                LogMinimizeApp(hwnd, wnd.Title);
                return;
            }

            var wasMinimized = WndAndApp.ActivateWindow(hwnd);
            LogSwitchApp(hwnd, wnd.Title, wasMinimized);

            //refresh the window list
            RefreshAllWindowsCollection(false);
        }

        /// <summary>
        /// Registers and shows the application window thumbnail within the popup
        /// Parameter <paramref name="param"/> must be <see cref="ThumbnailPopupCommandParams"/> object,
        /// encapsulating <see cref="ThumbnailPopupCommandParams.SourceHwnd"/> of application window,
        /// <see cref="ThumbnailPopupCommandParams.TargetHwnd"/> of the popup window and
        /// <see cref="ThumbnailPopupCommandParams.TargetRect"/> with the bounding box within the popup window.
        /// </summary>
        /// <param name="param"><see cref="ThumbnailPopupCommandParams"/> object with source and target information</param>
        /// <exception cref="ArgumentException">When the <paramref name="param"/> is not <see cref="ThumbnailPopupCommandParams"/> object or is null, <see cref="ArgumentException"/> is thrown</exception>
        private void ShowThumbnail(object? param)
        {
            if (param is not ThumbnailPopupCommandParams cmdParams)
            {
                LogWrongCommandParameter(nameof(ThumbnailPopupCommandParams));
                throw new ArgumentException($"Command parameter must be {nameof(ThumbnailPopupCommandParams)}", nameof(param));
            }

            HideThumbnail(); //unregister (hide) existing thumbnail if any
            if (cmdParams.SourceHwnd == IntPtr.Zero) return;

            thumbnailHandle = Thumbnail.ShowThumbnail(cmdParams.SourceHwnd, cmdParams.TargetHwnd, (Rect)cmdParams.TargetRect, out var thumbCentered);

            LogShowThumbnail(cmdParams.SourceHwnd, cmdParams.TargetHwnd, thumbCentered, thumbnailHandle);
        }

        /// <summary>
        /// Unregister (hide) the existing thumbnail identified by <see cref="MainViewModel.thumbnailHandle"/>
        /// </summary>
        /// <remarks>
        /// When there is no thumbnail (<see cref="MainViewModel.thumbnailHandle"/> is <see cref="IntPtr.Zero"/>),
        /// no exception is thrown and the method "silently" returns
        /// </remarks>
        private void HideThumbnail()
        {
            if (thumbnailHandle == IntPtr.Zero) return;

            Thumbnail.HideThumbnail(thumbnailHandle);
            LogHideThumbnail(thumbnailHandle);
            thumbnailHandle = IntPtr.Zero;
        }

        /// <summary>
        /// Launches the pinned application
        /// Parameter <paramref name="param"/> must be <see cref="PinnedAppInfo"/> object
        /// </summary>
        /// <param name="param"><see cref="PinnedAppInfo"/> object with reference to pinned application</param>
        /// <exception cref="ArgumentException">When the <paramref name="param"/> is not <see cref="PinnedAppInfo"/> object or is null, <see cref="ArgumentException"/> is thrown</exception>

        internal void LaunchPinnedApp(object? param)
        {
            if (param is not PinnedAppInfo pinnedAppInfo)
            {
                LogWrongCommandParameter(nameof(PinnedAppInfo));
                throw new ArgumentException($"Command parameter must be {nameof(PinnedAppInfo)}", nameof(param));
            }

            pinnedAppInfo.LaunchPinnedApp(e =>
            {
                LogCantStartApp(pinnedAppInfo.PinnedAppType == PinnedAppTypeEnum.Package ? pinnedAppInfo.AppId ?? "[Null] appID" : pinnedAppInfo.LinkFile ?? "[Null] link file", e);
            });
        }

        /// <summary>
        /// Launches the installed application
        /// Parameter <paramref name="param"/> must be <see cref="InstalledApplication"/> object
        /// </summary>
        /// <param name="param"><see cref="InstalledApplication"/> object with reference to installed application</param>
        /// <exception cref="ArgumentException">When the <paramref name="param"/> is not <see cref="InstalledApplication"/> object or is null, <see cref="ArgumentException"/> is thrown</exception>

        internal void LaunchInstalledApp(object? param)
        {
            if (param is not InstalledApplication installedApplication)
            {
                LogWrongCommandParameter(nameof(InstalledApplication));
                throw new ArgumentException($"Command parameter must be {nameof(InstalledApplication)}", nameof(param));
            }

            installedApplication.LaunchInstalledApp(e =>
            {
                LogCantStartApp(installedApplication.ShellProperties.IsStoreApp ? installedApplication.AppUserModelId ?? "[Null] appID" : installedApplication.Executable ?? "[Null] file", e);
            });
        }

        /// <summary>
        /// Builds the context menu for application window button
        /// Parameter <paramref name="param"/> must be <see cref="BuildContextMenuCommandParams"/> object
        /// </summary>
        /// <param name="param"><see cref="BuildContextMenuCommandParams"/> object with reference to <see cref="AppButton"/> and <see cref="ButtonInfo"/></param>
        /// <exception cref="ArgumentException">When the <paramref name="param"/> is not <see cref="BuildContextMenuCommandParams"/> object or is null, <see cref="ArgumentException"/> is thrown</exception>
        private void BuildContextMenu(object? param)
        {
            if (param is not BuildContextMenuCommandParams cmdParams)
            {
                LogWrongCommandParameter(nameof(BuildContextMenuCommandParams));
                throw new ArgumentException($"Command parameter must be {nameof(BuildContextMenuCommandParams)}",
                    nameof(param));
            }

            MenuItem menuItem;
            var menu = new ContextMenu();

            var buttonInfo = cmdParams.ButtonInfo;
            WndInfo? wndInfo = null;
            if (buttonInfo is WndInfo wi)
            {
                wndInfo = wi;
            }

            var isWindow = wndInfo != null;

            var appId = buttonInfo.AppId;

            if (appId != null)
            {
                //appId can be an executable full path, ensure that known folders are transformed to their GUIDs
                appId = Shell.ReplaceKnownFolderWithGuid(appId);

                //JumpList into the context menu
                var jumplistItems = jumpListService.GetJumpListItems(appId!, InstalledApplications);
                if (jumplistItems.Length > 0)
                {
                    string? lastCategory = null;
                    string localizedTasks = languageService.Translate(TranslationKeys.JumpListCategoryTasks) ?? "Tasks";

                    foreach (var linkInfo in jumplistItems.Where(l => l.HasTarget)) //skip separators for UI simplicity
                    {
                        if (linkInfo.Category != lastCategory)
                        {
                            //category title
                            menuItem = new MenuItem
                            {
                                Header = linkInfo.Category,
                                IsEnabled = false
                            };
                            menu.Items.Add(menuItem);

                            lastCategory = linkInfo.Category;
                        }

                        //jumplist item
                        menuItem = new MenuItem
                        {
                            Header = linkInfo.Name
                        };

                        //caption anonymization
                        if (Settings.FeatureFlag<bool>(FF_AnonymizeWindows) && linkInfo.Category != localizedTasks)
                        {
                            menuItem.Header = Anonymize(linkInfo.Name, linkInfo.GetHashCode());
                        }

                        if (linkInfo.Icon != null)
                        {
                            menuItem.Icon = new Image
                            {
                                Source = InvertBitmapIfApplicable(linkInfo.Icon)
                            };
                        }
                        else
                        {
                            //use app icon
                            menuItem.Icon = new Image
                            {
                                Source = InvertBitmapIfApplicable(buttonInfo.BitmapSource)
                            };
                        }

                        menuItem.Click += (_, _) =>
                        {
                            try
                            {
                                if (!linkInfo.IsStoreApp)
                                {
                                    Process.Start(new ProcessStartInfo(linkInfo.TargetPath!)
                                    {
                                        Arguments = linkInfo.Arguments,
                                        WorkingDirectory = linkInfo.WorkingDirectory,
                                        UseShellExecute = true
                                    });
                                }
                                else
                                {
                                    Package.ActivateApplication(linkInfo.TargetPath, linkInfo.Arguments, out _);
                                }
                            }
                            catch (Exception ex)
                            {
                                LogCantStartApp(linkInfo.ToString(), ex);
                            }

                        };
                        menu.Items.Add(menuItem);
                    }

                    menu.Items.Add(new Separator());
                }
            }


            BuildContextMenuItemLaunchNewInstance(isWindow, buttonInfo, menu);

            if (isWindow)
            {
                //close window menu item
                menuItem = new MenuItem
                {
                    Header = languageService.Translate(TranslationKeys.JumpListMenuCloseWindow),
                    Icon = new SymbolIcon()
                    {
                        Symbol = SymbolRegular.DismissSquare20
                    }
                };
                menuItem.Click += (_, _) => { WndAndApp.CloseWindow(wndInfo!.Hwnd); };
                menu.Items.Add(menuItem);
            }

            menu.Items.Add(new Separator());

            //close menu (cancel) menu item
            menuItem = new MenuItem
            {
                Header = languageService.Translate(TranslationKeys.JumpListMenuCancel),
                Icon = new SymbolIcon()
                {
                    Symbol = SymbolRegular.ShareCloseTray20
                }
            };
            menuItem.Click += (_, _) =>
            {
                //do nothing, just close the context menu
            };
            menu.Items.Add(menuItem);

            cmdParams.Button.ContextMenu = menu;
        }

        /// <summary>
        /// Builds the context menu item for launching a new instance of application
        /// </summary>
        /// <param name="isWindow">Flag whether the context menu is for window button</param>
        /// <param name="buttonInfo">Information about application window or pinned app </param>
        /// <param name="menu">Context menu</param>
        private void BuildContextMenuItemLaunchNewInstance(bool isWindow, ButtonInfo buttonInfo, ContextMenu menu)
        {
            if (isWindow)
            {
                //Start new instance menu item (window)
                if (!File.Exists(buttonInfo.Executable)) return;

                var appName =
                    InstalledApplications.GetInstalledApplicationFromAppId(buttonInfo.AppId ?? string.Empty)?.Name ??
                    InstalledApplications.GetInstalledApplicationFromExecutable(buttonInfo.Executable)?.Name ??
                    FileVersionInfo.GetVersionInfo(buttonInfo.Executable).FileDescription ??
                    Path.GetFileName(buttonInfo.Executable);

                var menuItem = new MenuItem
                {
                    Header = appName,
                    Icon = new Image
                    {
                        Source = InvertBitmapIfApplicable(buttonInfo.BitmapSource)
                    }
                };
                menuItem.Click += (_, _) =>
                {
                    if (buttonInfo.Executable.ToLowerInvariant().EndsWith("\\explorer.exe"))
                    {
                        //explorer and the "special" folders like control panel
                        try
                        {
                            Process.Start(new ProcessStartInfo("explorer")
                            {
                                Arguments = buttonInfo.AppId != null
                                    ? $"shell:appsFolder\\{buttonInfo.AppId}"
                                    : null,
                            });
                        }
                        catch (Exception ex)
                        {
                            LogCantStartApp(appName, ex);
                        }
                    }
                    else
                    {
                        var started = false;
                        if (!buttonInfo.Executable.ToLowerInvariant().EndsWith("\\applicationframehost.exe"))
                        {
                            try
                            {
                                Process.Start(buttonInfo.Executable);
                                started = true;
                            }
                            catch (Exception ex)
                            {
                                LogCantStartApp(appName, ex);
                            }
                        }

                        if (started || buttonInfo.AppId == null) return;

                        //maybe store/UWP app
                        try
                        {
                            Package.ActivateApplication(buttonInfo.AppId, null, out _);
                        }
                        catch (Exception ex)
                        {
                            LogCantStartApp(appName, ex);
                        }
                    }
                };
                menu.Items.Add(menuItem);
            }
            else
            {
                //Start new instance menu item (pinned app)
                if (buttonInfo is not PinnedAppInfo pinnedAppInfo) return;
                var menuItem = new MenuItem
                {
                    Header = pinnedAppInfo.Title,
                    Icon = new Image
                    {
                        Source = InvertBitmapIfApplicable(pinnedAppInfo.BitmapSource)
                    }
                };
                menuItem.Click += (_, _) => { LaunchPinnedApp(pinnedAppInfo); };
                menu.Items.Add(menuItem);
            }
        }

        /// <summary>
        /// Occurs when a property value changes
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raise <see cref="PropertyChanged"/> event for given <paramref name="propertyName"/>
        /// </summary>
        /// <param name="propertyName">Name of the property changed</param>
        // ReSharper disable once UnusedMember.Global
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


}
