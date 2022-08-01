using System;
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
/// Exposes methods that enumerate and retrieve individual property description details.
/// </summary>
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Win32Consts.IID_IPropertyDescription)]
internal interface IPropertyDescription
{
    /// <summary>
    /// Gets a structure that acts as a property's unique identifier.
    /// </summary>
    /// <param name="pkey">When this method returns, contains a pointer to a PROPERTYKEY structure.</param>
    /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HRESULT GetPropertyKey(out PropertyKey pkey);

    /// <summary>
    /// Gets the case-sensitive name by which a property is known to the system, regardless of its localized name.
    /// </summary>
    /// <param name="ppszName">When this method returns, contains the address of a pointer to the property's canonical name as a null-terminated Unicode string.</param>
    /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HRESULT GetCanonicalName([MarshalAs(UnmanagedType.LPWStr)] out string ppszName);

    /// <summary>
    /// Gets the variant type of the property.
    /// </summary>
    /// <param name="pvartype">When this method returns, contains a pointer to a VARTYPE that indicates the property type. If the property is multi-valued, the value pointed to is a VT_VECTOR mask (VT_VECTOR ORed to the VARTYPE. </param>
    /// <returns>This method always returns S_OK.</returns>
    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HRESULT GetPropertyType(out VarEnum pvartype);

    /// <summary>
    /// Gets the display name of the property as it is shown in any UI.
    /// </summary>
    /// <param name="ppszName">Contains the address of a pointer to the property's name as a null-terminated Unicode string.</param>
    /// <returns>Returns one of the following values:S_OK, E_INVALIDARG, E_OUTOFMEMORY</returns>
    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HRESULT GetDisplayName(out IntPtr ppszName);

    /// <summary>
    /// Gets the text used in edit controls hosted in various dialog boxes.
    /// </summary>
    /// <param name="ppszInvite">When this method returns, contains the address of a pointer to a null-terminated Unicode buffer that holds the invitation text.</param>
    /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HRESULT GetEditInvitation(out IntPtr ppszInvite);

    /// <summary>
    /// Gets a set of flags that describe the uses and capabilities of the property.
    /// </summary>
    /// <param name="mask">A mask that specifies which type flags to retrieve. A combination of values found in the PROPDESC_TYPE_FLAGS constants. To retrieve all type flags, pass PDTF_MASK_ALL</param>
    /// <param name="ppdtFlags">When this method returns, contains a pointer to a value that consists of bitwise PROPDESC_TYPE_FLAGS values.</param>
    /// <returns>Always returns S_OK.</returns>
    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HRESULT GetTypeFlags([In] PropertyTypeOptions mask, out PropertyTypeOptions ppdtFlags);

    /// <summary>
    /// Gets the current set of flags governing the property's view.
    /// </summary>
    /// <param name="ppdvFlags">When this method returns, contains a pointer to a value that includes one or more of the following flags. See PROPDESC_VIEW_FLAGS for valid values.</param>
    /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HRESULT GetViewFlags(out PropertyViewOptions ppdvFlags);

    /// <summary>
    /// Gets the default column width of the property in a list view.
    /// </summary>
    /// <param name="pcxChars">A pointer to the column width value, in characters.</param>
    /// <returns>Always returns S_OK.</returns>
    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HRESULT GetDefaultColumnWidth(out uint pcxChars);

    /// <summary>
    /// Gets the current data type used to display the property.
    /// </summary>
    /// <param name="pdisplaytype">Contains a pointer to a value that indicates the display type. </param>
    /// <returns>Always returns S_OK.</returns>
    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HRESULT GetDisplayType(out PropertyDisplayType pdisplaytype);

    /// <summary>
    /// Gets the column state flag, which describes how the property should be treated by interfaces or APIs that use this flag.
    /// </summary>
    /// <param name="pcsFlags">When this method returns, contains a pointer to the column state flag. See SHCOLSTATE for valid values.</param>
    /// <returns>Always returns S_OK.</returns>
    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HRESULT GetColumnState(out PropertyColumnStateOptions pcsFlags);

    /// <summary>
    /// Gets the grouping method to be used when a view is grouped by a property, and retrieves the grouping type.
    /// </summary>
    /// <param name="pgr">Receives a pointer to a flag value that indicates the grouping type.</param>
    /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HRESULT GetGroupingRange(out PropertyGroupingRange pgr);

    /// <summary>
    /// Gets the relative description type for a property description.
    /// </summary>
    /// <param name="prdt">When this method returns, contains a pointer to the relative description type value. </param>
    /// <returns>Always returns S_OK.</returns>
    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HRESULT GetRelativeDescriptionType(out RelativeDescriptionType prdt);

    /// <summary>
    /// Compares two property values in the manner specified by the property description. Returns two display strings that describe how the two properties compare.
    /// </summary>
    /// <param name="propvar1">A reference to a PROPVARIANT structure that contains the type and value of the first property.</param>
    /// <param name="propvar2">A reference to a PROPVARIANT structure that contains the type and value of the second property.</param>
    /// <param name="ppszDesc1">When this method returns, contains the address of a pointer to the description string that compares the first property with the second property. The string is null-terminated.</param>
    /// <param name="ppszDesc2">When this method returns, contains the address of a pointer to the description string that compares the second property with the first property. The string is null-terminated.</param>
    /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HRESULT GetRelativeDescription([In] PropVariant propvar1, [In] PropVariant propvar2,
        [MarshalAs(UnmanagedType.LPWStr)] out string ppszDesc1,
        [MarshalAs(UnmanagedType.LPWStr)] out string ppszDesc2);

    /// <summary>
    /// Gets the current sort description flags for the property, which indicate the particular wordings of sort offerings.
    /// </summary>
    /// <param name="psd">When this method returns, contains a pointer to the value of one or more of the following flags that indicate the sort types available to the user. Note that the strings shown are English versions only. Localized strings are used for other locales.</param>
    /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HRESULT GetSortDescription(out PropertySortDescription psd);

    /// <summary>
    /// Gets the localized display string that describes the current sort order.
    /// </summary>
    /// <param name="fDescending">TRUE if ppszDescription should reference the string "Z on top"; FALSE to reference the string "A on top".</param>
    /// <param name="ppszDescription">When this method returns, contains the address of a pointer to the sort description as a null-terminated Unicode string.</param>
    /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HRESULT GetSortDescriptionLabel([In] bool fDescending, out IntPtr ppszDescription);

    /// <summary>
    /// Gets a value that describes how the property values are displayed when multiple items are selected in the UI.
    /// </summary>
    /// <param name="paggtype">When this method returns, contains a pointer to a value that indicates the aggregation type. See PROPDESC_AGGREGATION_TYPE.</param>
    /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HRESULT GetAggregationType(out PropertyAggregationType paggtype);

    /// <summary>
    /// Gets the condition type and default condition operation to use when displaying the property in the query builder UI. This influences the list of predicate conditions (for example, equals, less than, and contains) that are shown for this property.
    /// </summary>
    /// <param name="pcontype">A pointer to a value that indicates the condition type.</param>
    /// <param name="popDefault">When this method returns, contains a pointer to a value that indicates the default condition operation.</param>
    /// <returns>Always returns S_OK.</returns>
    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HRESULT GetConditionType(out PropertyConditionType pcontype, out PropertyConditionOperation popDefault);

    /// <summary>
    /// Gets an instance of an IPropertyEnumTypeList, which can be used to enumerate the possible values for a property.
    /// </summary>
    /// <param name="riid">A reference to the IID of the interface to retrieve through ppv, typically IID_IShellItem.</param>
    /// <param name="ppv">When this method returns successfully, contains the interface pointer requested in riid. This is typically IPropertyEnumTypeList.</param>
    /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HRESULT GetEnumTypeList([In] ref Guid riid, [Out, MarshalAs(UnmanagedType.Interface)] out IPropertyEnumTypeList? ppv);

    /// <summary>
    /// Coerces the value to the canonical value, according to the property description.
    /// </summary>
    /// <param name="propvar">On entry, contains a pointer to a PROPVARIANT structure that contains the original value. When this method returns, contains the canonical value.</param>
    /// <returns>If the failure code is not INPLACE_S_TRUNCATED or E_INVALIDARG, then coercion from the value's type to the property description's type was not possible, and the PROPVARIANT structure has been cleared.</returns>
    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HRESULT CoerceToCanonicalValue([In, Out] PropVariant propvar);

    /// <summary>
    /// Gets a formatted, Unicode string representation of a property value.
    /// </summary>
    /// <param name="propvar">A reference to a PROPVARIANT structure that contains the type and value of the property.</param>
    /// <param name="pdfFlags">One or more of the PROPDESC_FORMAT_FLAGS flags, which are either bitwise or multiple values, that indicate the property string format.</param>
    /// <param name="ppszDisplay">The address of a pointer to a null-terminated Unicode string that contains the display text.</param>
    /// <returns></returns>
    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HRESULT FormatForDisplay([In] PropVariant propvar, [In] ref PropertyDescriptionFormatOptions pdfFlags, [MarshalAs(UnmanagedType.LPWStr)] out string ppszDisplay);

    /// <summary>
    /// Gets a value that indicates whether a property is canonical according to the definition of the property description.
    /// </summary>
    /// <param name="propvar">A reference to a PROPVARIANT structure that contains the type and value of the property.</param>
    /// <returns>Returns one of the following values: S_OK (The value is canonical), S_FALSE (The value is not canonical.)</returns>
    [PreserveSig]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    HRESULT IsValueCanonical([In] PropVariant propvar);
}