using System;

namespace net.adamec.ui.AppSwitcherBar.Win32
{
    /// <summary>
    /// An application-defined callback function used with the <see cref="User32.EnumWindows(EnumWindowsCallback, int)"/> function. It receives top-level window handles.
    /// </summary>
    /// <param name="hwnd">A handle to a top-level window.</param>
    /// <param name="lParam">The application-defined value given in <see cref="User32.EnumWindows(EnumWindowsCallback, int)"/> </param>
    /// <returns>To continue enumeration, the callback function must return TRUE; to stop enumeration, it must return FALSE.</returns>
    internal delegate bool EnumWindowsCallback(IntPtr hwnd, int lParam);

    /// <summary>
    /// An application-defined callback function that is called by the EnumDisplayMonitors function.
    /// </summary>
    /// <param name="hMonitor">A handle to the display monitor. This value will always be non-NULL.</param>
    /// <param name="hdcMonitor">A handle to a device context.</param>
    /// <param name="lprcMonitor">If hdcMonitor is non-NULL, this rectangle is the intersection of the clipping area of the device context identified by hdcMonitor and the display monitor rectangle. The rectangle coordinates are device-context coordinates.
    /// If hdcMonitor is NULL, this rectangle is the display monitor rectangle. The rectangle coordinates are virtual-screen coordinate</param>
    /// <param name="dwData">Application-defined data that EnumDisplayMonitors passes directly to the enumeration function.</param>
    /// <returns>To continue the enumeration, return TRUE. To stop the enumeration, return FALSE.</returns>
    internal delegate bool MonitorEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);
}
