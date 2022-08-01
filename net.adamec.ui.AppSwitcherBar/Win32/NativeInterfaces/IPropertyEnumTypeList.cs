using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
using net.adamec.ui.AppSwitcherBar.Win32.Services.Shell.Properties;

// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces;

/// <summary>
/// Exposes methods that enumerate the possible values for a property.
/// </summary>
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Win32Consts.IID_IPropertyEnumTypeList)]
internal interface IPropertyEnumTypeList
{
    /// <summary>
    /// Gets the number of elements in the list.
    /// </summary>
    /// <param name="pctypes">When this method returns, contains a pointer to the number of list elements.</param>
    /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HRESULT GetCount([Out] out uint pctypes);

    /// <summary>
    /// Gets the IPropertyEnumType object at the specified index in the list.
    /// </summary>
    /// <param name="itype">The index of the object in the list.</param>
    /// <param name="riid">A reference to the IID of the interface to retrieve through ppv, typically IID_IShellItem.</param>
    /// <param name="ppv">When this method returns successfully, contains the interface pointer requested in riid. This is typically IPropertyEnumType.</param>
    /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HRESULT GetAt([In] uint itype, [In] ref Guid riid, [Out, MarshalAs(UnmanagedType.Interface)] out IPropertyEnumType ppv);

    /// <summary>
    /// Not supported.
    /// Gets the condition at the specified index.
    /// </summary>
    /// <param name="index">Index of the desired condition.</param>
    /// <param name="riid">A reference to the IID of the interface to retrieve.</param>
    /// <param name="ppv">When this method returns, contains the address of an ICondition interface pointer.</param>
    /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HRESULT GetConditionAt([In] uint index, [In] ref Guid riid, out IntPtr ppv);

    /// <summary>
    /// Compares the specified property value against the enumerated values in a list and returns the matching index.
    /// </summary>
    /// <param name="propvarCmp">A reference to a PROPVARIANT structure that represents the property value.</param>
    /// <param name="pnIndex">When this method returns, contains a pointer to the index in the enumerated type list that matches the property value, if any.</param>
    /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HRESULT FindMatchingIndex([In] PropVariant propvarCmp, [Out] out uint pnIndex);
}