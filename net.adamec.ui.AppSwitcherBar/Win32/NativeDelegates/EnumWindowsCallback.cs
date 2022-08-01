using net.adamec.ui.AppSwitcherBar.Win32.NativeMethods;
using System;

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeDelegates
{
    /// <summary>
    /// An application-defined callback function used with the <see cref="User32.EnumWindows(EnumWindowsCallback, int)"/> function. It receives top-level window handles.
    /// </summary>
    /// <param name="hwnd">A handle to a top-level window.</param>
    /// <param name="lParam">The application-defined value given in <see cref="User32.EnumWindows(EnumWindowsCallback, int)"/> </param>
    /// <returns>To continue enumeration, the callback function must return TRUE; to stop enumeration, it must return FALSE.</returns>
    internal delegate bool EnumWindowsCallback(IntPtr hwnd, int lParam);
}
