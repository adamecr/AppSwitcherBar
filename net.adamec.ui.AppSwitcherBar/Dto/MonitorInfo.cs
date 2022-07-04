using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using net.adamec.ui.AppSwitcherBar.Win32;

namespace net.adamec.ui.AppSwitcherBar.Dto
{
    /// <summary>
    /// Information about the monitor (display)
    /// </summary>
    public class MonitorInfo : IEquatable<MonitorInfo>
    {
        /// <summary>
        /// A <see cref="Rect"/> structure that specifies the display monitor rectangle, expressed in virtual-screen coordinates.
        /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
        /// </summary>
        public Rect ViewportBounds { get; }
        /// <summary>
        /// A <see cref="Rect"/> structure that specifies the work area rectangle of the display monitor, expressed in virtual-screen coordinates.
        /// Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
        /// </summary>
        public Rect WorkAreaBounds { get; }
        /// <summary>
        /// Flag whether the monitor is the primary display
        /// </summary>
        public bool IsPrimary { get; }
        /// <summary>
        /// A string that specifies the device name of the monitor being used
        /// </summary>
        public string DeviceId { get; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="mex"><see cref="MONITORINFOEX"/> structure returned from <see cref="User32.EnumDisplayMonitors(IntPtr, IntPtr, MonitorEnumDelegate, IntPtr)"/> via <see cref="MonitorEnumDelegate"/></param>
        internal MonitorInfo(MONITORINFOEX mex)
        {
            ViewportBounds = (Rect)mex.rcMonitor;
            WorkAreaBounds = (Rect)mex.rcWork;
            IsPrimary = mex.dwFlags.HasFlag(MONITORINFOF.PRIMARY);
            DeviceId = mex.szDevice;
        }

        /// <summary>
        /// Retrieves information about all monitors (displays) present in the system
        /// </summary>
        /// <returns>All monitors (displays) present in the system</returns>
        /// <exception cref="Win32Exception">Thrown when the call of <see cref="User32.GetMonitorInfo(IntPtr, ref MONITORINFOEX)"/> fails</exception>
        public static IEnumerable<MonitorInfo> GetAllMonitors()
        {
            var monitors = new List<MonitorInfo>();
            
            //enum monitors
            MonitorEnumDelegate callback = delegate (IntPtr hMonitor, IntPtr _, ref RECT _, IntPtr _)
            {
                var mi = new MONITORINFOEX
                {
                    cbSize = Marshal.SizeOf(typeof(MONITORINFOEX))
                };

                if (!User32.GetMonitorInfo(hMonitor, ref mi)) throw new Win32Exception();

                monitors.Add(new MonitorInfo(mi));
                return true;
            };

            User32.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, callback, IntPtr.Zero);

            return monitors;
        }

        /// <summary>
        /// Returns the string representation of the object
        /// </summary>
        /// <returns>String representation of the object</returns>
        public override string ToString() => DeviceId;

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return Equals(obj as MonitorInfo);
        }
        /// <inheritdoc/>
        public override int GetHashCode() => DeviceId.GetHashCode();
        /// <inheritdoc/>
        public bool Equals(MonitorInfo? other) => DeviceId == other?.DeviceId;
       
        public static bool operator ==(MonitorInfo? a, MonitorInfo? b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            return a?.Equals(b)??false;
        }
        public static bool operator !=(MonitorInfo? a, MonitorInfo? b) => !(a == b);
    }
}
