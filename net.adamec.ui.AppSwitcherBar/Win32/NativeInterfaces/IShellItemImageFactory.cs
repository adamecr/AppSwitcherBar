using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;

// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces;

/// <summary>
/// Exposes a method to return either icons or thumbnails for Shell items. If no thumbnail or icon is available for the requested item, a per-class icon may be provided from the Shell.
/// </summary>
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Win32Consts.IID_IShellItemImageFactory)]
internal interface IShellItemImageFactory
{
    /// <summary>
    /// Gets an HBITMAP that represents an IShellItem. The default behavior is to load a thumbnail.
    /// If there is no thumbnail for the current IShellItem, it retrieves an HBITMAP for the icon of the item. The thumbnail or icon is extracted if it is not currently cached.
    /// </summary>
    /// <remarks>
    /// Icon extraction can be time consuming.
    /// This method generally should not be called from a UI thread to avoid causing that thread to become unresponsive.
    /// You can call IShellItemImageFactory::GetImage on a UI thread if you set the SIIGBF_INCACHEONLY flag.
    /// However, if the image is not found in the cache, the calling application should be prepared to launch a background thread to extract the image.
    /// An extraction should never be done on a UI thread.
    /// </remarks>
    /// <param name="size">A structure that specifies the size of the image to be received.</param>
    /// <param name="flags">Options</param>
    /// <param name="phbm">Pointer to a value that, when this method returns successfully, receives the handle of the retrieved bitmap.
    /// It is the responsibility of the caller to free this retrieved resource through DeleteObject when it is no longer needed.</param>
    /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [PreserveSig]
    [MethodImpl(MethodImplOptions.PreserveSig)]
    HRESULT GetImage([MarshalAs(UnmanagedType.Struct), In] SIZE size, [In] SIIGBF flags, out IntPtr phbm);
}