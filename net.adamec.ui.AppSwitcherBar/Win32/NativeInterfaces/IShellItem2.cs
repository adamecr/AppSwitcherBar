using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
using net.adamec.ui.AppSwitcherBar.Win32.Services.Shell.Properties;

// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces;

/// <summary>
/// Exposes methods that retrieve information about a Shell item. IShellItem and IShellItem2 are the preferred representations of items in any new code.
/// </summary>
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Win32Consts.IID_IShellItem2)]
internal interface IShellItem2 : IShellItem
{
    #region IShellItem redeclarations
    /// <summary>
    /// Binds to a handler for an item as specified by the handler ID value (BHID).
    /// </summary>
    /// <param name="pbc">A pointer to an IBindCtx interface on a bind context object. Used to pass optional parameters to the handler. The contents of the bind context are handler-specific. For example, when binding to BHID_Stream, the STGM flags in the bind context indicate the mode of access desired (read or read/write).</param>
    /// <param name="bhid">Reference to a GUID that specifies which handler will be created.</param>
    /// <param name="riid">IID of the object type to retrieve.</param>
    /// <param name="ppv">When this method returns, contains a pointer of type riid that is returned by the handler specified by rbhid.</param>
    /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [PreserveSig]
    new HRESULT BindToHandler(IntPtr pbc, [MarshalAs(UnmanagedType.LPStruct)] Guid bhid, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IntPtr ppv);

    /// <summary>
    /// Gets the parent of an IShellItem object.
    /// </summary>
    /// <returns>The parent of an IShellItem interface.</returns>
    new IShellItem GetParent();

    /// <summary>
    /// Gets the display name of the IShellItem object.
    /// </summary>
    /// <param name="sigdnName">One of the SIGDN values that indicates how the name should look.</param>
    /// <param name="ppszName">Retrieved display name</param>
    /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [PreserveSig]
    new HRESULT GetDisplayName(SIGDN sigdnName, [Out, MarshalAs(UnmanagedType.LPWStr)] out string? ppszName);

    /// <summary>
    /// Gets a requested set of attributes of the IShellItem object.
    /// </summary>
    /// <param name="sfgaoMask">Specifies the attributes to retrieve. One or more of the SFGAO values. Use a bitwise OR operator to determine the attributes to retrieve.</param>
    /// <returns>Value that, when this method returns successfully, contains the requested attributes. One or more of the SFGAO values. Only those attributes specified by sfgaoMask are returned; other attribute values are undefined.</returns>
    new uint GetAttributes(SFGAO sfgaoMask);

    /// <summary>
    /// Compares two IShellItem objects.
    /// </summary>
    /// <param name="psi">A pointer to an IShellItem object to compare with the existing IShellItem object.</param>
    /// <param name="hint">One of the SICHINTF values that determines how to perform the comparison. See SICHINTF for the list of possible values for this parameter.</param>
    /// <returns>The result of the comparison. If the two items are the same this parameter equals zero; if they are different the parameter is nonzero.</returns>
    new int Compare(IShellItem psi, SICHINT hint);
    #endregion

    /// <summary>
    /// Gets a property store object for specified property store flags.
    /// </summary>
    /// <param name="flags">The GETPROPERTYSTOREFLAGS constants that modify the property store object.</param>
    /// <param name="riid">A reference to the IID of the object to be retrieved.</param>
    /// <param name="propertyStore">When this method returns, contains the address of an IPropertyStore interface pointer.</param>
    /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [PreserveSig]
    HRESULT GetPropertyStore(GPS flags, [In] ref Guid riid, out IPropertyStore propertyStore);

    /// <summary>
    /// Uses the specified ICreateObject instead of CoCreateInstance to create an instance of the property handler associated with the Shell item on which this method is called. Most calling applications do not need to call this method, and can call IShellItem2::GetPropertyStore instead.
    /// </summary>
    /// <param name="flags">The GETPROPERTYSTOREFLAGS constants that modify the property store object.</param>
    /// <param name="punkCreateObject">A pointer to a factory for low-rights creation of type ICreateObject.
    /// The method CreateObject creates an instance of a COM object. The implementation of IShellItem2::GetPropertyStoreWithCreateObject uses CreateObject instead of CoCreateInstance to create the property handler, which is a Shell extension, for a given file type. The property handler provides many of the important properties in the property store that this method returns.
    /// This method is useful only if the ICreateObject object is created in a separate process (as a LOCALSERVER instead of an INPROCSERVER), and also if this other process has lower rights than the process calling IShellItem2::GetPropertyStoreWithCreateObject.</param>
    /// <param name="riid">A reference to the IID of the object to be retrieved.</param>
    /// <returns>When this method returns, contains the address of the requested IPropertyStore interface pointer.</returns>
    [return: MarshalAs(UnmanagedType.Interface)]
    object GetPropertyStoreWithCreateObject(
        GPS flags,
        [MarshalAs(UnmanagedType.IUnknown)] object punkCreateObject,   // factory for low-rights creation of type ICreateObject
        [In] ref Guid riid);

    /// <summary>
    /// Gets property store object for specified property keys.
    /// </summary>
    /// <param name="rgKeys">A pointer to an array of PROPERTYKEY structures. Each structure contains a unique identifier for each property used in creating the property store.</param>
    /// <param name="cKeys">The number of PROPERTYKEY structures in the array pointed to by rgKeys.</param>
    /// <param name="flags">The GETPROPERTYSTOREFLAGS constants that modify the property store object.</param>
    /// <param name="riid">A reference to the IID of the object to be retrieved.</param>
    /// <returns>When this method returns, contains the address of an IPropertyStore interface pointer.</returns>
    [return: MarshalAs(UnmanagedType.Interface)]
    object GetPropertyStoreForKeys(IntPtr rgKeys, uint cKeys, GPS flags, [In] ref Guid riid);

    /// <summary>
    /// Gets a property description list object given a reference to a property key.
    /// </summary>
    /// <param name="keyType">A reference to a PROPERTYKEY structure.</param>
    /// <param name="riid">A reference to a desired IID.</param>
    /// <returns>IPropertyDescriptionList interface pointer.</returns>
    [return: MarshalAs(UnmanagedType.Interface)]
    object GetPropertyDescriptionList(IntPtr keyType, [In] ref Guid riid);

    /// <summary>
    /// Ensures that any cached information in this item is updated.
    /// </summary>
    /// <param name="pbc">A pointer to an IBindCtx interface on a bind context object.</param>
    void Update(IBindCtx pbc);

    /// <summary>
    /// Gets a PROPVARIANT structure from a specified property key.
    /// </summary>
    /// <param name="key">A reference to a PROPERTYKEY structure.</param>
    /// <param name="pv">Contains a pointer to a PROPVARIANT structure.</param>
    /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [PreserveSig]
    HRESULT GetProperty(PropertyKey key, [In, Out] PropVariant pv);

    /// <summary>
    /// Gets the CLSID value of a specified property key.
    /// </summary>
    /// <param name="key">A reference to a PROPERTYKEY structure.</param>
    /// <returns>A CLSID value.</returns>
    Guid GetCLSID(PropertyKey key);

    /// <summary>
    /// Gets the FILETIME value of a specified property key.
    /// </summary>
    /// <param name="key">A reference to a PROPERTYKEY structure.</param>
    /// <returns>A FILETIME value.</returns>
    FILETIME GetFileTime(PropertyKey key);

    /// <summary>
    /// Gets the Int32 value of a specified property key.
    /// </summary>
    /// <param name="key">A reference to a PROPERTYKEY structure.</param>
    /// <returns>A Int32 value.</returns>
    int GetInt32(PropertyKey key);

    /// <summary>
    /// Gets the String value of a specified property key.
    /// </summary>
    /// <param name="key">A reference to a PROPERTYKEY structure.</param>
    /// <returns>A String value.</returns>
    [return: MarshalAs(UnmanagedType.LPWStr)]
    string GetString(PropertyKey key);

    /// <summary>
    /// Gets the UInt32 value of a specified property key.
    /// </summary>
    /// <param name="key">A reference to a PROPERTYKEY structure.</param>
    /// <returns>A UInt32 value.</returns>
    uint GetUInt32(PropertyKey key);

    /// <summary>
    /// Gets the UInt64 value of a specified property key.
    /// </summary>
    /// <param name="key">A reference to a PROPERTYKEY structure.</param>
    /// <returns>A UInt64 value.</returns>
    ulong GetUInt64(PropertyKey key);

    /// <summary>
    /// Gets the boolean value of a specified property key.
    /// </summary>
    /// <param name="key">A reference to a PROPERTYKEY structure.</param>
    /// <returns>A boolean value.</returns>
    [return: MarshalAs(UnmanagedType.Bool)]
    bool GetBool(PropertyKey key);
}