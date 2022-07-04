using System;
using System.Runtime.InteropServices;

namespace net.adamec.ui.AppSwitcherBar.Win32
{
    /// <summary>
    /// Desktop Window Manager (DWM) Win32 Api
    /// </summary>
    internal class DwmApi
    {
        /// <summary>
        /// Name of the DLL
        /// </summary>
        private const string DLL_NAME = "dwmapi";

        /// <summary>
        /// Creates a Desktop Window Manager (DWM) thumbnail relationship between the destination and source windows.
        /// </summary>
        /// <param name="hwndDestination">The handle to the window that will use the DWM thumbnail. Setting the destination window handle to anything other than a top-level window type will result in a return value of <see cref="Win32Consts.E_INVALIDARG"/>.</param>
        /// <param name="hwndSource">The handle to the window to use as the thumbnail source. Setting the source window handle to anything other than a top-level window type will result in a return value of <see cref="Win32Consts.E_INVALIDARG"/>.</param>
        /// <param name="hThumbnail">A pointer to a handle that, when this function returns successfully, represents the registration of the DWM thumbnail.</param>
        /// <returns></returns>
        [DllImport(DLL_NAME)]
        public static extern int DwmRegisterThumbnail(IntPtr hwndDestination, IntPtr hwndSource, out IntPtr hThumbnail);

        /// <summary>
        /// Removes a Desktop Window Manager (DWM) thumbnail relationship created by the <see cref="DwmRegisterThumbnail"/> function.
        /// </summary>
        /// <param name="hThumbnail">The handle to the thumbnail relationship to be removed. Null or non-existent handles will result in a return value of <see cref="Win32Consts.E_INVALIDARG"/>.</param>
        /// <returns>If this function succeeds, it returns <see cref="Win32Consts.S_OK"/>. Otherwise, it returns an HRESULT error code.</returns>
        [DllImport(DLL_NAME)]
        public static extern int DwmUnregisterThumbnail(IntPtr hThumbnail);

        /// <summary>
        /// Retrieves the source size of the Desktop Window Manager (DWM) thumbnail.
        /// </summary>
        /// <param name="hThumbnail">A handle to the thumbnail to retrieve the source window size from.</param>
        /// <param name="size">A pointer to a <see cref="SIZE"/> structure that, when this function returns successfully, receives the size of the source thumbnail.</param>
        /// <returns>If this function succeeds, it returns <see cref="Win32Consts.S_OK"/>. Otherwise, it returns an HRESULT error code.</returns>
        [DllImport(DLL_NAME)]
        public static extern int DwmQueryThumbnailSourceSize(IntPtr hThumbnail, out SIZE size);

        /// <summary>
        /// Updates the properties for a Desktop Window Manager (DWM) thumbnail.
        /// </summary>
        /// <param name="hThumbnail">The handle to the DWM thumbnail to be updated. Null or invalid thumbnails, as well as thumbnails owned by other processes will result in a return value of <see cref="Win32Consts.E_INVALIDARG"/>.</param>
        /// <param name="props">A pointer to a <see cref="DWM_THUMBNAIL_PROPERTIES"/> structure that contains the new thumbnail properties.</param>
        /// <returns>If this function succeeds, it returns <see cref="Win32Consts.S_OK"/>. Otherwise, it returns an HRESULT error code.</returns>
        [DllImport(DLL_NAME)]
        public static extern int DwmUpdateThumbnailProperties(IntPtr hThumbnail, ref DWM_THUMBNAIL_PROPERTIES props);
    }
}
