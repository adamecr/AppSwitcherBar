using System;
using System.Runtime.InteropServices;
using System.Text;

namespace net.adamec.ui.AppSwitcherBar.Win32
{
    /// <summary>
    /// User32 Win32 Api
    /// </summary>
    internal class User32
    {
        /// <summary>
        /// Name of the DLL
        /// </summary>
        private const string DLL_NAME = "user32";

        /// <summary>
        /// Enumerates all top-level windows on the screen by passing the handle to each window, in turn, to an application-defined callback function. EnumWindows continues until the last top-level window is enumerated or the callback function returns FALSE
        /// </summary>
        /// <param name="lpEnumFunc">An application-defined callback function</param>
        /// <param name="lParam">An application-defined value to be passed to the callback function</param>
        /// <returns></returns>
        [DllImport(DLL_NAME)]
        public static extern int EnumWindows(EnumWindowsCallback lpEnumFunc, int lParam);

        /// <summary>
        /// Copies the text of the specified window's title bar (if it has one) into a buffer.
        /// </summary>
        /// <param name="hWnd">A handle to the window or control containing the text.</param>
        /// <param name="lpString">The buffer that will receive the text. If the string is as long or longer than the buffer, the string is truncated and terminated with a null character.</param>
        /// <param name="nMaxCount">The maximum number of characters to copy to the buffer, including the null character. If the text exceeds this limit, it is truncated.</param>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode)]
        public static extern void GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        /// <summary>
        /// Brings the thread that created the specified window into the foreground and activates the window. Keyboard input is directed to the window, and various visual cues are changed for the user. The system assigns a slightly higher priority to the thread that created the foreground window than it does to other threads.
        /// </summary>
        /// <param name="hWnd">A handle to the window that should be activated and brought to the foreground.</param>
        /// <returns>If the window was brought to the foreground, the return value is true. If the window was not brought to the foreground, the return value is false.</returns>
        [DllImport(DLL_NAME)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// Retrieves a handle to the foreground window (the window with which the user is currently working). The system assigns a slightly higher priority to the thread that creates the foreground window than it does to other threads.
        /// </summary>
        /// <returns>The return value is a handle to the foreground window. The foreground window can be NULL in certain circumstances, such as when a window is losing activation.</returns>
        [DllImport(DLL_NAME)]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// Retrieves the identifier of the thread that created the specified window and, optionally, the identifier of the process that created the window.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <param name="lpdwProcessId">A pointer to a variable that receives the process identifier</param>
        /// <returns>The return value is the identifier of the thread that created the window.</returns>
        [DllImport(DLL_NAME, SetLastError = true)]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out IntPtr lpdwProcessId);

        /// <summary>
        /// Changes the size, position, and Z order of a child, pop-up, or top-level window. These windows are ordered according to their appearance on the screen. The topmost window receives the highest rank and is the first window in the Z order.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <param name="hWndInsertAfter">A handle to the window to precede the positioned window in the Z order. This parameter must be a window handle or one of the following values:
        /// <see cref="Win32Consts.HWND_BOTTOM"/>, <see cref="Win32Consts.HWND_NOTOPMOST"/>, <see cref="Win32Consts.HWND_TOP"/>, <see cref="Win32Consts.HWND_TOPMOST"/>.</param>
        /// <param name="x">The new position of the left side of the window, in client coordinates.</param>
        /// <param name="y">The new position of the top of the window, in client coordinates.</param>
        /// <param name="cx">The new width of the window, in pixels.</param>
        /// <param name="cy">The new height of the window, in pixels.</param>
        /// <param name="uFlags">The window sizing and positioning flags. This parameter can be a combination of the values in <see cref="SetWindowPosFlags"/></param>
        /// <returns>If the function succeeds, the return value is true. If the function fails, the return value is false.</returns>
        [DllImport(DLL_NAME, SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPosFlags uFlags);

        /// <summary>
        /// Determines the visibility state of the specified window.
        /// </summary>
        /// <param name="hWnd">A handle to the window to be tested.</param>
        /// <returns>If the specified window, its parent window, its parent's parent window, and so forth, have the WS_VISIBLE style, the return value is true.
        /// Otherwise, the return value is false.
        /// Because the return value specifies whether the window has the <see cref="Win32Consts.WS_VISIBLE"/> style, it may be true even if the window is totally obscured by other windows.</returns>
        [DllImport(DLL_NAME)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        /// <summary>
        /// Sets the specified window's show state.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <param name="nCmdShow">Controls how the window is to be shown</param>
        /// <returns>If the window was previously visible, the return value is true. If the window was previously hidden, the return value is false.</returns>
        [DllImport(DLL_NAME)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        /// <summary>
        /// In WM_SYSCOMMAND messages, the four low-order bits of the wParam parameter are used internally by the system.
        /// To obtain the correct result when testing the value of wParam, an application must combine the value 0xFFF0 with the wParam value by using the bitwise AND operator.
        /// </summary>
        /// <param name="wparam">wParam value</param>
        /// <returns><paramref name="wparam"/> combined combine the value 0xFFF0</returns>
        public static int SC_FROM_WPARAM(IntPtr wparam)
        {
            return (int)wparam & 0xfff0;
        }

        /// <summary>
        /// Defines a new window message that is guaranteed to be unique throughout the system. The message value can be used when sending or posting messages.
        /// </summary>
        /// <param name="msg">The message to be registered.</param>
        /// <returns>If the message is successfully registered, the return value is a message identifier in the range 0xC000 through 0xFFFF.
        /// If the function fails, the return value is zero.</returns>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode)]
        public static extern int RegisterWindowMessage(string msg);

        /// <summary>
        /// Sends the specified message to a window or windows. The SendMessage function calls the window procedure for the specified window and does not return until the window procedure has processed the message.
        /// </summary>
        /// <param name="hWnd">A handle to the window whose window procedure will receive the message. If this parameter is HWND_BROADCAST ((HWND)0xffff), the message is sent to all top-level windows in the system, including disabled or invisible unowned windows, overlapped windows, and pop-up windows; but the message is not sent to child windows.</param>
        /// <param name="wMsg">The message to be sent.</param>
        /// <param name="wParam">Additional message-specific information.</param>
        /// <param name="lParam">Additional message-specific information.</param>
        /// <returns>The return value specifies the result of the message processing; it depends on the message sent.</returns>
        [DllImport(DLL_NAME)]
        public static extern int SendMessageA(IntPtr hWnd, uint wMsg, uint wParam, int lParam);

        /// <summary>
        /// The GetMonitorInfo function retrieves information about a display monitor.
        /// </summary>
        /// <param name="hMonitor">A handle to the display monitor of interest.</param>
        /// <param name="lpmi">A pointer to a <see cref="MONITORINFOEX"/> structure that receives information about the specified display monitor.</param>
        /// <returns></returns>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFOEX lpmi);

        /// <summary>
        /// The EnumDisplayMonitors function enumerates display monitors (including invisible pseudo-monitors associated with the mirroring drivers)
        /// that intersect a region formed by the intersection of a specified clipping rectangle and the visible region of a device context.
        /// EnumDisplayMonitors calls an application-defined <see cref="MonitorEnumDelegate"/> callback function once for each monitor that is enumerated
        /// </summary>
        /// <param name="hdc">A handle to a display device context that defines the visible region of interest.
        /// If this parameter is NULL, the hdcMonitor parameter passed to the callback function will be NULL, and the visible region of interest is the virtual screen that encompasses all the displays on the desktop.</param>
        /// <param name="lprcClip">A pointer to a RECT structure that specifies a clipping rectangle. The region of interest is the intersection of the clipping rectangle with the visible region specified by hdc.
        /// If hdc is non-NULL, the coordinates of the clipping rectangle are relative to the origin of the hdc.If hdc is NULL, the coordinates are virtual-screen coordinates.</param>
        /// <param name="lpfnEnum"><see cref="MonitorEnumDelegate"/>  application-defined callback function.</param>
        /// <param name="dwData">Application-defined data that EnumDisplayMonitors passes directly to the MonitorEnumProc function.</param>
        /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.</returns>
        [DllImport(DLL_NAME)]
        public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumDelegate lpfnEnum, IntPtr dwData);

        /// <summary>
        /// Retrieves the specified value from the WNDCLASSEX structure associated with the specified window.
        /// </summary>
        /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
        /// <param name="nIndex">The value to be retrieved.</param>
        /// <returns>If the function succeeds, the return value is the requested value. If the function fails, the return value is zero.</returns>
        public static IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size > 4)
                return GetClassLongPtr64(hWnd, nIndex);
            else
                return new IntPtr(GetClassLongPtr32(hWnd, nIndex));
        }

        /// <summary>
        /// Retrieves the specified value from the WNDCLASSEX structure associated with the specified window.
        /// </summary>
        /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
        /// <param name="nIndex">The value to be retrieved.</param>
        /// <returns>If the function succeeds, the return value is the requested value. If the function fails, the return value is zero.</returns>
        [DllImport(DLL_NAME, EntryPoint = "GetClassLong")]
        public static extern uint GetClassLongPtr32(IntPtr hWnd, int nIndex);

        /// <summary>
        /// Retrieves the specified value from the WNDCLASSEX structure associated with the specified window.
        /// </summary>
        /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
        /// <param name="nIndex">The value to be retrieved.</param>
        /// <returns>If the function succeeds, the return value is the requested value. If the function fails, the return value is zero.</returns>
        [DllImport(DLL_NAME, EntryPoint = "GetClassLongPtr")]
        public static extern IntPtr GetClassLongPtr64(IntPtr hWnd, int nIndex);

        /// <summary>
        /// Retrieves information about the specified window.
        /// </summary>
        /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
        /// <param name="nIndex">The zero-based offset to the value to be retrieved</param>
        /// <returns>If the function succeeds, the return value is the requested value. If the function fails, the return value is zero.</returns>
        [DllImport(DLL_NAME)]
        public static extern ulong GetWindowLongA(IntPtr hWnd, int nIndex);

        /// <summary>
        /// Retrieves information about the specified window.
        /// </summary>
        /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
        /// <param name="index">The zero-based offset to the value to be retrieved</param>
        /// <returns>If the function succeeds, the return value is the requested value. If the function fails, the return value is zero.</returns>
        public static IntPtr GetWindowLongPtr(IntPtr hWnd, int index)
            => IntPtr.Size == 4 ? GetWindowLongPtr32(hWnd, index) : GetWindowLongPtr64(hWnd, index);

        /// <summary>
        /// Retrieves information about the specified window.
        /// </summary>
        /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
        /// <param name="index">The zero-based offset to the value to be retrieved</param>
        /// <returns>If the function succeeds, the return value is the requested value. If the function fails, the return value is zero.</returns>
        [DllImport(DLL_NAME, EntryPoint = "GetWindowLong")]
        private static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, int index);

        /// <summary>
        /// Retrieves information about the specified window.
        /// </summary>
        /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
        /// <param name="index">The zero-based offset to the value to be retrieved</param>
        /// <returns>If the function succeeds, the return value is the requested value. If the function fails, the return value is zero.</returns>
        [DllImport(DLL_NAME, EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int index);

        /// <summary>
        /// Changes an attribute of the specified window
        /// </summary>
        /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
        /// <param name="index">The zero-based offset to the value to be set</param>
        /// <param name="newLong">The replacement value.</param>
        /// <returns>If the function succeeds, the return value is the previous value of the specified offset. If the function fails, the return value is zero. </returns>
        public static IntPtr SetWindowLongPtr(IntPtr hWnd, int index, IntPtr newLong)
            => IntPtr.Size == 4 ? SetWindowLongPtr32(hWnd, index, newLong) : SetWindowLongPtr64(hWnd, index, newLong);

        /// <summary>
        /// Changes an attribute of the specified window
        /// </summary>
        /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
        /// <param name="index">The zero-based offset to the value to be set</param>
        /// <param name="newLong">The replacement value.</param>
        /// <returns>If the function succeeds, the return value is the previous value of the specified offset. If the function fails, the return value is zero. </returns>
        [DllImport(DLL_NAME, EntryPoint = "SetWindowLong")]
        private static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, int index, IntPtr newLong);

        /// <summary>
        /// Changes an attribute of the specified window
        /// </summary>
        /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
        /// <param name="index">The zero-based offset to the value to be set</param>
        /// <param name="newLong">The replacement value.</param>
        /// <returns>If the function succeeds, the return value is the previous value of the specified offset. If the function fails, the return value is zero. </returns>
        [DllImport(DLL_NAME, EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int index, IntPtr newLong);













    }


}
