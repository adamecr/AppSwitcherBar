using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable CommentTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces
{


    /// <summary>
    /// Represents an object in the Shell.
    /// Methods are provided to control the Shell and to execute commands within the Shell.
    /// There are also methods to obtain other Shell-related objects.
    /// IShellDispatch is implemented and accessed through the Shell object.
    /// </summary>
    [ComImport]
    [Guid(Win32Consts.IID_IShellDispatch6)]
    internal interface IShellDispatch6
    {
        /// <summary>
        /// <para>Contains an object that represents an application.</para>
        /// </summary>
        [DispId(0x60020000)]
        object Application { [return: MarshalAs(UnmanagedType.IDispatch)][MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x60020000)] get; }

        /// <summary>
        /// <para>Retrieves an object that represents the parent of the current object.</para>
        /// </summary>
        [DispId(0x60020001)]
        object Parent { [return: MarshalAs(UnmanagedType.IDispatch)][MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x60020001)] get; }

        /// <summary>Creates and returns a Folder object for the specified folder.</summary>
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x60020002)]
        IUnknown NameSpace([In, MarshalAs(UnmanagedType.Struct)] object vDir);

        /// <summary>
        /// Creates a dialog box that enables the user to select a folder and then returns the selected folder's <c>Folder</c> object.
        /// </summary>
        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x60020003)]
        IUnknown BrowseForFolder([In] int Hwnd, [In, MarshalAs(UnmanagedType.BStr)] string Title, [In] int Options, [In, Optional, MarshalAs(UnmanagedType.Struct)] object RootFolder);

        /// <summary>
        /// Creates and returns a <c>ShellWindows</c> object. This object represents a collection of all of the open windows that belong
        /// to the Shell.
        /// </summary>
        [return: MarshalAs(UnmanagedType.IDispatch)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x60020004)]
        object Windows();

        /// <summary>Opens the specified folder.</summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x60020005)]
        void Open([In, MarshalAs(UnmanagedType.Struct)] object vDir);

        /// <summary>Opens a specified folder in a Windows Explorer window.</summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x60020006)]
        void Explore([In, MarshalAs(UnmanagedType.Struct)] object vDir);

        /// <summary>
        /// Minimizes all of the windows on the desktop. This method has the same effect as right-clicking the taskbar and selecting
        /// <c>Minimize All Windows</c> on older systems or clicking the <c>Show Desktop</c> icon on the taskbar.
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x60020007)]
        void MinimizeAll();

        /// <summary>
        /// Restores all desktop windows to the state they were in before the last <c>MinimizeAll</c> command. This method has the same
        /// effect as right-clicking the taskbar and selecting <c>Undo Minimize All Windows</c> (on older systems) or a second clicking
        /// of the <c>Show Desktop</c> icon in the taskbar.
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x60020008)]
        void UndoMinimizeALL();

        /// <summary>Displays the <c>Run</c> dialog to the user.</summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x60020009)]
        void FileRun();

        /// <summary>
        /// Cascades all of the windows on the desktop. This method has the same effect as right-clicking the taskbar and selecting
        /// <c>Cascade windows</c>.
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x6002000a)]
        void CascadeWindows();

        /// <summary>
        /// Tiles all of the windows on the desktop vertically. This method has the same effect as right-clicking the taskbar and
        /// selecting <c>Show windows side by side</c>.
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x6002000b)]
        void TileVertically();

        /// <summary>
        /// Tiles all of the windows on the desktop horizontally. This method has the same effect as right-clicking the taskbar and
        /// selecting <c>Show windows stacked</c>.
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x6002000c)]
        void TileHorizontally();

        /// <summary>
        /// Displays the <c>Shut Down Windows</c> dialog box. This is the same as clicking the <c>Start</c> menu and selecting <c>Shut Down</c>.
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x6002000d)]
        void ShutdownWindows();

        /// <summary>This method is not implemented.</summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x6002000e)]
        void Suspend();

        /// <summary>
        /// Ejects the computer from its docking station. This is the same as clicking the <c>Start</c> menu and selecting <c>Eject
        /// PC</c>, if your computer supports this command.
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x6002000f)]
        void EjectPC();

        /// <summary>
        /// Displays the <c>Date and Time</c> dialog box. This method has the same effect as right-clicking the clock in the taskbar
        /// status area and selecting <c>Adjust date/time</c>.
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x60020010)]
        void SetTime();

        /// <summary>
        /// Displays the <c>Taskbar and Start Menu Properties</c> dialog box. This method has the same effect as right-clicking the
        /// taskbar and selecting <c>Properties</c>.
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x60020011)]
        void TrayProperties();

        /// <summary>
        /// Displays the Windows Help and Support window. This method has the same effect as clicking the <c>Start</c> menu and selecting
        /// <c>Help and Support</c>.
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x60020012)]
        void Help();

        /// <summary>
        /// Displays the <c>Find: All Files</c> dialog box. This is the same as clicking the <c>Start</c> menu and then selecting <c>Search</c>.
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x60020013)]
        void FindFiles();

        /// <summary>
        /// Displays the <c>Search Results: Computers</c> dialog box. The dialog box shows the result of the search for a specified computer.
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x60020014)]
        void FindComputer();

        /// <summary>Refreshes the contents of the <c>Start</c> menu. Used only with systems preceding Windows XP.</summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x60020015)]
        void RefreshMenu();

        /// <summary>
        /// Runs the specified Control Panel application. If the application is already open, it will activate the running instance.
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x60020016)]
        void ControlPanelItem([In, MarshalAs(UnmanagedType.BStr)] string bstrDir);

        /// <summary>Retrieves a group's restriction setting from the registry.</summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x60030000)]
        int IsRestricted([In, MarshalAs(UnmanagedType.BStr)] string sGroup, [In, MarshalAs(UnmanagedType.BStr)] string sRestriction);

        /// <summary>Performs a specified operation on a specified file.</summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x60030001)]
        void ShellExecute([In, MarshalAs(UnmanagedType.BStr)] string sFile, [In, Optional, MarshalAs(UnmanagedType.Struct)] object? vArguments, [In, Optional, MarshalAs(UnmanagedType.Struct)] object vDirectory, [In, Optional, MarshalAs(UnmanagedType.Struct)] object vOperation, [In, Optional, MarshalAs(UnmanagedType.Struct)] object vShow);

        /// <summary>Displays the <c>Find Printer</c> dialog box.</summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x60030002)]
        void FindPrinter([In, Optional, MarshalAs(UnmanagedType.BStr)] string Name, [In, Optional, MarshalAs(UnmanagedType.BStr)] string location, [In, Optional, MarshalAs(UnmanagedType.BStr)] string model);

        /// <summary>Retrieves system information.</summary>
        [return: MarshalAs(UnmanagedType.Struct)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x60030003)]
        object GetSystemInformation([In, MarshalAs(UnmanagedType.BStr)] string sName);

        /// <summary>Starts a named service.</summary>
        [return: MarshalAs(UnmanagedType.Struct)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x60030004)]
        object ServiceStart([In, MarshalAs(UnmanagedType.BStr)] string sServiceName, [In, MarshalAs(UnmanagedType.Struct)] object vPersistent);

        /// <summary>Stops a named service.</summary>
        [return: MarshalAs(UnmanagedType.Struct)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x60030005)]
        object ServiceStop([In, MarshalAs(UnmanagedType.BStr)] string sServiceName, [In, MarshalAs(UnmanagedType.Struct)] object vPersistent);

        /// <summary>Returns a value that indicates whether a particular service is running.</summary>
        [return: MarshalAs(UnmanagedType.Struct)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x60030006)]
        object IsServiceRunning([In, MarshalAs(UnmanagedType.BStr)] string sServiceName);

        /// <summary>Determines if the current user can start and stop the named service.</summary>
        [return: MarshalAs(UnmanagedType.Struct)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x60030007)]
        object CanStartStopService([In, MarshalAs(UnmanagedType.BStr)] string sServiceName);

        /// <summary>Displays a browser bar.</summary>
        [return: MarshalAs(UnmanagedType.Struct)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x60030008)]
        object ShowBrowserBar([In, MarshalAs(UnmanagedType.BStr)] string sCLSID, [In, MarshalAs(UnmanagedType.Struct)] object vShow);

        /// <summary>Adds a file to the most recently used (MRU) list.</summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x60040000)]
        void AddToRecent([In, MarshalAs(UnmanagedType.Struct)] object varFile, [In, Optional, MarshalAs(UnmanagedType.BStr)] string bstrCategory);

        /// <summary>Displays the <c>Windows Security</c> dialog box.</summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x60050000)]
        void WindowsSecurity();

        /// <summary>Displays or hides the desktop.</summary>
        // https://docs.microsoft.com/en-us/windows/win32/shell/ishelldispatch4-toggledesktop
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x60050001)]
        void ToggleDesktop();

        /// <summary>Gets the value for a specified Windows Internet Explorer policy.</summary>
        [return: MarshalAs(UnmanagedType.Struct)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x60050002)]
        object ExplorerPolicy([In, MarshalAs(UnmanagedType.BStr)] string bstrPolicyName);

        /// <summary>Retrieves a global Shell setting.</summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x60050003)]
        bool GetSetting([In] int lSetting);

        /// <summary>Displays your open windows in a 3D stack that you can flip through.</summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x60060000)]
        void WindowSwitcher();

        /// <summary>Displays the Apps Search pane, which normally appears when you begin to type a search term from the Start screen.</summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(0x60070000)]
        void SearchCommand();
    }
}
