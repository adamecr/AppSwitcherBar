using System;
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeConstants
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
        /// <summary>
        /// Invalid argument
        /// </summary>
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
        /// The WM_CLOSE message is sent as a signal that a window or an application should terminate.
        /// </summary>
        internal const int WM_CLOSE = 0x0010;

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
        /// Does not activate the window. If this flag is not set, the window is activated and moved to the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).
        /// </summary>
        internal const int SWP_NOACTIVATE = 0x0010;

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
        /// Link property key format
        /// </summary>
        internal const string PKEY_FORMAT_Link_TargetParsingPath = "B9B4B3FC-2B51-4A42-B5D8-324146AFCF25";

        /// <summary>
        /// AppUserModel property key format
        /// </summary>
        internal const string PKEY_FORMAT_AppUserModel = "9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3";
        /// <summary>
        /// Title property key format
        /// </summary>
        internal const string PKEY_FORMAT_Title = "F29F85E0-4FF9-1068-AB91-08002B27B3D9";
        /// <summary>
        /// Use with DwmGetWindowAttribute.If the window is cloaked, provides one of the following values explaining why.
        /// DWM_CLOAKED_APP(value 0x0000001). The window was cloaked by its owner application.
        /// DWM_CLOAKED_SHELL(value 0x0000002). The window was cloaked by the Shell.
        /// DWM_CLOAKED_INHERITED(value 0x0000004). The cloak value was inherited from its owner window.
        /// </summary>
        internal const uint DWMWA_CLOAKED = 14;

        /// <summary>
        /// The retrieved handle identifies the specified window's owner window, if any.
        /// </summary>
        internal const int GW_OWNER = 4;

        /// <summary>Maximal Length of unmanaged Windows-Path-strings</summary>
        internal const int MAX_PATH = 260;
        /// <summary>Maximal Length of unmanaged Typename</summary>
        internal const int MAX_TYPE = 80;

        internal static readonly Guid BHID_EnumItems = new("94f60519-2850-4924-aa5a-d15e84868039");

        //class IDs
        internal const string CLSID_ShellLink = "00021401-0000-0000-C000-000000000046";
        internal const string CLSID_TaskbanPin = "90aa3a4e-1cba-4233-b8bb-535773d48449";
        internal const string CLSID_StartMenuPin = "A2A9545D-A0C2-42B4-9708-A0B2BADD77C8";
        internal const string CLSID_KnownFolderManager = "4df0c730-df9d-4ae3-9153-aa6b82e9795a";
        internal const string CLSID_ApplicationResolver = "660B90C8-73A9-4B58-8CAE-355B7F55341B";

        //interface IDs
        internal const string IID_IPropertyStore = "886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99";
        internal const string IID_IPersistStream = "00000109-0000-0000-C000-000000000046";
        internal const string IID_IShellFolder = "000214E6-0000-0000-C000-000000000046";
        internal const string IID_IShellLink = "000214F9-0000-0000-C000-000000000046";
        internal const string IID_IShellItem = "43826d1e-e718-42ee-bc55-a1e261c37bfe";
        internal const string IID_IShellItem2 = "7e9fb0d3-919f-4307-ab2e-9b1860310c93";
        internal const string IID_IShellItemArray = "B63EA76D-1F85-456F-A19C-48159EFA858B";
        internal const string IID_IEnumIDList = "000214F2-0000-0000-C000-000000000046";
        internal const string IID_IEnumFullIDList = "d0191542-7954-4908-bc06-b2360bbe45ba";
        internal const string IID_IShellItemImageFactory = "bcc18b79-ba16-442f-80c4-8a59c30c463b";
        internal const string IID_IEnumShellItems = "70629033-e363-4a28-a567-0db78006e6d7";
        internal const string IID_IBindCtx = "0000000e-0000-0000-C000-000000000046";
        internal const string IID_IUnknown = "00000000-0000-0000-C000-000000000046";
        internal const string IID_IShellLinkDataList = "45e2b4ae-b1c3-11d0-b92f-00a0c90312e1";
        internal const string IID_IPropertyDescription = "6F79D558-3E96-4549-A1D1-7D75D2288814";
        internal const string IID_IPropertyDescription2 = "57D2EDED-5062-400E-B107-5DAE79FE57A6";
        internal const string IID_IPropertyDescriptionList = "1F9FC1D0-C39B-4B26-817F-011967D3440E";
        internal const string IID_IPropertyEnumType = "11E1FBF9-2D56-4A6B-8DB3-7CD193A471F2";
        internal const string IID_IPropertyEnumType2 = "9B6E051C-5DDD-4321-9070-FE2ACB55E794";
        internal const string IID_IPropertyEnumTypeList = "A99400F4-3D84-4557-94BA-1242FB2CC9A6";
        internal const string IID_IPropertyStoreCapabilities = "c8e2d566-186e-4d49-bf41-6909ead56acc";
        internal const string IID_IApplicationActivationManager = "2e941141-7f97-4756-ba1d-9decde894a3d";
        internal const string IID_IPinnedList3 = "0dd79ae2-d156-45d4-9eeb-3b549769e940";
        internal const string IID_IFlexibleTaskbarPinnedList = "60274FA2-611F-4B8A-A293-F27BF103D148";
        internal const string IID_IKnownFolder = "3AA7AF7E-9B36-420c-A8E3-F77D4674A488";
        internal const string IID_IKnownFolderManager = "8BE2D872-86AA-4d47-B776-32CCA40C7018";

    }
}
