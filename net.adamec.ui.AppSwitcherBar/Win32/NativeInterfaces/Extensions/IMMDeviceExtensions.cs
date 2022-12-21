using net.adamec.ui.AppSwitcherBar.Win32.Services;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces.Extensions
{
    /// <summary>
    /// Extensions for <see cref="IMMDevice"/>
    /// </summary>
    internal static class IMMDeviceExtensions
    {
        /// <summary>
        /// Gets the device ID
        /// </summary>
        /// <param name="device">Device to get the ID from</param>
        /// <returns>ID of device or null if it can't be retrieved</returns>
        public static string? Id(this IMMDevice device)
        {
            return device.GetId(out var id).IsSuccess ? id : null;
        }
    }
}
