using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
using BIND_OPTS = System.Runtime.InteropServices.ComTypes.BIND_OPTS;

// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
namespace net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces;

/// <summary>
/// Provides access to a bind context, which is an object that stores information about a particular moniker binding operation.
/// </summary>
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Win32Consts.IID_IBindCtx)]
internal interface IBindCtx
{
    /// <summary>
    /// Registers an object with the bind context to ensure that the object remains active until the bind context is released.
    /// </summary>
    /// <param name="punk">A pointer to the IUnknown interface on the object that is being registered as bound.</param>
    /// <returns>This method can return the standard return values E_OUTOFMEMORY and S_OK.</returns>
    [PreserveSig]
    HRESULT RegisterObjectBound([MarshalAs(UnmanagedType.Interface)] object punk);

    /// <summary>
    /// Removes the object from the bind context, undoing a previous call to RegisterObjectBound.
    /// </summary>
    /// <param name="punk">A pointer to the IUnknown interface on the object to be removed.</param>
    /// <returns>This method can return the following values: S_OK,MK_E_NOTBOUND</returns>
    [PreserveSig]
    HRESULT RevokeObjectBound([MarshalAs(UnmanagedType.Interface)] object punk);

    /// <summary>
    /// Releases all pointers to all objects that were previously registered by calls to RegisterObjectBound.
    /// </summary>
    /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [PreserveSig]
    HRESULT ReleaseBoundObjects();

    /// <summary>
    /// Sets new values for the binding parameters stored in the bind context.
    /// </summary>
    /// <param name="pbindopts">A pointer to a BIND_OPTS structure containing the binding parameters.</param>
    /// <returns>This method can return the standard return values E_OUTOFMEMORY and S_OK.</returns>
    [PreserveSig]
    HRESULT SetBindOptions([In()] ref BIND_OPTS pbindopts);

    /// <summary>
    /// Retrieves the binding options stored in this bind context.
    /// </summary>
    /// <param name="pbindopts">A pointer to an initialized structure that receives the current binding parameters on return.</param>
    /// <returns>This method can return the standard return values E_UNEXPECTED and S_OK.</returns>
    [PreserveSig]
    HRESULT GetBindOptions(ref BIND_OPTS pbindopts);

    /// <summary>
    /// Retrieves an interface pointer to the running object table (ROT) for the computer on which this bind context is running.
    /// </summary>
    /// <param name="pprot">The address of a IRunningObjectTable* pointer variable that receives the interface pointer to the running object table. If an error occurs, *pprot is set to NULL. If *pprot is non-NULL, the implementation calls AddRef on the running table object; it is the caller's responsibility to call Release.</param>
    /// <returns>This method can return the standard return values E_OUTOFMEMORY, E_UNEXPECTED, and S_OK.</returns>
    [PreserveSig]
    HRESULT GetRunningObjectTable(out IRunningObjectTable pprot);

    /// <summary>
    /// Associates an object with a string key in the bind context's string-keyed table of pointers.
    /// </summary>
    /// <param name="pszKey">The bind context string key under which the object is being registered. Key string comparison is case-sensitive.</param>
    /// <param name="punk">A pointer to the IUnknown interface on the object that is to be registered.The method calls AddRef on the pointer.</param>
    /// <returns>This method can return the standard return values E_OUTOFMEMORY and S_OK.</returns>
    [PreserveSig]
    HRESULT RegisterObjectParam([MarshalAs(UnmanagedType.LPWStr)] string pszKey, [MarshalAs(UnmanagedType.Interface)] object punk);

    /// <summary>
    /// Retrieves an interface pointer to the object associated with the specified key in the bind context's string-keyed table of pointers.
    /// </summary>
    /// <param name="pszKey">The bind context string key to be searched for. Key string comparison is case-sensitive.</param>
    /// <param name="ppunk">The address of an IUnknown* pointer variable that receives the interface pointer to the object associated with pszKey. When successful, the implementation calls AddRef on *ppunk. It is the caller's responsibility to call Release. If an error occurs, the implementation sets *ppunk to NULL.</param>
    /// <returns>If the method succeeds, the return value is S_OK. Otherwise, it is E_FAIL.</returns>
    [PreserveSig]
    HRESULT GetObjectParam([MarshalAs(UnmanagedType.LPWStr)] string pszKey, [MarshalAs(UnmanagedType.Interface)] out object ppunk);

    /// <summary>
    /// Retrieves a pointer to an interface that can be used to enumerate the keys of the bind context's string-keyed table of pointers.
    /// </summary>
    /// <param name="ppenum">The address of an IEnumString* pointer variable that receives the interface pointer to the enumerator. If an error occurs, *ppenum is set to NULL. If *ppenum is non-NULL, the implementation calls AddRef on *ppenum; it is the caller's responsibility to call Release.</param>
    /// <returns>This method can return the standard return values E_OUTOFMEMORY and S_OK.</returns>
    [PreserveSig]
    HRESULT EnumObjectParam(out IEnumString ppenum);

    /// <summary>
    /// Removes the specified key and its associated pointer from the bind context's string-keyed table of objects. The key must have previously been inserted into the table with a call to RegisterObjectParam.
    /// </summary>
    /// <param name="pszKey">The bind context string key to be removed. Key string comparison is case-sensitive.</param>
    /// <returns>This method can return the following values: S_OK, S_FALSE</returns>
    [PreserveSig]
    HRESULT RevokeObjectParam([MarshalAs(UnmanagedType.LPWStr)] string pszKey);
}