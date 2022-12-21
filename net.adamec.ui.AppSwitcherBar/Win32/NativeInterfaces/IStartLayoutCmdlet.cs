using System.Runtime.InteropServices;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces
{
    /// <summary>
    /// Interface retrieved from Export-StartLayout cmdlet dll
    /// </summary>
    [Guid(Win32Consts.IID_IStartLayoutCmdlet)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IStartLayoutCmdlet
    {
        /// <summary>
        /// Export start menu layout
        /// </summary>
        /// <param name="pszFilePath">File to export the start menu layout to</param>
        void ExportStartLayout([MarshalAs(UnmanagedType.LPWStr), In] string pszFilePath);
        void ExportStartLayoutWithDesktopApplicationIDs([MarshalAs(UnmanagedType.LPWStr), In] string pszFilePath);
        void ValidateLayoutFile([MarshalAs(UnmanagedType.LPWStr), In] string pszFilePath);
        void ExportEdgeAssets([MarshalAs(UnmanagedType.LPWStr), In] string pszFilePath);
    }
}
