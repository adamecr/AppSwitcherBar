using System;
using System.Runtime.InteropServices;
using System.Text;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;

// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces;

/// <summary>
/// Exposes methods that retrieve information about a Shell item. IShellItem and IShellItem2 are the preferred representations of items in any new code.
/// </summary>
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid(Win32Consts.IID_IShellItem)]
internal interface IShellItem
{
    /// <summary>
    /// Binds to a handler for an item as specified by the handler ID value (BHID).
    /// </summary>
    /// <param name="pbc">A pointer to an IBindCtx interface on a bind context object. Used to pass optional parameters to the handler. The contents of the bind context are handler-specific. For example, when binding to BHID_Stream, the STGM flags in the bind context indicate the mode of access desired (read or read/write).</param>
    /// <param name="bhid">Reference to a GUID that specifies which handler will be created.</param>
    /// <param name="riid">IID of the object type to retrieve.</param>
    /// <param name="ppv">When this method returns, contains a pointer of type riid that is returned by the handler specified by rbhid.</param>
    /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [PreserveSig]
    HRESULT BindToHandler(IntPtr pbc, [MarshalAs(UnmanagedType.LPStruct)] Guid bhid, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IntPtr ppv);

    /// <summary>
    /// Gets the parent of an IShellItem object.
    /// </summary>
    /// <returns>The parent of an IShellItem interface.</returns>
    IShellItem GetParent();

    /// <summary>
    /// Gets the display name of the IShellItem object.
    /// </summary>
    /// <param name="sigdnName">One of the SIGDN values that indicates how the name should look.</param>
    /// <param name="ppszName">Retrieved display name</param>
    /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
    [PreserveSig]
    HRESULT GetDisplayName(SIGDN sigdnName, [Out, MarshalAs(UnmanagedType.LPWStr)] out string? ppszName);

    /// <summary>
    /// Gets a requested set of attributes of the IShellItem object.
    /// </summary>
    /// <param name="sfgaoMask">Specifies the attributes to retrieve. One or more of the SFGAO values. Use a bitwise OR operator to determine the attributes to retrieve.</param>
    /// <returns>Value that, when this method returns successfully, contains the requested attributes. One or more of the SFGAO values. Only those attributes specified by sfgaoMask are returned; other attribute values are undefined.</returns>
    uint GetAttributes(SFGAO sfgaoMask);

    /// <summary>
    /// Compares two IShellItem objects.
    /// </summary>
    /// <param name="psi">A pointer to an IShellItem object to compare with the existing IShellItem object.</param>
    /// <param name="hint">One of the SICHINTF values that determines how to perform the comparison. See SICHINTF for the list of possible values for this parameter.</param>
    /// <returns>The result of the comparison. If the two items are the same this parameter equals zero; if they are different the parameter is nonzero.</returns>
    int Compare(IShellItem psi, SICHINT hint);
}