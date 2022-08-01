using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
using net.adamec.ui.AppSwitcherBar.Win32.Services.Shell.Properties;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces
{
    /// <summary>
    /// IPropertyStore interface
    /// </summary>
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid(Win32Consts.IID_IPropertyStore)]
    internal interface IPropertyStore
    {
        /// <summary>
        /// Gets the number of properties contained in the property store.
        /// </summary>
        /// <param name="cProps">A pointer to a value that indicates the property count.</param>
        /// <returns>Method call result</returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetCount([Out] out uint cProps);

        /// <summary>
        /// Get a property key located at a specific index.
        /// </summary>
        /// <param name="iProp">The index of the property key in the array of PROPERTYKEY structures. This is a zero-based index.</param>
        /// <param name="pkey">Property key located at a specific index</param>
        /// <returns>Method call result</returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetAt([In] uint iProp, out PropertyKey pkey);

        /// <summary>
        /// Gets the value of a property from the store
        /// </summary>
        /// <param name="key">Property key of the property</param>
        /// <param name="pv"><see cref="PropVariant"/> structure that contains data about the property.</param>
        /// <returns>Method call result</returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetValue([In] ref PropertyKey key, [Out] PropVariant pv);

        /// <summary>
        /// Sets the value of a property in the store
        /// </summary>
        /// <param name="key">Property key of the property</param>
        /// <param name="pv"><see cref="PropVariant"/> structure that contains data about the property</param>
        /// <returns>Method call result</returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT SetValue([In] ref PropertyKey key, [In] PropVariant pv);

        /// <summary>
        /// Commits the changes.
        /// </summary>
        /// <returns>Method call result</returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT Commit();
    }


}
