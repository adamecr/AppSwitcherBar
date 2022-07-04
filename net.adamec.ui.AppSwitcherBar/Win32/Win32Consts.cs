using System;
using net.adamec.ui.AppSwitcherBar.Win32.ShellExt;

namespace net.adamec.ui.AppSwitcherBar.Win32
{
    /// <summary>
    /// Win32 api constants
    /// </summary>
    internal static class Win32Consts
    {
        /// <summary>
        /// Operation successful
        /// </summary>
        internal const int S_OK = 0x00000000;
        internal const int E_INVALIDARG = unchecked((int)0x80070057);

        /// <summary>
        /// The WM_ACTIVATE message is sent to both the window being activated and the window being deactivated. If the windows use the same input queue, the message is sent synchronously, first to the window procedure of the top-level window being deactivated, then to the window procedure of the top-level window being activated. If the windows use different input queues, the message is sent asynchronously, so the window is activated immediately.
        /// </summary>
        internal const int WM_ACTIVATE = 0x0006;
        /// <summary>
        /// The WM_WINDOWPOSCHANGED message is sent to a window whose size, position, or place in the Z order has changed as a result of a call to the SetWindowPos function or another window-management function.
        /// </summary>
        internal const int WM_WINDOWPOSCHANGED = 0x0047;
        /// <summary>
        /// A window receives this message when the user chooses a command from the Window menu, clicks the maximize button, minimize button, restore button, close button, or moves the form. You can stop the form from moving by filtering this out.
        /// </summary>
        internal const int WM_SYSCOMMAND = 0x0112;
        /// <summary>
        /// The WM_WINDOWPOSCHANGING message is sent to a window whose size, position, or place in the Z order is about to change as a result of a call to the SetWindowPos function or another window-management function.
        /// </summary>
        internal const int WM_WINDOWPOSCHANGING = 0x0046;
        /// <summary>
        /// The WM_GETICON message is sent to a window to retrieve a handle to the large or small icon associated with a window. The system displays the large icon in the ALT+TAB dialog, and the small icon in the window caption.
        /// </summary>
        internal const int WM_GETICON = 0x007F;

        /// <summary>
        /// The window is initially visible. This style can be turned on and off by using the ShowWindow or SetWindowPos function.
        /// </summary>
        internal const ulong WS_VISIBLE = 0x10000000L;
        /// <summary>
        /// The window has a thin-line border.
        /// </summary>
        internal const ulong WS_BORDER = 0x00800000L;
        /// <summary>
        /// The window has a title bar (includes the WS_BORDER style).
        /// </summary>
        internal const ulong WS_CAPTION = 0x00c00000L;
        /// <summary>
        /// The window is initially minimized.
        /// </summary>
        internal const ulong WS_MINIMIZE = 0x20000000L;
        /// <summary>
        /// The window is a child window. A window with this style cannot have a menu bar. This style cannot be used with the WS_POPUP style.
        /// </summary>
        internal const ulong WS_CHILD = 0x40000000L;
        /// <summary>
        /// Forces a top-level window onto the taskbar when the window is visible.
        /// </summary>
        internal const ulong WS_EX_APPWINDOW = 0x00040000;
        /// <summary>
        /// Specifies a window that is intended to be used as a floating toolbar.
        /// A tool window has a title bar that is shorter than a normal title bar, and the window title is drawn using a smaller font.
        /// A tool window does not appear in the taskbar or in the dialog that appears when the user presses ALT+TAB.
        /// If a tool window has a system menu, its icon is not displayed on the title bar.
        /// However, you can display the system menu by right-clicking or by typing ALT+SPACE.
        /// </summary>
        internal const ulong WS_EX_TOOLWINDOW = 0x00000080;

        /// <summary>
        /// Get or sets a window style.
        /// </summary>
        internal const int GWL_STYLE = -16;
        /// <summary>
        /// Get or sets a extended window style.
        /// </summary>
        internal const int GWL_EXSTYLE = -20;

        /// <summary>
        /// Retrieves a handle to the small icon associated with the class
        /// </summary>
        internal const int GCLP_HICONSM = -34;
        /// <summary>
        /// Retrieves a handle to the icon associated with the class
        /// </summary>
        internal const int GCLP_HICON = -14;

        /// <summary>
        /// Minimizes the specified window and activates the next top-level window in the Z order.
        /// </summary>
        internal const int SW_MINIMIZE = 6;
        /// <summary>
        /// Activates and displays the window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when restoring a minimized window
        /// </summary>
        internal const int SW_RESTORE = 9;

        /// <summary>
        /// Retains the current position (ignores X and Y parameters).
        /// </summary>
        internal const int SWP_NOMOVE = 0x0002;
        /// <summary>
        /// Retains the current size (ignores the cx and cy parameters).
        /// </summary>
        internal const int SWP_NOSIZE = 0x0001;

        /// <summary>
        /// A value for the fVisible member has been specified.
        /// </summary>
        internal const int DWM_TNP_VISIBLE = 0x8;
        /// <summary>
        /// A value for the opacity member has been specified.
        /// </summary>
        internal const int DWM_TNP_OPACITY = 0x4;
        /// <summary>
        /// A value for the rcDestination member has been specified.
        /// </summary>
        internal const int DWM_TNP_RECTDESTINATION = 0x1;

        /// <summary>
        /// Device name max length
        /// </summary>
        internal const int CCHDEVICENAME = 32;

        /// <summary>
        /// Places the window above all non-topmost windows. The window maintains its topmost position even when it is deactivated.
        /// </summary>
        internal static readonly IntPtr HWND_TOPMOST = new(-1);
        /// <summary>
        /// Places the window above all non-topmost windows (that is, behind all topmost windows). This flag has no effect if the window is already a non-topmost window.
        /// </summary>
        internal static readonly IntPtr HWND_NOTOPMOST = new(-2);
        /// <summary>
        /// Places the window at the top of the Z order.
        /// </summary>
        internal static readonly IntPtr HWND_TOP = new(0);
        /// <summary>
        /// Places the window at the bottom of the Z order. If the hWnd parameter identifies a topmost window, the window loses its topmost status and is placed at the bottom of all other windows
        /// </summary>
        internal static readonly IntPtr HWND_BOTTOM = new(1);

        /// <summary>
        /// <see cref="IPropertyStore"/> interface ID
        /// </summary>
        internal const string IID_IPropertyStore = "886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99"; 
        
        /// <summary>
        /// AppUserModel property key
        /// </summary>
        internal const string PKEY_AppUserModel_ID = "9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3";
    }
}
