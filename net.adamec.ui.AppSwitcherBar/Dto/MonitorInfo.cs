using System;
using System.Windows;

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
        /// <param name="viewportBounds"> A <see cref="Rect"/> structure that specifies the display monitor rectangle, expressed in virtual-screen coordinates.</param>
        /// <param name="workAreaBounds">A <see cref="Rect"/> structure that specifies the work area rectangle of the display monitor, expressed in virtual-screen coordinates.</param>
        /// <param name="isPrimary">Flag whether the monitor is the primary display</param>
        /// <param name="deviceId">A string that specifies the device name of the monitor being used</param>
        internal MonitorInfo(Rect viewportBounds, Rect workAreaBounds, bool isPrimary, string deviceId)
        {
            ViewportBounds = viewportBounds;
            WorkAreaBounds = workAreaBounds;
            IsPrimary = isPrimary;
            DeviceId = deviceId;
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

            return a?.Equals(b) ?? false;
        }
        public static bool operator !=(MonitorInfo? a, MonitorInfo? b) => !(a == b);
    }
}
