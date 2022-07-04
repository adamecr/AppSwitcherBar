using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using net.adamec.ui.AppSwitcherBar.AppBar;
using net.adamec.ui.AppSwitcherBar.Config;
using net.adamec.ui.AppSwitcherBar.Dto;
using net.adamec.ui.AppSwitcherBar.Win32;
using net.adamec.ui.AppSwitcherBar.Win32.ShellExt;
using net.adamec.ui.AppSwitcherBar.Wpf;
using static net.adamec.ui.AppSwitcherBar.Win32.Win32Consts;

namespace net.adamec.ui.AppSwitcherBar.ViewModel
{
    /// <summary>
    /// The ViewModel for <see cref="MainWindow"/>.
    /// Encapsulates the data and logic related to "task bar applications/windows"
    ///  - pulling the list of them, switching the apps, presenting the thumbnails
    /// </summary>
    public partial class MainViewModel : INotifyPropertyChanged
    {
        #region Logging
        /// <summary>
        /// Logger used
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Logs record (Critical) about the wrong parameter of <see cref="ShowThumbnailCommand"/> (null or wrong type)
        /// </summary>
        /// <param name="commandParameterTypeName">Name of expected <see cref="Type"/> of the command parameter value</param>
        [LoggerMessage(
            EventId = 900,
            Level = LogLevel.Critical,
            Message = "Command parameter must be `{commandParameterTypeName}`")]
        private partial void LogWrongCommandParameter(string commandParameterTypeName);

        /// <summary>
        /// Logs record (Debug) when a window thumbnail is shown in popup
        /// </summary>
        /// <param name="sourceHwnd">HWND of the source window to present within the popup as a thumbnail</param>
        /// <param name="targetHwnd">HWND of the popup (thumbnail target)</param>
        /// <param name="targetRegion"><see cref="RECT"/> region of popup where to rended the thumbnail to</param>
        /// <param name="thumbnailHandle">Handle of the thumbnail returned from DWM api</param>
        [LoggerMessage(
            EventId = 101,
            Level = LogLevel.Debug,
            Message = "Show thumbnail of hwnd:{sourceHwnd} in hwnd:{targetHwnd} {targetRegion} - handle is #{thumbnailHandle}")]
        private partial void LogShowThumbnail(IntPtr sourceHwnd, IntPtr targetHwnd, RECT targetRegion, IntPtr thumbnailHandle);

        /// <summary>
        /// Logs record (Debug) when a window thumbnail is unregitered (hidden)
        /// </summary>
        /// <param name="thumbnailHandle">Handle of the thumbnail</param>
        [LoggerMessage(
            EventId = 102,
            Level = LogLevel.Debug,
            Message = "Hide (unregister) thumnail #{thumbnailHandle}")]
        private partial void LogHideThumbnail(IntPtr thumbnailHandle);

        /// <summary>
        /// Logs record (Information) after switching the foreground app window based on the user interaction (click to app button) 
        /// </summary>
        /// <param name="appHwnd">HWND of the application window switched to foreground</param>
        /// <param name="title">Title of the application window switched to foreground</param>
        /// <param name="isMinimized">Information whether the application was minimized before the switch</param>
        [LoggerMessage(
            EventId = 201,
            Level = LogLevel.Information,
            Message = "Switched to app {title} (hwnd:{appHwnd}). Was minimized before: {isMinimized}")]
        private partial void LogSwitchApp(IntPtr appHwnd, string title, bool isMinimized);

        /// <summary>
        /// Logs record (Information) after minimizing the app window based on the user interaction (click to app button) 
        /// </summary>
        /// <param name="appHwnd">HWND of the minimized application window</param>
        /// <param name="title">Title of the minimized application window</param>
        [LoggerMessage(
            EventId = 202,
            Level = LogLevel.Information,
            Message = "App {title} (hwnd:{appHwnd}) was minimized")]
        private partial void LogMinimizeApp(IntPtr appHwnd, string title);
        #endregion 

        /// <summary>
        /// Native handle (HWND) of the <see cref="MainWindow"/>
        /// </summary>
        private IntPtr mainWindowHwnd;
        /// <summary>
        /// Native handle of the application window thumbnail currently shown
        /// </summary>
        private IntPtr thumbnailHandle;

        /// <summary>
        /// Native window style to recognize the top-level application window (WS_BORDER | WS_VISIBLE | WS_EX_APPWINDOW - all must apply)
        /// </summary>
        private const ulong NATIVE_WINDOW_STYLE_FILTER = WS_BORDER | WS_VISIBLE | WS_EX_APPWINDOW;

        /// <summary>
        /// <see cref="DispatcherTimer"/> used to periodically pull (refresh) the information about (open) application windows
        /// </summary>
        private readonly DispatcherTimer timer;

        /// <summary>
        /// Native handle of the last known foreground window
        /// </summary>
        private IntPtr lastForegroundWindow = IntPtr.Zero;

        /// <summary>
        /// Application settings
        /// </summary>
        public IAppSettings Settings { get; }

        /// <summary>
        /// Array of the screen edges the app-bar can be docked to
        /// </summary>
        public AppBarDockMode[] Edges { get; } = new[]{
            AppBarDockMode.Left,
            AppBarDockMode.Right,
            AppBarDockMode.Top,
            AppBarDockMode.Bottom};

        /// <summary>
        /// Array of the information about all monitors (displays)
        /// </summary>
        public MonitorInfo[] AllMonitors { get; }

        /// <summary>
        /// All application windows to be presented as application buttons 
        /// </summary>
        public ObservableCollection<WndInfo> AllWindows { get; } = new ObservableCollection<WndInfo>();

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
        /// Internal CTOR
        /// Directly used by <see cref="ViewModelLocator"/> when creating a design time instance.
        /// Internally called by public "DI bound" CTOR
        /// </summary>
        /// <param name="settings">Application setting</param>
        /// <param name="logger">Logger to be used</param>
        internal MainViewModel(AppSettings settings, ILogger logger)
        {
            this.logger = logger;
            Settings = settings;
            AllMonitors = MonitorInfo.GetAllMonitors().ToArray();
            RefreshWindowCollectionCommand = new RelayCommand(RefreshAllWindowsCollection);
            ToggleApplicationWindowCommand = new RelayCommand(ToggleApplicationWindow);
            ShowThumbnailCommand = new RelayCommand(ShowThumbnail);
            HideThumbnailCommand = new RelayCommand(UnregisterThumbnail);

            timer = new DispatcherTimer(DispatcherPriority.Render)
            {
                Interval = TimeSpan.FromMilliseconds(Settings.RefreshWindowInfosIntervalMs)
            };
            timer.Tick += (_, _) =>
            {
                RefreshAllWindowsCollection(false);
            };

        }

        /// <summary>
        /// CTOR used by DI
        /// </summary>
        /// <param name="options">Application settings configuration</param>
        /// <param name="logger">Logger to be used</param>
        public MainViewModel(IOptions<AppSettings> options, ILogger<MainViewModel> logger) : this(options.Value, logger)
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
            timer.Start();
        }

        /// <summary>
        /// Pulls the information about available application windows and updates <see cref="AllWindows"/> collection.
        /// </summary>
        /// <param name="hardRefresh">When the parameter is bool and true, it forces the hard refresh - the <see cref="AllWindows"/> collection is cleared first. 
        /// Otherwise it's just updated</param>
        private void RefreshAllWindowsCollection(object? hardRefresh)
        {
            //UnregisterThumbnail(); - TODO close thumb when not "valid" anymore

            if ((hardRefresh is bool isHardRefresh || bool.TryParse(hardRefresh?.ToString(), out isHardRefresh)) && isHardRefresh)
            {
                AllWindows.Clear(); //clear the All windows collection
            }
            else
            {
                //Mark existing records for removal - when the record is updated the mark is changed, otherwise the record will be removed from collection at the end of processing
                foreach (var w in AllWindows)
                {
                    w.MarkForRemoval();
                }
            }

            //Retrieve the current foreground window
            var foregroundWindow = User32.GetForegroundWindow();
            if (foregroundWindow != mainWindowHwnd) lastForegroundWindow = foregroundWindow; //"filter out" the main window as being the foreground one to proper handle the toggle

            //Enum windows
            _ = User32.EnumWindows((hwnd, _) =>
            {
                //Filter windows - only the top-level application windows except "itself" 
                var wndStyle = User32.GetWindowLongA(hwnd, GWL_STYLE);
                if (mainWindowHwnd != hwnd &&
                    (wndStyle & NATIVE_WINDOW_STYLE_FILTER) == NATIVE_WINDOW_STYLE_FILTER && //appwindow, with border  and visible
                    (wndStyle & WS_EX_TOOLWINDOW) == 0 &&
                    (wndStyle & WS_CHILD) == 0)
                {
                    //Get window title
                    var sb = new StringBuilder(255);
                    User32.GetWindowText(hwnd, sb, sb.Capacity);
                    var text = sb.ToString();

                    if (!string.IsNullOrWhiteSpace(text)) //ignore the windows without the title
                    {
                        //Check whether it's a "new" application window or a one already existing in the AllWindows collection
                        var wnd = AllWindows.FirstOrDefault(w => w.Hwnd == hwnd);
                        if (wnd == null)
                        {
                            //new window
                            wnd = new WndInfo(hwnd, text);
                        }
                        else
                        {
                            //existing (known) window
                            wnd.MarkToKeep();//reset the "remove from collection" flag
                            wnd.Title = text;//update the title
                        }
                        wnd.IsForeground = hwnd == lastForegroundWindow; //check whether the window is foreground window (will be highlighted in UI)
                        wnd.ThreadHandle = User32.GetWindowThreadProcessId(hwnd, out var process);//get the window thread and process IDs (handles)
                        wnd.ProcessHandle = process;

                        #region Feature Flag "JumpList" - work in progress
                        if (Settings.FeatureFlag<bool>("JumpList"))
                        {
                            //IDEA - provide the right-click menu containing the jumplist and close 
                            // TODOs:
                            //  - get AppUserModelId - explicit from window or explicit from process or ??? 
                            //  - CRC "hash" the AppUserModelId or exec path(?) to get the jumplist file
                            //  - parse and process the jumplist file
                            //  - when will be able to properly get AppUserModelId, it should also be used for grouping of the buttons


                            //try to get AppUserModelId from window - seems to work for windows that explicitly define the AppId, 
                            ShellExtensions.SHGetPropertyStoreForWindow(wnd.Hwnd, out var store);
                            var appUserModelIdPropKey = new PropertyKey(new Guid(Win32Consts.PKEY_AppUserModel_ID), 5);
                            uint c = 0;
                            try
                            {
                                store?.GetCount(out c);
                                if (c != 0)
                                {
                                    //try to get AppUserModelId property 
                                    var appUserModelIdPropVar = new PropVariant();
                                    store!.GetValue(ref appUserModelIdPropKey, appUserModelIdPropVar);
                                    var appUserModelId = appUserModelIdPropVar.Value?.ToString();

                                    //enum properties
                                    var pv = new PropVariant();
                                    for (uint i = 0; i < c; i++)
                                    {
                                        store.GetAt(i, out var propertyKey);
                                        store.GetValue(ref propertyKey, pv);
                                    }
                                }
                            }
                            catch
                            {

                            }

                            //try to get AppUserModelId from process - this doesn't work yet
                            uint appIdLen = 130;
                            sb = new StringBuilder((int)appIdLen);
                            var result = ShellExtensions.GetApplicationUserModelId(process, ref appIdLen, sb);
                            switch (result)
                            {
                                case 0:
                                    wnd.AppId = sb.ToString();
                                    break;
                                case 0x7a: //ERROR_INSUFFICIENT_BUFFER
                                {
                                    sb = new StringBuilder((int)appIdLen);
                                    if (ShellExtensions.GetApplicationUserModelId(process, ref appIdLen, sb) == 0)
                                    {
                                        wnd.AppId = sb.ToString();
                                    }

                                    break;
                                }
                            }
                        }
                        #endregion
                        
                        //Try to retrieve the window icon
                        IntPtr hiconPtr;
                        // Use the WM_GETICON message first
                        var hicon = User32.SendMessageA(hwnd, WM_GETICON, 2, 0);

                        if (hicon == 0)
                        {
                            //When the window doesn't provide icon via WM_GETICON, try to get it from native window class
                            hiconPtr = User32.GetClassLongPtr(hwnd, GCLP_HICONSM);
                            if (hiconPtr == IntPtr.Zero)
                            {
                                hiconPtr = User32.GetClassLongPtr(hwnd, GCLP_HICON);
                            }
                        }
                        else
                        {
                            hiconPtr = new IntPtr(hicon); //Native icon handle from WM_GETICON
                        }

                        //Got the icon?
                        if (hiconPtr != IntPtr.Zero)
                        {
                            //Transform the icon into the bitmap presentable in WPF UI
                            var bitmapSource = Imaging.CreateBitmapSourceFromHIcon(hiconPtr, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                            wnd.BitmapSource = bitmapSource;
                        }

                        //"Sort" window buttons by process
                        if (wnd.ChangeStatus == WndInfo.ChangeStatusEnum.New) //the process handle should not change, so do it just for new windows
                        {
                            var lastFromProcess = AllWindows.LastOrDefault(w => w.ProcessHandle == wnd.ProcessHandle); //any window from the same process already exist in collection?
                            if (lastFromProcess != null)
                            {
                                //there is already a window for the same process
                                var idx = AllWindows.IndexOf(lastFromProcess);
                                idx++;
                                if (idx < AllWindows.Count)
                                {
                                    AllWindows.Insert(idx, wnd); //add to existing process sequence
                                }
                                else
                                {
                                    AllWindows.Add(wnd); //add to end of collection as the windows from the process are the last within the collection
                                }
                            }
                            else
                            {
                                AllWindows.Add(wnd); //new process, add to end
                            }

                        }

                    }
                }

                return true;//end of window enum
            }, 0);

            //Cleanup the collection - remove the windows that were not present in the last enum windows
            var toRemove = AllWindows.Where(w => w.ChangeStatus == WndInfo.ChangeStatusEnum.ToRemove).ToArray();
            foreach (var w in toRemove)
            {
                AllWindows.Remove(w);
            }

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
            if (hwnd is not IntPtr hWndIntPtr) return; //invalid command parameter, do nothing

            if (hWndIntPtr != IntPtr.Zero)
            {
                //got the handle, get the window information
                var wnd = AllWindows.FirstOrDefault(w => w.Hwnd == hWndIntPtr);
                if (wnd is null) return; //unknown window, do nothing

                if (wnd.IsForeground)
                {
                    //it's a foreground window - minimize it and return
                    User32.ShowWindow(hWndIntPtr, SW_MINIMIZE);
                    LogMinimizeApp(hWndIntPtr, wnd.title);
                    return;
                }

                //get the window state
                var isMinimized = (User32.GetWindowLongA(hWndIntPtr, GWL_STYLE) & WS_MINIMIZE) > 0;
                var isVisible = User32.IsWindowVisible(hWndIntPtr);

                //make sure the window is shown and activated
                User32.SetWindowPos(
                    hWndIntPtr,
                    HWND_TOP,
                    0, 0, 0, 0,
                    SetWindowPosFlags.IgnoreMove |
                    SetWindowPosFlags.IgnoreResize |
                    SetWindowPosFlags.ShowWindow |
                    (isVisible ? SetWindowPosFlags.DoNotActivate : 0));

                //restore the minimized window
                if (isMinimized) User32.ShowWindow(hWndIntPtr, SW_RESTORE);

                //set the foreground window
                User32.SetForegroundWindow(hWndIntPtr);
                LogSwitchApp(hWndIntPtr, wnd.title, isMinimized);

                //refresh the window list
                RefreshAllWindowsCollection(false);
            }
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

            UnregisterThumbnail(); //unregister (hide) existing thumbnail if any

            if (cmdParams.SourceHwnd != IntPtr.Zero)
            {
                //Register thumbnail
                if (DwmApi.DwmRegisterThumbnail(cmdParams.TargetHwnd, cmdParams.SourceHwnd, out thumbnailHandle) == 0 && thumbnailHandle != IntPtr.Zero)
                {
                    //Get the size of the thumbnail source window
                    _ = DwmApi.DwmQueryThumbnailSourceSize(thumbnailHandle, out SIZE size);

                    //Scale and center the thumbnail within the bounding box
                    var thumbSize = new Size(size.x, size.y);
                    var thumbSizeScaled = ScaleToBound(thumbSize, cmdParams.TargetRect.Size);
                    var thumbCentered = CenterInRectangle(thumbSizeScaled, (Rect)cmdParams.TargetRect);

                    //Provide the visibility, destination rectangle and opacity parameters to the thumbnail to show it
                    var props = new DWM_THUMBNAIL_PROPERTIES
                    {
                        dwFlags = DWM_TNP_VISIBLE | DWM_TNP_RECTDESTINATION | DWM_TNP_OPACITY,
                        fVisible = true,
                        rcDestination = (RECT)thumbCentered,
                        opacity = 255,
                    };
                    _ = DwmApi.DwmUpdateThumbnailProperties(thumbnailHandle, ref props);

                    LogShowThumbnail(cmdParams.SourceHwnd, cmdParams.TargetHwnd, (RECT)thumbCentered, thumbnailHandle);
                }
            }

        }

        /// <summary>
        /// Centers given <paramref name="thumbSize"/> within the <paramref name="boundingBox"/>
        /// </summary>
        /// <param name="thumbSize"><see cref="Size"/> of the thumbnail</param>
        /// <param name="boundingBox">Bounding box rectangle</param>
        /// <returns>Rectangle having the <paramref name="thumbSize"/> that is centered within the <paramref name="boundingBox"/></returns>
        private static Rect CenterInRectangle(Size thumbSize, Rect boundingBox)
        {

            var left = boundingBox.Left + (boundingBox.Width - thumbSize.Width) / 2;
            var top = boundingBox.Top + (boundingBox.Height - thumbSize.Height) / 2;

            return new Rect(left, top, thumbSize.Width, thumbSize.Height);
        }

        /// <summary>
        /// Scale given <paramref name="thumbSize"/> into the <paramref name="boundingBox"/> while keeping the aspect ratio
        /// </summary>
        /// <param name="thumbSize"><see cref="Size"/> of the thumbnail</param>
        /// <param name="boundingBox"><see cref="Size"/> of the bounding box</param>
        /// <returns>Scaled <see cref="Size"/> of the thumbnail</returns>
        private static Size ScaleToBound(Size thumbSize, Size boundingBox)
        {
            if (thumbSize.Width == 0 || thumbSize.Height == 0) return new Size(0, 0);
            var widthScale = boundingBox.Width / thumbSize.Width;
            var heightScale = boundingBox.Height / thumbSize.Height;

            var scale = Math.Min(widthScale, heightScale);
            return new Size(thumbSize.Width * scale, thumbSize.Height * scale);
        }

        /// <summary>
        /// Unregister (hide) the existing thumbnail identified by <see cref="MainViewModel.thumbnailHandle"/>
        /// </summary>
        /// <remarks>
        /// When there is no thumbnail (<see cref="MainViewModel.thumbnailHandle"/> is <see cref="IntPtr.Zero"/>),
        /// no exception is thrown and the method "silently" returns
        /// </remarks>
        private void UnregisterThumbnail()
        {
            if (thumbnailHandle != IntPtr.Zero)
            {
                _ = DwmApi.DwmUnregisterThumbnail(thumbnailHandle);
                LogHideThumbnail(thumbnailHandle);
                thumbnailHandle = IntPtr.Zero;
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
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
