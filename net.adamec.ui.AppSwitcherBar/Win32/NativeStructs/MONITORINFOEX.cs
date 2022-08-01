using System.Runtime.InteropServices;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;

/// <summary>
/// The MONITORINFOEX structure contains information about a display monitor.
/// </summary>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct MONITORINFOEX
{
    /// <summary>
    /// The size of the structure, in bytes.
    /// </summary>
    public int cbSize;
    /// <summary>
    /// A <see cref="RECT"/> structure that specifies the display monitor rectangle, expressed in virtual-screen coordinates. Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
    /// </summary>
    public RECT rcMonitor;
    /// <summary>
    /// A <see cref="RECT"/> structure that specifies the work area rectangle of the display monitor, expressed in virtual-screen coordinates. Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
    /// </summary>
    public RECT rcWork;
    /// <summary>
    /// A set of flags that represent attributes of the display monitor.
    /// </summary>
    public MONITORINFOF dwFlags;
    /// <summary>
    /// A string that specifies the device name of the monitor being used. 
    /// </summary>
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Win32Consts.CCHDEVICENAME)]
    public string szDevice;
}