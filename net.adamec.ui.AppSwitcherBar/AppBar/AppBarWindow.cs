using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using net.adamec.ui.AppSwitcherBar.Config;
using net.adamec.ui.AppSwitcherBar.Dto;
using net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;
using net.adamec.ui.AppSwitcherBar.Win32.NativeMethods;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
using net.adamec.ui.AppSwitcherBar.Win32.Services;
using net.adamec.ui.AppSwitcherBar.WpfExt;
using static net.adamec.ui.AppSwitcherBar.Win32.NativeConstants.Win32Consts;
// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable IdentifierTypo

namespace net.adamec.ui.AppSwitcherBar.AppBar
{
    //
    /// <summary>
    /// Top level window that can dock as Application Desktop Toolbar (Appbar) 
    /// </summary>
    /// <remarks>See https://docs.microsoft.com/en-us/windows/win32/shell/application-desktop-toolbars for details.
    /// WPF implementation based on https://github.com/mgaffigan/WpfAppBar</remarks>
    public class AppBarWindow : Window
    {
        #region Logging
        // ReSharper disable InconsistentNaming
        /// <summary>
        /// Logger
        /// </summary>
        protected readonly ILogger logger;
        // 3xxx - AppBarWindow (39xx Errors/Exceptions)
        /// <summary>
        /// Log definition options
        /// </summary>
        private static readonly LogDefineOptions LogOptions = new() { SkipEnabledCheck = true };


        //----------------------------------------------
        // 3001 AppBar initialized
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogAppBarInitialized
        /// </summary>
        private static readonly Action<ILogger,  Exception?> __LogAppBarInitializedDefinition =

            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(3001, nameof(LogAppBarInitialized)),
                "AppBar initialized",
                LogOptions);

        /// <summary>
        /// Logs record (Information) when the AppBar is initialized
        /// </summary>
        private void LogAppBarInitialized()
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                __LogAppBarInitializedDefinition(logger, null);
            }
        }

        //----------------------------------------------
        // 3002 AppBar removed
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogAppBarRemoved
        /// </summary>
        private static readonly Action<ILogger, Exception?> __LogAppBarRemovedDefinition =

            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(3003, nameof(LogAppBarRemoved)),
                "AppBar removed",
                LogOptions);

        /// <summary>
        /// Logs record (Information) when the AppBar is removed
        /// </summary>
        private void LogAppBarRemoved()
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                __LogAppBarRemovedDefinition(logger, null);
            }
        }

        //----------------------------------------------
        // 3003 User settings saved
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogUserSettingsSaved
        /// </summary>
        private static readonly Action<ILogger, string,Exception?> __LogUserSettingsSavedDefinition =

            LoggerMessage.Define<string>(
                LogLevel.Debug,
                new EventId(3003, nameof(LogUserSettingsSaved)),
                "User settings saved {userSettings}",
                LogOptions);

        /// <summary>
        /// Logs record (Debug) when the user settings was saved
        /// </summary>
        private void LogUserSettingsSaved()
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                __LogUserSettingsSavedDefinition(logger, Settings.UserSettings.ToString(), null);
            }
        }
        // ReSharper restore InconsistentNaming
        #endregion

        /// <summary>
        /// Original appbar width or height when dragging the thumb to resize
        /// </summary>
        private double ThumbDragOriginalDockedWidthOrHeight { get; set; }
        /// <summary>
        /// Appbar resize delta (width or height) when dragging the thumb to resize
        /// </summary>
        private double ThumbDragDeltaFromOriginal { get; set; }
        /// <summary>
        /// Width of measured (main) element used for auto size feature
        /// </summary>
        private double sizeMeasureElementWidth;
        /// <summary>
        /// Height of measured (main) element used for auto size feature
        /// </summary>
        private double sizeMeasureElementHeight;

        /// <summary>
        /// Window handle source
        /// </summary>
        protected HwndSource? HwndSource { get; private set; }

        /// <summary>
        /// Window handle (Hwnd)
        /// </summary>
        protected IntPtr Hwnd { get; private set; }

        /// <summary>
        /// Flag whether the appbar is registered in Windows
        /// </summary>
        private bool IsAppBarRegistered { get; set; }
        /// <summary>
        /// Flag whether the appbar is resizing (to prevent the infinite event loop)
        /// </summary>
        private bool IsInAppBarResize { get; set; }

        /// <summary>
        /// Custom windows message to be received as a callback/notification from appbar Windows system 
        /// </summary>
        private int appBarMessageId;
        /// <summary>
        /// Custom windows message to be received as a callback/notification from appbar Windows system 
        /// </summary>
        protected int AppBarMessageId
        {
            get
            {
                if (appBarMessageId == 0) appBarMessageId = WndAndApp.RegisterWindowMessage($"AppBarMessage_{Guid.NewGuid()}");
                return appBarMessageId;
            }
        }

        /// <summary>
        /// Flag whether the window is used by XAML Designer 
        /// </summary>
        protected bool IsDesignTime => DesignerProperties.GetIsInDesignMode(this);
        /// <summary>
        /// Flag whether the window (appbar) is docked vertically
        /// </summary>
        protected bool IsVertical => DockMode is AppBarDockMode.Left or AppBarDockMode.Right;
        /// <summary>
        /// Flag whether the window (appbar) is docked horizontally 
        /// </summary>
        protected bool IsHorizontal => DockMode is AppBarDockMode.Top or AppBarDockMode.Bottom;

        /// <summary>
        /// Appbar dock mode
        /// </summary>
        public AppBarDockMode DockMode
        {
            get => (AppBarDockMode)GetValue(DockModeProperty);
            set => SetValue(DockModeProperty, value);
        }

        /// <summary>
        /// Appbar dock mode
        /// </summary>
        public static readonly DependencyProperty DockModeProperty = DependencyProperty.Register(
            nameof(DockMode),
            typeof(AppBarDockMode),
            typeof(AppBarWindow),
            new FrameworkPropertyMetadata(AppBarDockMode.Bottom, DockLocationOrSizeChanged));

        /// <summary>
        /// Current monitor (display) information
        /// </summary>
        public MonitorInfo? Monitor
        {
            get => (MonitorInfo)GetValue(MonitorProperty);
            set => SetValue(MonitorProperty, value);
        }

        /// <summary>
        /// Current monitor (display) information
        /// </summary>
        public static readonly DependencyProperty MonitorProperty = DependencyProperty.Register(
            nameof(Monitor),
            typeof(MonitorInfo),
            typeof(AppBarWindow),
            new FrameworkPropertyMetadata(null, DockLocationOrSizeChanged));

        /// <summary>
        /// Flag whether the application bar is auto sized
        /// </summary>
        public bool IsAutoSized
        {
            get => (bool)GetValue(IsAutoSizedProperty);
            set => SetValue(IsAutoSizedProperty, value);
        }
        /// <summary>
        /// Flag whether the application bar is auto sized
        /// </summary>
        public static readonly DependencyProperty IsAutoSizedProperty = DependencyProperty.Register(
            nameof(IsAutoSized),
            typeof(bool),
            typeof(AppBarWindow),
            new FrameworkPropertyMetadata(true, DockLocationOrSizeChanged));

        /// <summary>
        /// Flag that the autosize should be performed after <see cref="IsAutoSized"/> has been set
        /// </summary>
        private bool forceAutosize;

        /// <summary>
        /// App bar width when docked vertically
        /// </summary>
        public int DockedWidth
        {
            get => (int)GetValue(DockedWidthProperty);
            set => SetValue(DockedWidthProperty, value);
        }
        /// <summary>
        /// App bar width when docked vertically
        /// </summary>
        public static readonly DependencyProperty DockedWidthProperty = DependencyProperty.Register(
            nameof(DockedWidth),
            typeof(int),
            typeof(AppBarWindow),
            new FrameworkPropertyMetadata(200, DockLocationOrSizeChanged, CoerceDockedWidthOrHeight));

        /// <summary>
        /// App bar height when docked horizontally
        /// </summary>
        public int DockedHeight
        {
            get => (int)GetValue(DockedHeightProperty);
            set => SetValue(DockedHeightProperty, value);
        }
        /// <summary>
        /// App bar height when docked horizontally
        /// </summary>
        public static readonly DependencyProperty DockedHeightProperty = DependencyProperty.Register(
            nameof(DockedHeight),
            typeof(int),
            typeof(AppBarWindow),
            new FrameworkPropertyMetadata(100, DockLocationOrSizeChanged, CoerceDockedWidthOrHeight));

        /// <summary>
        /// Application settings
        /// </summary>
        public IAppSettings Settings
        {
            get => (IAppSettings)GetValue(SettingsPropertyKey.DependencyProperty);
            private init => SetValue(SettingsPropertyKey, value);
        }
        /// <summary>
        /// Application settings
        /// </summary>
        private static readonly DependencyPropertyKey SettingsPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(Settings),
            typeof(AppSettings),
            typeof(AppBarWindow),
            new FrameworkPropertyMetadata());

        /// <summary>
        /// Attached property to be used by child items to implement the auto size feature
        /// </summary>
        public static readonly DependencyProperty BarAutoSizeProperty = DependencyProperty.RegisterAttached(
            "BarAutoSize",
            typeof(AppBarAutoSizeElement),
            typeof(AppBarWindow),
            new FrameworkPropertyMetadata(defaultValue: AppBarAutoSizeElement.None)
            );

        /// <summary>
        ///  Get accessor method to <see cref="BarAutoSizeProperty"/>
        /// </summary>
        /// <param name="target"><see cref="UIElement"/> to get the <see cref="BarAutoSizeProperty"/> value from</param>
        /// <returns>Value of <see cref="BarAutoSizeProperty"/> for given <paramref name="target"/></returns>
        public static AppBarAutoSizeElement GetBarAutoSize(UIElement target) => (AppBarAutoSizeElement)target.GetValue(BarAutoSizeProperty);

        /// <summary>
        /// Set accessor method to <see cref="BarAutoSizeProperty"/>
        /// </summary>
        /// <param name="target"><see cref="UIElement"/> to set the <see cref="BarAutoSizeProperty"/> value to</param>
        /// <param name="value">Value of <see cref="BarAutoSizeProperty"/> for given <paramref name="target"/></param>
        public static void SetBarAutoSize(UIElement? target, AppBarAutoSizeElement value) => target?.SetValue(BarAutoSizeProperty, value);

        /// <summary>
        /// Command requesting to build the close the application
        /// </summary>
        public ICommand CloseAppBarCommand { get; }
        
        /// <summary>
        /// CTOR - used by XAML designer
        /// </summary>
        /// <remarks>HACK: must be public, so Name property can be used in XAML</remarks>
        // ReSharper disable once UnusedMember.Global
        public AppBarWindow()
        {
            //create dummy logger
            var factory = LoggerFactory.Create(b => b.AddConsole());
            logger = factory.CreateLogger<AppBarWindow>();

            //use design time settings
            Settings = AppSettings.DesignTimeAppSettings;

            CloseAppBarCommand = new RelayCommand(Close);
        }
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="options">Application settings</param>
        /// <param name="logger">Logger to be used</param>
        protected AppBarWindow(IOptions<AppSettings> options, ILogger logger)
        {
            this.logger = logger;
            Settings = options.Value;
            MinHeightProperty.OverrideMetadata(typeof(AppBarWindow), new FrameworkPropertyMetadata((double)Settings.AppBarMinHeight, MinMaxHeightChanged));
            MinWidthProperty.OverrideMetadata(typeof(AppBarWindow), new FrameworkPropertyMetadata((double)Settings.AppBarMinWidth, MinMaxWidthChanged));
            MaxHeightProperty.OverrideMetadata(typeof(AppBarWindow), new FrameworkPropertyMetadata(MinMaxHeightChanged));
            MaxWidthProperty.OverrideMetadata(typeof(AppBarWindow), new FrameworkPropertyMetadata(MinMaxWidthChanged));

            ShowInTaskbar = Settings.ShowInTaskbar;
            DockMode = Settings.AppBarDock;
            IsAutoSized = Settings.AppBarAutoSize;
            DockedWidth = Settings.AppBarDockedWidth;
            DockedHeight = Settings.AppBarDockedHeight;

            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            Topmost = true;

            CloseAppBarCommand = new RelayCommand(Close);
        }
        #region Dependency properties change handlers
        /// <summary>
        /// MinWidth and MaxWidth change handler
        /// </summary>
        /// <param name="d">Dependency object</param>
        /// <param name="e">Event arguments</param>
        private static void MinMaxWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(DockedWidthProperty);//adjust DockedHeight
        }

        /// <summary>
        /// MinHeight and MaxHeight change handler
        /// </summary>
        /// <param name="d">Dependency object</param>
        /// <param name="e">Event arguments</param>
        private static void MinMaxHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(DockedHeightProperty); //adjust DockedHeight
        }

        /// <summary>
        /// Coerces (adjusts) DockedWidth or DockedHeight to Min/Max Width and Height
        /// </summary>
        /// <param name="d">Dependency object</param>
        /// <param name="baseValue">DockedWidth or DockedHeight to adjust with Min/Max Width and Height</param>
        /// <returns>Coreced DockedWidth or DockedHeight value</returns>
        /// <exception cref="NotSupportedException">Thrown when the appbar is not docked</exception>
        private static object CoerceDockedWidthOrHeight(DependencyObject d, object baseValue)
        {
            var appBarWindow = (AppBarWindow)d;
            var newValue = (int)baseValue;

            return appBarWindow.DockMode switch
            {
                AppBarDockMode.Left or AppBarDockMode.Right => newValue.EnsureMinMax(appBarWindow.MinWidth, appBarWindow.MaxWidth),
                AppBarDockMode.Top or AppBarDockMode.Bottom => newValue.EnsureMinMax(appBarWindow.MinHeight, appBarWindow.MaxHeight),
                _ => throw new NotSupportedException(),
            };
        }

        /// <summary>
        /// Handler of the events raised when the appbar <see cref="DockMode"/>, location or size changes
        /// </summary>
        /// <param name="d">Dependency object</param>
        /// <param name="e">Event arguments</param>
        private static void DockLocationOrSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var appBarWindow = (AppBarWindow)d;

            if (e.Property == IsAutoSizedProperty && e.NewValue is true)
            {
                appBarWindow.forceAutosize = true;
            }

            if (appBarWindow.IsAppBarRegistered)
            {
                appBarWindow.AppBarUpdate();
            }
        }
        #endregion

        #region Window events
        /// <summary>
        /// Raises <see cref="Window.SourceInitialized"/> event when the underlying Win32 window handle becomes available
        /// </summary>
        /// <param name="e">Event arguments</param>
        /// <exception cref="InvalidOperationException">Thrown when HWND Source is null</exception>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            //Get HWND source and HWND of window
            HwndSource = (HwndSource?)PresentationSource.FromVisual(this) ?? throw new InvalidOperationException("Can't get HWND Source");

            Hwnd = HwndSource.Handle;

            //HWND is available, initialize appbar
            if (!IsDesignTime) AppBarInitialize();
        }

        /// <summary>
        /// Handler of DPI change event
        /// </summary>
        /// <param name="oldDpi">Old DPI</param>
        /// <param name="newDpi">New DPI</param>
        protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
        {
            base.OnDpiChanged(oldDpi, newDpi);
#if DEBUG
            Debug.WriteLine($"DPI Change: {oldDpi.DpiScaleX},{oldDpi.DpiScaleY} to {newDpi.DpiScaleX},{newDpi.DpiScaleY}");
#endif
            AppBarUpdate();
        }

        /// <inheritdoc/>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (e.Cancel)
            {
                return;
            }

            //remove appbar
            AppBarRemove();
        }

        /// <summary>
        /// Close application handler that can be called from XAML (for example Click of close button)
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event arguments</param>
        protected void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        #region Appbar management
        /// <summary>
        /// Initialize appbar
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="HwndSource"/> is null</exception>
        private void AppBarInitialize()
        {
            if (IsDesignTime) return;
            if (HwndSource is null) throw new InvalidOperationException("HWND Source is not available");

            if (!ShowInTaskbar)
            {
                WndAndApp.HideFromTaskbar(Hwnd);
            }

            //Register WNDPROC hook
            HwndSource.AddHook(WndProc);

            //Create appbar
            SendAppBarMessage(ABMsg.NEW);
            IsAppBarRegistered = true;

            //Set the initial location and size
            AppBarUpdate();

            LogAppBarInitialized();
        }

        /// <summary>
        /// Updates appbar location and/or size when needed
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when the appbar is not docked</exception>
        private void AppBarUpdate()
        {
            if (IsInAppBarResize)
            {
                return; //prevent the infinite event loop
            }

            Settings.UserSettings.AppBarDock = DockMode;
            Settings.UserSettings.AppBarDockedWidth = DockedWidth;
            Settings.UserSettings.AppBarDockedHeight = DockedHeight;
            Settings.UserSettings.AppBarAutoSize = IsAutoSized;
            if(Settings.UserSettings.Save()) LogUserSettingsSaved();

            //To set the size and position of an appbar, an application first proposes a screen edge and bounding rectangle for the appbar by sending the ABM_QUERYPOS message.
            //The system determines whether any part of the screen area within the proposed rectangle is occupied by the taskbar or another appbar,
            //adjusts the rectangle (if necessary), and returns the adjusted rectangle to the application.
            SendAppBarMessage(ABMsg.QUERYPOS, (ref APPBARDATA d) => d.rc = (RECT)GetSelectedMonitor().ViewportBounds, out var abd);

            //Apply the width/size (from edge)
            var dockedWidthOrHeightInDesktopPixels = this.WpfDimensionToScreen(IsVertical ? DockedWidth : DockedHeight);
            switch (DockMode)
            {
                case AppBarDockMode.Top:
                    abd.rc.bottom = abd.rc.top + dockedWidthOrHeightInDesktopPixels;
                    break;
                case AppBarDockMode.Bottom:
                    abd.rc.top = abd.rc.bottom - dockedWidthOrHeightInDesktopPixels;
                    break;
                case AppBarDockMode.Left:
                    abd.rc.right = abd.rc.left + dockedWidthOrHeightInDesktopPixels;
                    break;
                case AppBarDockMode.Right:
                    abd.rc.left = abd.rc.right - dockedWidthOrHeightInDesktopPixels;
                    break;
                default: throw new NotSupportedException();
            }

            //Next, the application sends the ABM_SETPOS message to set the new bounding rectangle for the appbar.
            SendAppBarMessage(ABMsg.SETPOS, ref abd);

            //Again, the system may adjust the rectangle before returning it to the application.
            //For this reason, the application should use the adjusted rectangle returned by ABM_SETPOS to set the final size and position.

            IsInAppBarResize = true;
            try
            {
                //uppdate the appbar with final size and position
                var rect = (Rect)abd.rc;

                Left = this.ScreenDimensionToWpf(rect.Left);
                Top = this.ScreenDimensionToWpf(rect.Top);
                Width = this.ScreenDimensionToWpf(rect.Width);
                Height = this.ScreenDimensionToWpf(rect.Height);
            }
            finally
            {
                IsInAppBarResize = false;
            }
        }

        /// <summary>
        /// Remove the appbar
        /// </summary>
        private void AppBarRemove()
        {
            if (!IsAppBarRegistered) return;
            
            SendAppBarMessage(ABMsg.REMOVE);
            IsAppBarRegistered = false;
            LogAppBarRemoved();
        }

        /// <summary>
        /// WNDPROC handlers
        /// </summary>
        /// <param name="hwnd">HWND of window</param>
        /// <param name="msg">Message to handle</param>
        /// <param name="wParam">Message parameter</param>
        /// <param name="lParam">Message parameter</param>
        /// <param name="handled">Flag whether the message has been processed (handled)</param>
        /// <returns></returns>
        public IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {

            //Block window position or size change that is not part of "appbar flow"
            if (msg == WM_WINDOWPOSCHANGING && !IsInAppBarResize)
            {
                var wp = Marshal.PtrToStructure<WINDOWPOS>(lParam);
                wp.flags |= SWP_NOMOVE | SWP_NOSIZE; //force the NOMOVE and NOSIZE flags
                Marshal.StructureToPtr(wp, lParam, false);
            }

            //Whenever an appbar receives the WM_ACTIVATE message, it should send the ABM_ACTIVATE message.
            //Similarly, when an appbar receives a WM_WINDOWPOSCHANGED message, it must call ABM_WINDOWPOSCHANGED.
            //Sending these messages ensures that the system properly sets the z-order of any autohide appbars on the same edge.
            else if (msg == WM_ACTIVATE)
            {
                SendAppBarMessage(ABMsg.ACTIVATE);
            }
            else if (msg == WM_WINDOWPOSCHANGED)
            {
                SendAppBarMessage(ABMsg.WINDOWPOSCHANGED);
            }
            else if (msg == AppBarMessageId)
            {
                //Notfications from appbar system
                // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                switch ((ABNotify)(int)wParam)
                {
                    //An application should set the size and position of its appbar after registering it and whenever the appbar receives the ABN_POSCHANGED notification message.
                    //An appbar receives this notification message whenever a change occurs in the taskbar's size, position,
                    //or visibility state and whenever another appbar on the same side of the screen is resized, added, or removed.
                    case ABNotify.POSCHANGED:
                        AppBarUpdate();
                        handled = true;
                        break;

                    // A full-screen application has started, or the last full-screen
                    // application has closed. Set the appbar's z-order appropriately.
                    case ABNotify.FULLSCREENAPP:

                        if ((int)lParam > 0)
                        {
                            WndAndApp.SetWindowToBottom(hwnd);
                        }
                        else
                        {
                            WndAndApp.SetWindowToTop(hwnd);
                        }
                        break;
                }
            }

            return IntPtr.Zero;
        }


        /// <summary>
        /// Sends an appbar <paramref name="message"/> to the system.
        /// </summary>
        /// <param name="message">Appbar message value to send. This parameter can be one of the values from <see cref="ABMsg"/>.</param>
        /// <returns>This function returns a message-dependent value</returns>
        private void SendAppBarMessage(ABMsg message)
        {
            SendAppBarMessage(message, null, out _);
        }

        /// <summary>
        /// Delegate to an action enriching the message data before sending
        /// </summary>
        /// <param name="data"><see cref="APPBARDATA"/> structure to be enriched</param>
        private delegate void EnrichAppDataDelegate(ref APPBARDATA data);

        /// <summary>
        /// Sends an appbar <paramref name="message"/> to the system.
        /// </summary>
        /// <param name="message">Appbar message value to send. This parameter can be one of the values from <see cref="ABMsg"/>.</param>
        /// <param name="enrichDataAction">Optional action enriching the message data before sending</param>
        /// <param name="data"><see cref="APPBARDATA"/> structure. The content of the structure on entry and on exit depends on the value set in the <paramref name="message"/> parameter</param>
        private void SendAppBarMessage(ABMsg message, EnrichAppDataDelegate? enrichDataAction, out APPBARDATA data)
        {
            data = new APPBARDATA()
            {
                cbSize = Marshal.SizeOf(typeof(APPBARDATA)),
                hWnd = Hwnd,
                uCallbackMessage = AppBarMessageId,
                uEdge = (ABEdge)(int)DockMode
            };
            enrichDataAction?.Invoke(ref data);

            SendAppBarMessage(message, ref data);
        }

        /// <summary>
        /// Sends an appbar <paramref name="message"/> to the system.
        /// </summary>
        /// <param name="message">Appbar message value to send. This parameter can be one of the values from <see cref="ABMsg"/>.</param>
        /// <param name="data"><see cref="APPBARDATA"/> structure. The content of the structure on entry and on exit depends on the value set in the <paramref name="message"/> parameter</param>
        private static void SendAppBarMessage(ABMsg message, ref APPBARDATA data)
        {
            _ = Shell32.SHAppBarMessage(message, ref data);
        }

        /// <summary>
        /// Gets the <see cref="MonitorInfo"/> about the <see cref="Monitor"/> selected in UI
        /// </summary>
        /// <returns><see cref="MonitorInfo"/> about the <see cref="Monitor"/> selected in UI</returns>
        private MonitorInfo GetSelectedMonitor()
        {
            var monitor = Monitor;
            var allMonitors = Win32.Services.Monitor.GetAllMonitors();
            if (monitor is null || !allMonitors.Contains(monitor))
            {
                //if none is selected or is unknown, return the primary monitor
                monitor = allMonitors.First(f => f.IsPrimary);
            }

            return monitor;
        }
        #endregion

        #region Resize thumb drag and drop
        /// <summary>
        /// Thumb drag started event handled
        /// </summary>
        /// <param name="sender">Thumb</param>
        /// <param name="e">Event arguments</param>
        protected void ResizeDragStarted(object sender, DragStartedEventArgs e)
        {
            //Preserver the original Width/Height and reset delta
            ThumbDragOriginalDockedWidthOrHeight = IsVertical ? DockedWidth : DockedHeight;
            ThumbDragDeltaFromOriginal = 0;
        }

        /// <summary>
        /// Thumb drag delta event handled
        /// </summary>
        /// <param name="sender">Thumb</param>
        /// <param name="e">Event arguments</param>
        protected void ResizeDragDelta(object sender, DragDeltaEventArgs e)
        {
            //update delta (the event args contain change from the last event!)
            ThumbDragDeltaFromOriginal += GetThumbDragDelta(e.HorizontalChange, e.VerticalChange);
            //update "visual" when enabled in settings
            if (Settings.AppBarResizeRedrawOnDrag) ThumbDragResize();
        }

        /// <summary>
        /// Thumb drag completed event handled
        /// </summary>
        /// <param name="sender">Thumb</param>
        /// <param name="e">Event arguments</param>
        protected void ResizeDragCompleted(object sender, DragCompletedEventArgs e)
        {
            //update delta (the event args contain change from the start event!)
            ThumbDragDeltaFromOriginal = GetThumbDragDelta(e.HorizontalChange, e.VerticalChange);
            //update visual
            ThumbDragResize();
        }

        /// <summary>
        /// Transform the drag event delta to "size delta" taking into the consideration
        /// the dock mode and the DPI
        /// </summary>
        /// <param name="horizontalChange">Horizontal change as provided in event</param>
        /// <param name="verticalChange">Vertical change as provided in event</param>
        /// <returns>Required apbar size change</returns>
        /// <exception cref="NotSupportedException">Thrown when the appbar is not docked</exception>
        private double GetThumbDragDelta(double horizontalChange, double verticalChange)
        {
            var delta = DockMode switch
            {
                AppBarDockMode.Left => horizontalChange,
                AppBarDockMode.Right => horizontalChange * -1,
                AppBarDockMode.Top => verticalChange,
                AppBarDockMode.Bottom => verticalChange * -1,
                _ => throw new NotSupportedException(),
            };
            var deltaCorr = (int)(delta / VisualTreeHelper.GetDpi(this).PixelsPerDip);
            return deltaCorr;
        }

        /// <summary>
        /// Resize the appbar based on the Thumb drag (and drop)
        /// </summary>
        private void ThumbDragResize()
        {
            if (IsVertical)
            {
                DockedWidth = (int)(ThumbDragOriginalDockedWidthOrHeight + ThumbDragDeltaFromOriginal);
            }
            else
            {
                DockedHeight = (int)(ThumbDragOriginalDockedWidthOrHeight + ThumbDragDeltaFromOriginal);
            }
        }
        #endregion

        #region AutoSize

        /// <summary>
        /// Configure the auto size functionality (identify the related UI element and do initial auto size if enabled)
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            var sizeAddElements = new List<FrameworkElement>();
            GetAutoSizeUiElements(this, sizeAddElements, out var sizeMeasureElement);
            if (sizeMeasureElement == null) return;

            DoAutoSize(sizeMeasureElement, sizeAddElements);
            sizeMeasureElement.SizeChanged += (_, _) => DoAutoSize(sizeMeasureElement, sizeAddElements);
        }


        /// <summary>
        /// Auto size the <see cref="AppBarWindow"/> based on the content (size) of <paramref name="sizeMeasureElement"/>.
        /// Takes into the consideration the margins of parent elements ("containers") and adds the size of elements defined in <paramref name="sizeAddElements"/> collection
        /// </summary>
        /// <param name="sizeMeasureElement">Element used as a base for auto size</param>
        /// <param name="sizeAddElements">Elements to "add" to calculated size</param>
        private void DoAutoSize(FrameworkElement sizeMeasureElement, IEnumerable<FrameworkElement> sizeAddElements)
        {
            if (!IsAutoSized) return; //auto size is not enabled

            //check whether the size of measured element "significantly" changed
            const int sizeBuffer = 2;
            const double sensitivity = (double)sizeBuffer / 2;

            var width = sizeMeasureElement.RenderSize.Width;
            var height = sizeMeasureElement.RenderSize.Height;
            var isDirty = false;
            if (Math.Abs(width - sizeMeasureElementWidth) >= sensitivity)
            {
                isDirty = true;
                sizeMeasureElementWidth = width;
            }

            if (Math.Abs(height - sizeMeasureElementHeight) >= sensitivity)
            {
                isDirty = true;
                sizeMeasureElementHeight = height;
            }

            if (!isDirty && !forceAutosize) return; //no size change
            
            //calculate the target size
            // - size of measured element + margins of "containers" up to the visual tree + sum of sizes of "side/sibling" elements marked with "Add" property
            var targetSize = (int)(IsVertical ?
                sizeMeasureElementWidth + GetMargins(sizeMeasureElement, element => element.Margin.Left + element.Margin.Right) + sizeAddElements.Sum(element => element.ActualWidth) :
                sizeMeasureElementHeight + GetMargins(sizeMeasureElement, element => element.Margin.Top + element.Margin.Bottom) + sizeAddElements.Sum(element => element.ActualHeight));

            //check whether the target size change is bigger than "size buffer" used to absorb small changes
            //apply a new size when needed
            if (IsVertical)
            {
                if (Math.Abs(DockedWidth - targetSize) >= sizeBuffer || forceAutosize)
                {
                    forceAutosize = false;
                    DockedWidth = targetSize + sizeBuffer;
                }
            }
            else
            {
                if (Math.Abs(DockedHeight - targetSize) >= sizeBuffer || forceAutosize)
                {
                    forceAutosize = false;
                    DockedHeight = targetSize + sizeBuffer;
                }
            }
        }

        /// <summary>
        /// Iterates from <paramref name="element"/> up to the window and gets the sum of margins (measured using <paramref name="measure"/> function)
        /// </summary>
        /// <param name="element">Element to check</param>
        /// <param name="measure">Function used to measure margins.</param>
        /// <returns>Sum of margins from <paramref name="element"/> to window</returns>
        private double GetMargins(Visual element, Func<FrameworkElement, double> measure)
        {
            var margins = 0.0d;

            if (element is FrameworkElement frameworkElement) margins += measure(frameworkElement);
            var parent = VisualTreeHelper.GetParent(element) as Visual;
            if (parent != this && parent != null)
            {
                margins += GetMargins(parent, measure);
            }
            return margins;
        }

        /// <summary>
        /// Iterates through the visual tree to find a child with <see cref="BarAutoSizeProperty"/> set to <see cref="AppBarAutoSizeElement.Measure"/> and returns it in <paramref name="sizeMeasureElement"/>.
        /// Also checks for children having <see cref="BarAutoSizeProperty"/> set to <see cref="AppBarAutoSizeElement.Add"/> and returns them in <paramref name="sizeAddElements"/>
        /// </summary>
        /// <param name="parent">Element to process</param>
        /// <param name="sizeAddElements">Collection of the elements having <see cref="BarAutoSizeProperty"/> set to <see cref="AppBarAutoSizeElement.Add"/></param>
        /// <param name="sizeMeasureElement">Element with <see cref="BarAutoSizeProperty"/> set to <see cref="AppBarAutoSizeElement.Measure"/></param>
        private static void GetAutoSizeUiElements(DependencyObject parent, ICollection<FrameworkElement> sizeAddElements, out FrameworkElement? sizeMeasureElement)
        {
            sizeMeasureElement = null;
            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (var i = 0; i <= childrenCount - 1; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                var sizeElement = AppBarAutoSizeElement.None;
                if (child is FrameworkElement uiElement)
                {
                    sizeElement = GetBarAutoSize(uiElement);
                    // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                    switch (sizeElement)
                    {
                        case AppBarAutoSizeElement.Measure:
                            sizeMeasureElement = uiElement;
                            break;
                        case AppBarAutoSizeElement.Add:
                            sizeAddElements.Add(uiElement);
                            break;
                    }
                }

                if (VisualTreeHelper.GetChildrenCount(child) <= 0 || sizeElement == AppBarAutoSizeElement.Measure) continue;

                //traverse to children
                GetAutoSizeUiElements(child, sizeAddElements, out var tmpMeasureElement);
                if (tmpMeasureElement != null) sizeMeasureElement = tmpMeasureElement;
            }
        }
        #endregion

    }
}
