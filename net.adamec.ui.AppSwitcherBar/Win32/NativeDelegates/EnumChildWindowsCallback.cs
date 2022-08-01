using net.adamec.ui.AppSwitcherBar.Win32.NativeMethods;
using System;

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeDelegates;

/// <summary>
/// An application-defined callback function used with the <see cref="User32.EnumChildWindows"/> function. It receives child window handles.
/// </summary>
/// <param name="hwnd">A handle to a child window.</param>
/// <param name="lParam">The application-defined value given in <see cref="User32.EnumChildWindows"/> </param>
/// <returns>To continue enumeration, the callback function must return TRUE; to stop enumeration, it must return FALSE.</returns>
internal delegate bool EnumChildWindowsCallback(IntPtr hwnd, IntPtr lParam);