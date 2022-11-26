using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using net.adamec.ui.AppSwitcherBar.Dto;
using net.adamec.ui.AppSwitcherBar.Win32.NativeClasses;
using net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;
using net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces;
using net.adamec.ui.AppSwitcherBar.Win32.NativeMethods;
using static net.adamec.ui.AppSwitcherBar.Win32.NativeConstants.Win32Consts;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.Services
{
    /// <summary>
    /// Encapsulates the native window and application functionality
    /// </summary>
    internal static class WndAndApp
    {
        /// <summary>
        /// Defines a new window message that is guaranteed to be unique throughout the system. The message value can be used when sending or posting messages.
        /// </summary>
        /// <param name="msg">The message to be registered. When not provided, random GUID string is used</param>
        /// <returns>If the message is successfully registered, the return value is a message identifier in the range 0xC000 through 0xFFFF.
        /// If the function fails, the return value is zero.</returns>
        public static int RegisterWindowMessage(string? msg)
        {
            if (string.IsNullOrEmpty(msg)) msg = $"{Guid.NewGuid()}";
            return User32.RegisterWindowMessage(msg);
        }

        /// <summary>
        /// Hides the given window from taskbar
        /// </summary>
        /// <param name="hwnd">HWND of the window</param>
        public static void HideFromTaskbar(IntPtr hwnd)
        {
            //To prevent the window button from being placed on the taskbar, create the unowned window with the WS_EX_TOOLWINDOW extended style
            //https://docs.microsoft.com/en-us/windows/win32/shell/taskbar
            var exstyle = (ulong)User32.GetWindowLongPtr(hwnd, GWL_EXSTYLE) | WS_EX_TOOLWINDOW;
            User32.SetWindowLongPtr(hwnd, GWL_EXSTYLE, (IntPtr)exstyle);
        }


        /// <summary>
        /// Information about window Process ID
        /// </summary>
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private class PidInfos
        {
            /// <summary>
            /// Main window HWND
            /// </summary>
            public int parentHwnd;
            /// <summary>
            /// Main window PID
            /// </summary>
            public uint parentPid;
            /// <summary>
            /// Child window PID
            /// </summary>
            public uint childPid;
        }

        /// <summary>
        /// Delegate for action to be called for each window when enumerating the visible windows using <see cref="WndAndApp.EnumVisibleWindows"/>
        /// </summary>
        /// <param name="hwnd">HWND of window</param>
        /// <param name="wndInfo"><see cref="WndInfo"/> of window when already available otherwise null</param>
        /// <param name="title">Current caption of the window</param>
        /// <param name="threadId">Window's thread ID</param>
        /// <param name="processId">Window's process ID</param>
        /// <param name="ptrProcess">Handle to the window's process</param>
        public delegate void EnumVisibleWindowsAction(IntPtr hwnd, WndInfo? wndInfo, string title, uint threadId, uint processId, IntPtr ptrProcess);

        /// <summary>
        /// Enumerate application windows that should be presented as windows buttons
        /// </summary>
        /// <param name="applicationHwnd">Application window (HWND) to be excluded from enumeration</param>
        /// <param name="wndInfoAccessor">Function returning the <see cref="WndInfo"/> from HWND for already "known" windows (should return null for unknown windows)</param>
        /// <param name="action">Action to be called for each window when enumerating windows</param>
        public static void EnumVisibleWindows(IntPtr applicationHwnd, Func<IntPtr, WndInfo?> wndInfoAccessor, EnumVisibleWindowsAction action)
        {

            // ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (action == null || wndInfoAccessor == null) return;
            // ReSharper restore ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

            _ = User32.EnumWindows((hwnd, _) =>
                 {
                     //Filter windows - only the top-level application windows except "itself" 
                     int isCloakedAttribute = -1;
                     var isCloaked = (DwmApi.DwmGetWindowAttribute(hwnd, DWMWA_CLOAKED, ref isCloakedAttribute, sizeof(int)) == S_OK) && isCloakedAttribute > 0;

                     var wndStyle = (ulong)User32.GetWindowLongPtr(hwnd, GWL_STYLE).ToInt64();
                     var wndStyleEx = (ulong)User32.GetWindowLongPtr(hwnd, GWL_EXSTYLE).ToInt64();

                     var isVisible = applicationHwnd != hwnd &&
                                     User32.IsWindowVisible(hwnd) &&
                                     (wndStyleEx & WS_EX_TOOLWINDOW) == 0 &&
                                     (wndStyle & WS_CHILD) == 0 &&
                                     (wndStyle & WS_CAPTION) == WS_CAPTION &&
                                     !isCloaked &&
                                     (
                                         (wndStyleEx & WS_EX_APPWINDOW) == WS_EX_APPWINDOW || User32.GetWindow(hwnd, GW_OWNER) == IntPtr.Zero
                                     );


                     if (!isVisible) return true; //true -> continue enum

                     //Get window title
                     var sb = new StringBuilder(255);
                     User32.GetWindowText(hwnd, sb, sb.Capacity);
                     var text = sb.ToString();

                     if (string.IsNullOrWhiteSpace(text)) return true; //true -> continue enum

                     //Check whether it's a "new" application window or a one already known and get the threadId and processId

                     uint threadId;
                     uint processId;
                     var wnd = wndInfoAccessor(hwnd);
                     if (wnd == null)
                     {
                         //wnd thread ID and PID
                         //for some applications, the main process is just "container", so check also the child windows and when there
                         //is any with different PID, use that one as the application window PID
                         threadId = User32.GetWindowThreadProcessId(hwnd, out var process); //get the window thread handle and process ID
                         var pidInfos = new PidInfos { parentHwnd = hwnd.ToInt32(), parentPid = process, childPid = process };
                         var pidInfosHandle = GCHandle.Alloc(pidInfos);
                         try
                         {
                             User32.EnumChildWindows(hwnd, (childHwnd, param) =>
                             {
                                 var paramHandle = GCHandle.FromIntPtr(param);
                                 if (paramHandle.Target is not PidInfos pids) return false;

                                 var parentHwnd = User32.GetParent(childHwnd);
                                 if (parentHwnd.ToInt32() != pids.parentHwnd)
                                     return true; //not direct child, don't check for PID

                                 User32.GetWindowThreadProcessId(childHwnd, out var childProcess);

                                 if (childProcess != pids.parentPid)
                                     pids.childPid = childProcess;

                                 return true;
                             }, GCHandle.ToIntPtr(pidInfosHandle));
                         }
                         finally
                         {
                             pidInfosHandle.Free();
                         }

                         processId = pidInfos.childPid;
                     }
                     else
                     {
                         //existing (known) window
                         threadId = wnd.ThreadId;
                         processId = wnd.ProcessId;

                     }

                     var ptrProcess = Kernel32.OpenProcess(Shell.Shell.QueryLimitedInformation, false, processId);

                     action(hwnd, wnd, text, threadId, processId, ptrProcess);

                     Kernel32.CloseHandle(ptrProcess);

                     return true;//end of window enum
                 }, 0);
        }

        /// <summary>
        /// Retrieves the full name of the executable image for the specified process.
        /// </summary>
        /// <param name="processHandle">A handle to the process. This handle must be created with the PROCESS_QUERY_INFORMATION or PROCESS_QUERY_LIMITED_INFORMATION access right.</param>
        /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.</returns>
        public static string? GetProcessExecutable(IntPtr processHandle)
        {
            var capacity = 1024;
            var fullPathBuilder = new StringBuilder(capacity);
            var res = Kernel32.QueryFullProcessImageName(processHandle, 0, fullPathBuilder, ref capacity);
            return res ? fullPathBuilder.ToString() : null;
        }

        /// <summary>
        /// Retrieves a handle to the foreground window (the window with which the user is currently working).
        /// </summary>
        /// <returns>The return value is a handle to the foreground window. The foreground window can be NULL (IntPtr.Zero) in certain circumstances, such as when a window is losing activation.</returns>
        public static IntPtr GetForegroundWindow() => User32.GetForegroundWindow();

        /// <summary>
        /// Gets the application user model ID for the specified process.
        /// </summary>
        /// <param name="processHandle">A handle to the process. This handle must have the PROCESS_QUERY_LIMITED_INFORMATION access right.</param>
        /// <returns>Application user mode ID when available for given <paramref name="processHandle"/> or null</returns>
        public static string? GetProcessApplicationUserModelId(IntPtr processHandle)
        {
            //try to get AppUserModelId from process if not "at window"
            uint appIdLen = 130;
            var sb = new StringBuilder((int)appIdLen);
            var result = Kernel32.GetApplicationUserModelId(processHandle, ref appIdLen, sb);
            switch (result)
            {
                case 0:
                    return sb.ToString();
                case 0x7a: //ERROR_INSUFFICIENT_BUFFER
                    {
                        sb = new StringBuilder((int)appIdLen);
                        var rc = Kernel32.GetApplicationUserModelId(processHandle, ref appIdLen, sb);
                        if (rc == 0)
                        {
                            return sb.ToString();
                        }

                        break;
                    }
            }

            return null;
        }

        /// <summary>
        /// Gets the application user model ID for the specified window.
        /// Uses the undocumented win32 api, so it might change or not work as expected
        /// </summary>
        /// <param name="hwnd">A handle to the window (HWND).</param>
        /// <returns>Application user mode ID when available for given <paramref name="hwnd"/> or null</returns>
        public static string? GetWindowApplicationUserModelId(IntPtr hwnd)
        {
            IApplicationResolver? appResolver = null;
            try
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                appResolver = (IApplicationResolver)new CApplicationResolver();
                var hr = appResolver.GetAppIDForWindow(hwnd, out var appId, out _, out _, out _);
                return hr.IsSuccess && !string.IsNullOrEmpty(appId)? appId : null;
            }
            catch (Exception)
            {
                return null; //just return null
            }
            finally
            {
                if (appResolver != null && Marshal.IsComObject(appResolver))
                {
                    Marshal.ReleaseComObject(appResolver); 
                }
            }
        }

        /// <summary>
        /// Gets the application user model ID for the shell item representing a shortcut (link)
        /// Uses the undocumented win32 api, so it might change or not work as expected
        /// </summary>
        /// <param name="shellItem">A shell item representing a shortcut (link).</param>
        /// <returns>Application user mode ID when available for given <paramref name="shellItem"/> or null</returns>
        public static string? GetWindowApplicationUserModelId(IShellItem2 shellItem)
        {
            IApplicationResolver? appResolver = null;
            try
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                appResolver = (IApplicationResolver)new CApplicationResolver();
                var hr = appResolver.GetAppIDForShortcut(shellItem, out var appId);
                return hr.IsSuccess && !string.IsNullOrEmpty(appId) ? appId : null;
            }
            catch (Exception)
            {
                return null; //just return null
            }
            finally
            {
                if (appResolver != null && Marshal.IsComObject(appResolver))
                {
                    Marshal.ReleaseComObject(appResolver);
                }
            }
        }

        /// <summary>
        /// Get the icon of window with given <paramref name="hwnd"/>
        /// </summary>
        /// <param name="hwnd">HWND of the window</param>
        /// <returns>Icon as <see cref="BitmapSource"/> if available, otherwise null</returns>
        public static BitmapSource? GetWindowIcon(IntPtr hwnd)
        {
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
            if (hiconPtr == IntPtr.Zero) return null;

            try
            {
                //Transform the icon into the bitmap presentable in WPF UI
                var bitmapSource = Imaging.CreateBitmapSourceFromHIcon(hiconPtr, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                return bitmapSource;
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// Minimize the window
        /// </summary>
        /// <param name="hwnd">HWND of the window to minimize</param>
        public static void MinimizeWindow(IntPtr hwnd)
        {
            User32.ShowWindow(hwnd, SW_MINIMIZE);
        }

        /// <summary>
        /// Activates the window - shows and activates the window if needed and sets it as foreground window
        /// </summary>
        /// <param name="hwnd">HWND of the window to activate</param>
        /// <returns>True when the window was minimized before, otherwise false</returns>
        public static bool ActivateWindow(IntPtr hwnd)
        {
            //get the window state
            var isMinimized = (User32.GetWindowLongA(hwnd, GWL_STYLE) & WS_MINIMIZE) > 0;
            var isVisible = User32.IsWindowVisible(hwnd);

            //make sure the window is shown and activated
            User32.SetWindowPos(
                hwnd,
                HWND_TOP,
                0, 0, 0, 0,
                SetWindowPosFlags.IgnoreMove |
                SetWindowPosFlags.IgnoreResize |
                SetWindowPosFlags.ShowWindow |
                (isVisible ? SetWindowPosFlags.DoNotActivate : 0));

            //restore the minimized window
            if (isMinimized) User32.ShowWindow(hwnd, SW_RESTORE);

            //set the foreground window
            User32.SetForegroundWindow(hwnd);

            return isMinimized;
        }

        /// <summary>
        /// Closes the window by sending WM_CLOSE message 
        /// </summary>
        /// <param name="hwnd">HWND of window to close</param>
        public static void CloseWindow(IntPtr hwnd)
        {
            User32.PostMessage(hwnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }

        /// <summary>
        /// Sets the window to top of the z-order
        /// </summary>
        /// <param name="hwnd">HWND of window to set</param>
        public static void SetWindowToTop(IntPtr hwnd)
        {
            User32.SetWindowPos(hwnd,
                HWND_TOPMOST,
                0, 0, 0, 0,
                SetWindowPosFlags.IgnoreMove | SetWindowPosFlags.IgnoreResize | SetWindowPosFlags.DoNotActivate);
        }

        /// <summary>
        /// Sets the window to bottom of the z-order
        /// </summary>
        /// <param name="hwnd">HWND of window to set</param>
        public static void SetWindowToBottom(IntPtr hwnd)
        {
            User32.SetWindowPos(hwnd,
                HWND_BOTTOM,
                0, 0, 0, 0,
                SetWindowPosFlags.IgnoreMove | SetWindowPosFlags.IgnoreResize | SetWindowPosFlags.DoNotActivate);
        }
    }
}
