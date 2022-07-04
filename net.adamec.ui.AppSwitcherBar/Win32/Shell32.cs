using System;
using System.Runtime.InteropServices;

namespace net.adamec.ui.AppSwitcherBar.Win32
{
    /// <summary>
    /// Shell32 Win32 Api
    /// </summary>
    internal class Shell32
    {
        /// <summary>
        /// Name of the DLL
        /// </summary>
        private const string DLL_NAME = "Shell32.dll";

        /// <summary>
        /// Sends an appbar message to the system.
        /// </summary>
        /// <param name="dwMessage">Appbar message value to send. This parameter can be one of the values from <see cref="ABMsg"/>.</param>
        /// <param name="pData">A pointer to an <see cref="APPBARDATA"/> structure. The content of the structure on entry and on exit depends on the value set in the <paramref name="dwMessage"/> parameter</param>
        /// <returns>This function returns a message-dependent value</returns>
        [DllImport(DLL_NAME, ExactSpelling = true)]
        public static extern uint SHAppBarMessage(ABMsg dwMessage, ref APPBARDATA pData);

        /// <summary>
        /// Queries the dots per inch (dpi) of a display.
        /// </summary>
        /// <param name="hMonitor">Handle of the monitor being queried.</param>
        /// <param name="dpiType">The type of DPI being queried. Possible values are from the <see cref="MONITOR_DPI_TYPE"/> enumeration.</param>
        /// <param name="dpiX">The value of the DPI along the X axis. This value always refers to the horizontal edge, even when the screen is rotated.</param>
        /// <param name="dpiY">The value of the DPI along the Y axis. This value always refers to the vertical edge, even when the screen is rotated.</param>
        [DllImport(DLL_NAME, ExactSpelling = true, PreserveSig = false)]
        public static extern void GetDpiForMonitor(IntPtr hMonitor, MONITOR_DPI_TYPE dpiType, out int dpiX, out int dpiY);
    }
}
