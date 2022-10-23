using System;
using System.Runtime.InteropServices;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces;

/// <summary>
/// Exposes methods that enable clients to access items in a collection of objects that support IUnknown.
/// </summary>
[ComImport]
[Guid(Win32Consts.IID_IObjectArray)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IObjectArray
{
    /// <summary>
    /// Provides a count of the objects in the collection.
    /// </summary>
    /// <param name="cObjects">The number of objects in the collection.</param>
    /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [PreserveSig]
    HRESULT GetCount(out uint cObjects);

    /// <summary>
    /// Provides a pointer to a specified object's interface. The object and interface are specified by index and interface ID.
    /// </summary>
    /// <param name="iIndex">The index of the object</param>
    /// <param name="riid">Reference to the desired interface ID.</param>
    /// <param name="ppvObject">Receives the interface pointer requested in riid.</param>
    /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [PreserveSig]
    HRESULT GetAt(uint iIndex, ref Guid riid, [Out][MarshalAs(UnmanagedType.Interface)] out object ppvObject);
}