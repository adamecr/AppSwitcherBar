using System;
using System.Runtime.InteropServices;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
// ReSharper disable IdentifierTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces
{
    /// <summary>
    /// Provides methods for retrieving the appId.
    /// Undocumented interface, so it might change or not work as expected
    /// https://a-whiter.livejournal.com/1266.html
    /// </summary>
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("DE25675A-72DE-44B4-9373-05170450C140")]
    internal interface IApplicationResolver
    {
        /// <summary>
        /// Gets the application id from <see cref="IShellItem"/> representing a shortcut
        /// </summary>
        /// <param name="shellItem">Shortcut shell item</param>
        /// <param name="appId">Retrieved application id</param>
        /// <returns>Result of operation</returns>
        [PreserveSig]
        HRESULT GetAppIDForShortcut([In] IShellItem shellItem, [Out,MarshalAs(UnmanagedType.LPWStr)] out string appId);
        /// <summary>
        /// Gets the application id from <see cref="IShellItem"/> representing a shortcut and already retrieved <see cref="IShellLinkW"/> (?)
        /// </summary>
        /// <param name="shellLink">Shortcut (link)</param>
        /// <param name="shellItem">Shortcut shell item</param>
        /// <param name="appId">Retrieved application id</param>
        /// <returns>Result of operation</returns>
        [PreserveSig]
        HRESULT GetAppIDForShortcutObject([In] IShellLinkW shellLink, [In] IShellItem shellItem, [Out, MarshalAs(UnmanagedType.LPWStr)] out string appId);
        /// <summary>
        /// Gets the application id from window
        /// </summary>
        /// <param name="hwnd">HWND of window</param>
        /// <param name="appId">Retrieved application id</param>
        /// <param name="_">Unknown</param>
        /// <param name="__">Unknown</param>
        /// <param name="____">Unknown</param>
        /// <returns>Result of operation</returns>
        [PreserveSig]
        HRESULT GetAppIDForWindow([In] IntPtr hwnd, [MarshalAs(UnmanagedType.LPWStr)] out string appId, out IntPtr _, out IntPtr __, out IntPtr ____);
        /// <summary>
        /// Gets the application if from process
        /// </summary>
        /// <param name="processId">Process ID</param>
        /// <param name="appId">Retrieved application id</param>
        /// <param name="_">Unknown</param>
        /// <param name="__">Unknown</param>
        /// <param name="____">Unknown</param>
        /// <returns>Result of operation</returns>
        [PreserveSig]
        HRESULT GetAppIDForProcess(uint processId, [MarshalAs(UnmanagedType.LPWStr)] out string appId, out IntPtr _, out IntPtr __, out IntPtr ____);

        //The rest of interface not used
    }

   
}
