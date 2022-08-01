using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
using net.adamec.ui.AppSwitcherBar.Win32.Services.Shell.Properties;

// ReSharper disable BuiltInTypeReferenceStyle
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable UnusedMember.Global

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeMethods
{
    /// <summary>
    /// PropSys Win32 Api
    /// </summary>
    internal class PropSys
    {
        /// <summary>
        /// Name of the DLL
        /// </summary>
        private const string DLL_NAME = "propsys.dll";

        /// <summary>
        /// Retrieves the element count of a PROPVARIANT structure.
        /// </summary>
        /// <param name="propVar">Reference to the source PROPVARIANT structure.</param>
        /// <returns>Returns the element count of a VT_VECTOR or VT_ARRAY value: for single values, returns 1; for empty structures, returns 0.</returns>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        internal static extern int PropVariantGetElementCount([In] PropVariant propVar);

        /// <summary>
        /// Extracts a single Boolean element from a PROPVARIANT structure of type VT_BOOL, VT_VECTOR | VT_BOOL, or VT_ARRAY | VT_BOOL.
        /// </summary>
        /// <param name="propVar">A reference to the source PROPVARIANT structure.</param>
        /// <param name="iElem">Specifies the vector or array index; otherwise, iElem must be 0.</param>
        /// <param name="pfVal">When this function returns, contains the extracted Boolean value.</param>
        /// <returns>If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern HRESULT PropVariantGetBooleanElem([In] PropVariant propVar, [In] uint iElem, [Out, MarshalAs(UnmanagedType.Bool)] out bool pfVal);

        /// <summary>
        /// Extracts a single Int16 element from a PROPVARIANT structure of type  VT_I2, VT_VECTOR | VT_I2, or VT_ARRAY | VT_I2.
        /// </summary>
        /// <param name="propVar">A reference to the source PROPVARIANT structure.</param>
        /// <param name="iElem">Specifies the vector or array index; otherwise, iElem must be 0.</param>
        /// <param name="pnVal">When this function returns, contains the extracted Int16 value.</param>
        /// <returns>If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern HRESULT PropVariantGetInt16Elem([In] PropVariant propVar, [In] uint iElem, [Out] out short pnVal);

        /// <summary>
        /// Extracts a single UInt16 element from a PROPVARIANT structure of type VT_U12, VT_VECTOR | VT_U12, or VT_ARRAY | VT_U12.
        /// </summary>
        /// <param name="propVar">A reference to the source PROPVARIANT structure.</param>
        /// <param name="iElem">Specifies the vector or array index; otherwise, iElem must be 0.</param>
        /// <param name="pnVal">When this function returns, contains the extracted UInt16 value.</param>
        /// <returns>If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern HRESULT PropVariantGetUInt16Elem([In] PropVariant propVar, [In] uint iElem, [Out] out ushort pnVal);

        /// <summary>
        /// Extracts a single Int32 element from a PROPVARIANT structure of type VT_I4, VT_VECTOR | VT_I4, or VT_ARRAY | VT_I4.
        /// </summary>
        /// <param name="propVar">A reference to the source PROPVARIANT structure.</param>
        /// <param name="iElem">Specifies the vector or array index; otherwise, iElem must be 0.</param>
        /// <param name="pnVal">When this function returns, contains the extracted Int32 value.</param>
        /// <returns>If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern HRESULT PropVariantGetInt32Elem([In] PropVariant propVar, [In] uint iElem, [Out] out int pnVal);

        /// <summary>
        /// Extracts a single UInt32 element from a PROPVARIANT structure of type VT_UI4, VT_VECTOR | VT_UI4, or VT_ARRAY | VT_UI4.
        /// </summary>
        /// <param name="propVar">A reference to the source PROPVARIANT structure.</param>
        /// <param name="iElem">Specifies the vector or array index; otherwise, iElem must be 0.</param>
        /// <param name="pnVal">When this function returns, contains the extracted UInt32 value.</param>
        /// <returns>If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern HRESULT PropVariantGetUInt32Elem([In] PropVariant propVar, [In] uint iElem, [Out] out uint pnVal);

        /// <summary>
        /// Extracts a single Int64 element from a PROPVARIANT structure of type VT_I8, VT_VECTOR | VT_I8, or VT_ARRAY | VT_I8.
        /// </summary>
        /// <param name="propVar">A reference to the source PROPVARIANT structure.</param>
        /// <param name="iElem">Specifies the vector or array index; otherwise, iElem must be 0.</param>
        /// <param name="pnVal">When this function returns, contains the extracted Int64 value.</param>
        /// <returns>If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern HRESULT PropVariantGetInt64Elem([In] PropVariant propVar, [In] uint iElem, [Out] out Int64 pnVal);

        /// <summary>
        /// Extracts a single UInt64 element from a PROPVARIANT structure of type VT_UI8, VT_VECTOR | VT_UI8, or VT_ARRAY | VT_UI8.
        /// </summary>
        /// <param name="propVar">A reference to the source PROPVARIANT structure.</param>
        /// <param name="iElem">Specifies the vector or array index; otherwise, iElem must be 0.</param>
        /// <param name="pnVal">When this function returns, contains the extracted UInt64 value.</param>
        /// <returns>If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern HRESULT PropVariantGetUInt64Elem([In] PropVariant propVar, [In] uint iElem, [Out] out UInt64 pnVal);

        /// <summary>
        /// Extracts a single Double element from a PROPVARIANT structure of type VT_R8, VT_VECTOR | VT_R8, or VT_ARRAY | VT_R8.
        /// </summary>
        /// <param name="propVar">A reference to the source PROPVARIANT structure.</param>
        /// <param name="iElem">Specifies the vector or array index; otherwise, iElem must be 0.</param>
        /// <param name="pnVal">When this function returns, contains the extracted Double value.</param>
        /// <returns>If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern HRESULT PropVariantGetDoubleElem([In] PropVariant propVar, [In] uint iElem, [Out] out double pnVal);

        /// <summary>
        /// Extracts a single FILETIME element from a PROPVARIANT structure of type VT_FILETIME, VT_VECTOR | VT_FILETIME, or VT_ARRAY | VT_FILETIME.
        /// </summary>
        /// <param name="propVar">A reference to the source PROPVARIANT structure.</param>
        /// <param name="iElem">Specifies the vector or array index; otherwise, iElem must be 0.</param>
        /// <param name="pftVal">When this function returns, contains the extracted FILETIME value.</param>
        /// <returns>If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern HRESULT PropVariantGetFileTimeElem([In] PropVariant propVar, [In] uint iElem, [Out, MarshalAs(UnmanagedType.Struct)] out FILETIME pftVal);

        /// <summary>
        /// Extracts a single string element from a PROPVARIANT structure of type VT_LPWSTR, VT_BSTR, VT_VECTOR | VT_LPWSTR, VT_VECTOR | VT_BSTR, or VT_ARRAY | VT_BSTR.
        /// </summary>
        /// <param name="propVar">A reference to the source PROPVARIANT structure.</param>
        /// <param name="iElem">Specifies the vector or array index; otherwise, iElem must be 0.</param>
        /// <param name="ppszVal">When this function returns, contains the extracted string value.</param>
        /// <returns>If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern HRESULT PropVariantGetStringElem([In] PropVariant propVar, [In] uint iElem, [MarshalAs(UnmanagedType.LPWStr)] ref string ppszVal);


        /// <summary>
        /// Retrieves the canonical name of the property, given its PROPERTYKEY.
        /// </summary>
        /// <param name="propkey">Reference to a PROPERTYKEY structure that identifies the requested property.</param>
        /// <param name="ppszCanonicalName">When this function returns, contains a pointer to the property name as a null-terminated Unicode string.</param>
        /// <returns>Returns one of the following values.
        /// S_OK - The property's canonical name is obtained.
        /// TYPE_E_ELEMENTNOTFOUND - Indicates that the PROPERTYKEY does not exist in the schema subsystem cache.</returns>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern HRESULT PSGetNameFromPropertyKey(ref PropertyKey propkey, [Out, MarshalAs(UnmanagedType.LPWStr)] out string ppszCanonicalName);

        /// <summary>
        /// Gets an instance of a property description interface for a property specified by a PROPERTYKEY structure.
        /// </summary>
        /// <param name="propkey">Reference to a PROPERTYKEY structure that identifies the requested property.</param>
        /// <param name="riid">Reference to the interface ID of the requested interface.</param>
        /// <param name="ppv">When this function returns, contains the interface pointer requested in riid.
        /// This is typically IPropertyDescription, IPropertyDescriptionAliasInfo, or IPropertyDescriptionSearchInfo.</param>
        /// <returns>Returns one of the following values.
        /// S_OK -The interface was obtained.
        /// E_INVALIDARG - The ppv parameter is NULL.
        /// TYPE_E_ELEMENTNOTFOUND - The PROPERTYKEY does not exist in the schema subsystem cache.</returns>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern HRESULT PSGetPropertyDescription(ref PropertyKey propkey, ref Guid riid, [Out, MarshalAs(UnmanagedType.Interface)] out IPropertyDescription ppv);

    }
}
