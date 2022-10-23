using System;
using System.Runtime.InteropServices;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces;

/// <summary>
/// Extends the <see cref="IObjectArray"/> interface by providing methods that enable clients to add and remove objects that support IUnknown in a collection.
/// </summary>
[ComImport]
[Guid(Win32Consts.IID_IObjectCollection)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IObjectCollection
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

    /// <summary>
    /// Adds a single object to the collection.
    /// </summary>
    /// <param name="pvObject">Pointer to the IUnknown of the object to be added to the collection.</param>
    /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [PreserveSig]
    HRESULT AddObject([MarshalAs(UnmanagedType.Interface)] object pvObject);

    /// <summary>
    /// Adds the objects contained in an IObjectArray to the collection.
    /// </summary>
    /// <param name="poaSource">Pointer to the IObjectArray whose contents are to be added to the collection.</param>
    /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [PreserveSig]
    HRESULT AddFromArray([MarshalAs(UnmanagedType.Interface)] IObjectArray poaSource);

    /// <summary>
    /// Removes a single, specified object from the collection.
    /// </summary>
    /// <param name="uiIndex">A pointer to the index of the object within the collection.</param>
    /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [PreserveSig]
    HRESULT RemoveObject(uint uiIndex);

    /// <summary>
    /// Removes all objects from the collection.
    /// </summary>
    /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [PreserveSig]
    HRESULT Clear();
}