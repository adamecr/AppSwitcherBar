using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using net.adamec.ui.AppSwitcherBar.Dto;
using net.adamec.ui.AppSwitcherBar.Win32.NativeDelegates;
using net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;
using net.adamec.ui.AppSwitcherBar.Win32.NativeMethods;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;

namespace net.adamec.ui.AppSwitcherBar.Win32.Services
{
    /// <summary>
    /// Encapsulates the monitor (display) related functionality
    /// </summary>
    internal static class Monitor
    {
        /// <summary>
        /// Retrieves information about all monitors (displays) present in the system
        /// </summary>
        /// <returns>All monitors (displays) present in the system</returns>
        public static MonitorInfo[] GetAllMonitors()
        {
            var monitors = new List<MonitorInfo>();

            //enum monitors
            // ReSharper disable once ConvertToLocalFunction
            MonitorEnumDelegate callback = delegate (IntPtr hMonitor, IntPtr _, ref RECT _, IntPtr _)
            {
                var mi = new MONITORINFOEX
                {
                    cbSize = Marshal.SizeOf(typeof(MONITORINFOEX))
                };

                if (User32.GetMonitorInfo(hMonitor, ref mi))
                {
                    monitors.Add(new MonitorInfo((Rect)mi.rcMonitor, (Rect)mi.rcWork, mi.dwFlags.HasFlag(MONITORINFOF.PRIMARY), mi.szDevice));
                }

                return true;
            };

            User32.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, callback, IntPtr.Zero);

            return monitors.ToArray();
        }
    }
}
