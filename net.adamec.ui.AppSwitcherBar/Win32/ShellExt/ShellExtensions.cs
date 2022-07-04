using System;
using System.Runtime.InteropServices;
using System.Text;

namespace net.adamec.ui.AppSwitcherBar.Win32.ShellExt
{
    /// <summary>
    /// Extended (complex) functionality of Win32 Shell
    /// </summary>
    internal class ShellExtensions
    {
        /// <summary>
        /// Gets the application user model ID for the specified process.
        /// </summary>
        /// <param name="hProcess">A handle to the process. This handle must have the PROCESS_QUERY_LIMITED_INFORMATION access right.</param>
        /// <param name="appModelIdLength">On input, the size of the applicationUserModelId buffer, in wide characters. On success, the size of the buffer used, including the null terminator.</param>
        /// <param name="sbAppUserModelId">A buffer that receives the application user model ID.</param>
        /// <returns></returns>
        [DllImport("kernel32", SetLastError = true)]
        internal static extern int GetApplicationUserModelId(IntPtr hProcess, ref uint appModelIdLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder sbAppUserModelId);

        /// <summary>
        /// Retrieves an object that represents a specific window's collection of properties, which allows those properties to be queried or set.
        /// </summary>
        /// <param name="handle">A handle to the window whose properties are being retrieved.</param>
        /// <param name="riid">A reference to the IID (interface GUID) of the property store object to retrieve through <paramref name="ppv"/>.
        /// This is typically <see cref="Win32Consts.IID_IPropertyStore"/>.</param>
        /// <param name="ppv">When this function returns, contains the interface pointer requested in <paramref name="riid"/>. </param>
        /// <returns></returns>
        [DllImport("shell32.dll", SetLastError = true)]
        internal static extern int SHGetPropertyStoreForWindow(IntPtr handle, ref Guid riid, out IPropertyStore ppv);


        /// <summary>
        /// Retrieves the <see cref="IPropertyStore"/> of the window
        /// </summary>
        /// <param name="handle">A handle to the window whose properties are being retrieved.</param>
        /// <param name="store">Property store </param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        internal static int SHGetPropertyStoreForWindow(IntPtr handle, out IPropertyStore? store)
        {
            var g = new Guid(Win32Consts.IID_IPropertyStore);
            return SHGetPropertyStoreForWindow(handle, ref g, out store);
        }




    }
}
