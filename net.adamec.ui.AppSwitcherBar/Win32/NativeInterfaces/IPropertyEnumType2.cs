using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
using net.adamec.ui.AppSwitcherBar.Win32.Services.Shell.Properties;
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
namespace net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces;

/// <summary>
/// Exposes methods that extract data from enumeration information. IPropertyEnumType2 extends IPropertyEnumType.
/// </summary>
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Win32Consts.IID_IPropertyEnumType2)]
internal interface IPropertyEnumType2 : IPropertyEnumType
{
    /// <summary>
    /// Gets an enumeration type from an enumeration information structure.
    /// </summary>
    /// <param name="penumtype">When this method returns, contains a pointer to one of the values that indicate the enumeration type.</param>
    /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new HRESULT GetEnumType([Out] out PropEnumType penumtype);

    /// <summary>
    /// Gets a value from an enumeration information structure.
    /// </summary>
    /// <param name="ppropvar">When this method returns, contains a pointer to the property value.</param>
    /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new HRESULT GetValue([Out] PropVariant ppropvar);

    /// <summary>
    /// Gets a minimum value from an enumeration information structure.
    /// </summary>
    /// <param name="ppropvar">When this method returns, contains a pointer to the minimum value.</param>
    /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new HRESULT GetRangeMinValue([Out] PropVariant ppropvar);

    /// <summary>
    /// Gets a set value from an enumeration information structure.
    /// </summary>
    /// <param name="ppropvar">When this method returns, contains a pointer to the set value.</param>
    /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new HRESULT GetRangeSetValue([Out] PropVariant ppropvar);

    /// <summary>
    /// Gets display text from an enumeration information structure.
    /// </summary>
    /// <param name="ppszDisplay">When this method returns, contains the address of a pointer to a null-terminated Unicode string that contains the display text.</param>
    /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new HRESULT GetDisplayText([Out, MarshalAs(UnmanagedType.LPWStr)] out string ppszDisplay);


    /// <summary>
    /// Retrieves the image reference associated with a property enumeration.
    /// </summary>
    /// <param name="ppszImageRes">A pointer to a buffer that, when this method returns successfully, receives a string of the form dll name,-resid that is suitable to be passed to PathParseIconLocation.</param>
    /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HRESULT GetImageReference([Out, MarshalAs(UnmanagedType.LPWStr)] out string ppszImageRes);


}