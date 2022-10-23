using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
using System;
using System.Runtime.InteropServices;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces
{
    /// <summary>
    /// Interface for automatic destination list manipulation
    /// </summary>
    [ComImport]
    [Guid(Win32Consts.IID_IAutomaticDestinationList2)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAutomaticDestinationList2
    {

        [PreserveSig]
        HRESULT Initialize([In, MarshalAs(UnmanagedType.LPWStr)] string? pszAppID, [In, MarshalAs(UnmanagedType.LPWStr)] string? pszProcess, [In, MarshalAs(UnmanagedType.LPWStr)] string? pszFileName);

        [PreserveSig]
        HRESULT HasList([Out, MarshalAs(UnmanagedType.Bool)] out bool pHasList);

        [PreserveSig]
        HRESULT GetList([In] int listType, [In] uint maxCount, [In] int flags, ref Guid riid, [Out, MarshalAs(UnmanagedType.Interface)] out IObjectCollection? list);//list type: 0-pinned,1-recent,2-frequent; flags=0;

        // --- not used ---

        [PreserveSig]
        HRESULT AddUsagePoint([In] IUnknown pItem); //IShellItem or IShellLink

        [PreserveSig]
        HRESULT PinItem([In] IUnknown pItem, int pinIndex); //IShellItem or IShellLink; ret?: -1 - pin, -2 - unpin

        [PreserveSig]
        HRESULT IsPinned(IUnknown pItem, [Out] out int pinIndex);//IShellItem or IShellLink

        [PreserveSig]
        HRESULT RemoveDestination([In] IUnknown pItem);//IShellItem or IShellLink

        [PreserveSig]
        HRESULT SetUsageData([In] IUnknown pItem, [In] IntPtr unknown1, [In] IntPtr unknown2);//IShellItem or IShellLink

        [PreserveSig]
        HRESULT GetUsageData([In] IUnknown pItem, [Out] out IntPtr unknown1, [Out] out IntPtr unknown2);//IShellItem or IShellLink

        [PreserveSig]
        HRESULT ResolveDestination([In] IntPtr hWnd, [In] uint flags, [In] IShellItem shellItem, [In, MarshalAs(UnmanagedType.LPStruct)] Guid id, [Out, MarshalAs(UnmanagedType.Interface)] out IUnknown? unk);

        [PreserveSig]
        HRESULT ClearList([In] int listType);
    }
}
