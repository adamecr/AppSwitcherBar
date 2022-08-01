using System.Runtime.InteropServices;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
namespace net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces
{
    /// <summary>
    /// Exposes enumeration of IShellItem interfaces. This interface is typically obtained by calling the IEnumShellItems method.
    /// </summary>
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid(Win32Consts.IID_IEnumShellItems)]
    internal interface IEnumShellItems
    {
        /// <summary>
        /// Gets an array of one or more IShellItem interfaces from the enumeration.
        /// </summary>
        /// <param name="celt">The number of elements in the array referenced by the rgelt parameter.</param>
        /// <param name="rgelt">The address of an array of pointers to IShellItem interfaces that receive the enumerated item or items. The calling application is responsible for freeing the IShellItem interfaces by calling the IUnknown::Release method.</param>
        /// <param name="pceltFetched">A pointer to a value that receives the number of IShellItem interfaces successfully retrieved. The count can be smaller than the value specified in the celt parameter.
        /// This parameter can be NULL on entry only if celt is one, because in that case the method can only retrieve one item and return S_OK, or zero items and return S_FALSE.</param>
        /// <returns>This method can return one of these values: S_OK, S_FALSE</returns>
        [PreserveSig]
        HRESULT Next(uint celt, out IShellItem rgelt, out uint pceltFetched);

        /// <summary>
        /// Skips a given number of IShellItem interfaces in the enumeration. Used when retrieving interfaces.
        /// </summary>
        /// <param name="celt">The number of IShellItem interfaces to skip.</param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [PreserveSig]
        HRESULT Skip(uint celt);

        /// <summary>
        /// Resets the internal count of retrieved IShellItem interfaces in the enumeration.
        /// </summary>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [PreserveSig]
        HRESULT Reset();

        /// <summary>
        /// Gets a copy of the current enumeration.
        /// </summary>
        /// <param name="ppenum">The address of a pointer that receives a copy of this enumeration.</param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [PreserveSig]
        HRESULT Clone(out IEnumShellItems ppenum);
    }
}
