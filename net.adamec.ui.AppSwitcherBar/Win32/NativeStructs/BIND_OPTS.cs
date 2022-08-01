using System.Runtime.InteropServices;
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;

/// <summary>
/// Contains parameters used during a moniker-binding operation.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct BIND_OPTS
{
    /// <summary>
    /// The size of this structure, in bytes.
    /// </summary>
    public int cbStruct;
    /// <summary>
    /// Flags that control aspects of moniker binding operations. This value is any combination of the bit flags in the BIND_FLAGS enumeration. The CreateBindCtx function initializes this member to zero.
    /// </summary>
    public int grfFlags;
    /// <summary>
    /// Flags that should be used when opening the file that contains the object identified by the moniker. Possible values are the STGM constants. The binding operation uses these flags in the call to IPersistFile::Load when loading the file. If the object is already running, these flags are ignored by the binding operation. The CreateBindCtx function initializes this field to STGM_READWRITE.
    /// </summary>
    public int grfMode;
    /// <summary>
    /// The clock time by which the caller would like the binding operation to be completed, in milliseconds. This member lets the caller limit the execution time of an operation when speed is of primary importance. A value of zero indicates that there is no deadline
    /// </summary>
    public int dwTickCountDeadline;
}