using System;
using System.Runtime.InteropServices;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces
{
    /// <summary>
    /// Exposes a standard set of methods that enumerate the pointers to item identifier lists (PIDLs) of the items in a Shell folder
    /// </summary>
    [ComImport]
    [Guid(Win32Consts.IID_IEnumFullIDList)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IEnumFullIDList
    {
        /// <summary>
        /// Retrieves a specified number of IDLIST_ABSOLUTE items.
        /// </summary>
        /// <param name="celt">The number of items referenced in the array referenced by the rgelt parameter.</param>
        /// <param name="rgelt">
        /// On success, contains a PIDL array.
        /// The implementation must allocate these item identifiers using CoTaskMemAlloc.
        /// The calling application is responsible for freeing the item identifiers using CoTaskMemFree.
        /// </param>
        /// <param name="pceltFetched">On success, contains a pointer to a value that receives a count of the absolute item identifiers actually returned in rgelt.
        /// The count can be smaller than the value specified in the celt parameter.
        /// This parameter can be NULL on entry only if celt is 1, because in that case the method can only retrieve one (S_OK) or zero (S_FALSE) items.</param>
        /// <returns>Returns S_OK if the method successfully retrieved the requested celt elements.
        /// This method only returns S_OK if the full count of requested items are successfully retrieved.
        /// S_FALSE indicates that more items were requested than remained in the enumeration.
        /// The value pointed to by the pceltFetched parameter specifies the actual number of items retrieved.
        /// Note that the value will be 0 if there are no more items to retrieve.</returns>
        [PreserveSig]
        HRESULT Next(uint celt, out IntPtr rgelt, out int pceltFetched);

        /// <summary>
        /// Skips a specified number of IDLIST_ABSOLUTE items.
        /// </summary>
        /// <param name="celt">The number of items to skip.</param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [PreserveSig]
        HRESULT Skip(uint celt);

        /// <summary>
        /// Returns the enumerator to the beginning of the enumeration sequence.
        /// </summary>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [PreserveSig]
        HRESULT Reset();

        /// <summary>
        /// Creates a new item enumeration object with the same contents and state as the current one.
        /// </summary>
        /// <returns>Cloned enumeration object</returns>
        [return: MarshalAs(UnmanagedType.Interface)]
        IEnumFullIDList Clone();
    }
}
