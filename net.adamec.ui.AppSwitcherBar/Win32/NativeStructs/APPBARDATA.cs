using System;
using System.Runtime.InteropServices;
using net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;

/// <summary>
/// Contains information about a system appbar message.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct APPBARDATA
{
    /// <summary>
    /// The size of the structure, in bytes.
    /// </summary>
    public int cbSize;
    /// <summary>
    /// The handle to the appbar window. Not all messages use this member.
    /// </summary>
    public IntPtr hWnd;
    /// <summary>
    /// An application-defined message identifier. The application uses the specified identifier for notification messages that it sends to the appbar identified by the hWnd member. This member is used when sending the <see cref="ABMsg.NEW"/> message.
    /// </summary>
    public int uCallbackMessage;
    /// <summary>
    /// A value that specifies an edge of the screen.
    /// </summary>
    public ABEdge uEdge;
    /// <summary>
    /// The bounding rectangle, in screen coordinates, of an appbar or the Windows taskbar
    /// </summary>
    public RECT rc;
    /// <summary>
    /// A message-dependent value
    /// </summary>
    public IntPtr lParam;
}