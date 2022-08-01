using System;
using System.Runtime.InteropServices;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
namespace net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces;

/// <summary>
/// Exposes a standard set of methods used to enumerate the pointers to item identifier lists (PIDLs) of the items in a Shell folder.
/// When a folder's IShellFolder::EnumObjects method is called, it creates an enumeration object and passes a pointer to the object's IEnumIDList interface back to the calling application.
/// </summary>
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Win32Consts.IID_IEnumIDList)]
internal interface IEnumIDList
{
    /// <summary>
    /// Retrieves the specified number of item identifiers in the enumeration sequence and advances the current position by the number of items retrieved.
    /// </summary>
    /// <param name="celt">The number of elements in the array referenced by the rgelt parameter.</param>
    /// <param name="rgelt">The address of a pointer to an array of ITEMIDLIST pointers that receive the item identifiers.
    /// The implementation must allocate these item identifiers using CoTaskMemAlloc. The calling application is responsible for freeing the item identifiers using CoTaskMemFree.
    /// The ITEMIDLIST structures returned in the array are relative to the IShellFolder being enumerated.</param>
    /// <param name="pceltFetched">A pointer to a value that receives a count of the item identifiers actually returned in rgelt. The count can be smaller than the value specified in the celt parameter.
    /// This parameter can be NULL on entry only if celt = 1, because in that case the method can only retrieve one (S_OK) or zero (S_FALSE) items.</param>
    /// <returns>Returns S_OK if the method successfully retrieved the requested celt elements. This method only returns S_OK if the full count of requested items are successfully retrieved.
    /// S_FALSE indicates that more items were requested than remained in the enumeration.The value pointed to by the pceltFetched parameter specifies the actual number of items retrieved. Note that the value will be 0 if there are no more items to retrieve.
    /// Returns a COM-defined error value otherwise.</returns>
    [PreserveSig]
    HRESULT Next(uint celt, out IntPtr rgelt, out int pceltFetched);

    /// <summary>
    /// Skips the specified number of elements in the enumeration sequence.
    /// </summary>
    /// <param name="celt">The number of item identifiers to skip.</param>
    /// <returns>Returns S_OK if successful, or a COM-defined error value otherwise.</returns>
    [PreserveSig]
    HRESULT Skip(uint celt);

    /// <summary>
    /// Returns to the beginning of the enumeration sequence.
    /// </summary>
    /// <returns>Returns S_OK if successful, or a COM-defined error value otherwise.</returns>
    [PreserveSig]
    HRESULT Reset();

    /// <summary>
    /// Creates a new item enumeration object with the same contents and state as the current one.
    /// </summary>
    /// <returns>The address of a pointer to the new enumeration object. The calling application must eventually free the new object by calling its Release member function.</returns>
    [return: MarshalAs(UnmanagedType.Interface)]
    IEnumIDList Clone();
}