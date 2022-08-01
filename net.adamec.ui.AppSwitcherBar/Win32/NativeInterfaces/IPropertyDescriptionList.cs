using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;

// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
namespace net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces;

/// <summary>
/// Exposes methods that extract information from a collection of property descriptions presented as a list.
/// </summary>
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Win32Consts.IID_IPropertyDescriptionList)]
internal interface IPropertyDescriptionList
{
    /// <summary>
    /// Gets the number of properties included in the property list.
    /// </summary>
    /// <param name="pcElem">When this method returns, contains a pointer to the count of properties.</param>
    /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HRESULT GetCount(out uint pcElem);

    /// <summary>
    /// Gets the property description at the specified index in a property description list.
    /// </summary>
    /// <param name="iElem">The number of the property in the list string.</param>
    /// <param name="riid">A reference to the IID of the requested property description interface, typically IID_IPropertyDescription.</param>
    /// <param name="ppv">When this method returns, contains the interface pointer requested in riid. Typically, this is IPropertyDescription.</param>
    /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HRESULT GetAt([In] uint iElem, [In] ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out IPropertyDescription ppv);
}