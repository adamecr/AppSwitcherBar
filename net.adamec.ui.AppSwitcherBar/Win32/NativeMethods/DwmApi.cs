using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
using System;
using System.Runtime.InteropServices;
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeMethods
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

        /// <summary>
        /// Retrieves the current value of a specified Desktop Window Manager (DWM) attribute applied to a window.
        /// </summary>
        /// <param name="hwnd">The handle to the window from which the attribute value is to be retrieved.</param>
        /// <param name="dwAttribute">A flag describing which value to retrieve, specified as a value of the DWMWINDOWATTRIBUTE enumeration. This parameter specifies which attribute to retrieve, and the pvAttribute parameter points to an object into which the attribute value is retrieved.</param>
        /// <param name="pvAttribute">A pointer to a value which, when this function returns successfully, receives the current value of the attribute. The type of the retrieved value depends on the value of the dwAttribute parameter. The DWMWINDOWATTRIBUTE enumeration topic indicates, in the row for each flag, what type of value you should pass a pointer to in the pvAttribute parameter.</param>
        /// <param name="cbAttribute">The size, in bytes, of the attribute value being received via the pvAttribute parameter. The type of the retrieved value, and therefore its size in bytes, depends on the value of the dwAttribute parameter.</param>
        /// <returns>If the function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [DllImport(DLL_NAME)]
        public static extern int DwmGetWindowAttribute(IntPtr hwnd, uint dwAttribute, ref bool pvAttribute, int cbAttribute);

        /// <summary>
        /// Retrieves the current value of a specified Desktop Window Manager (DWM) attribute applied to a window.
        /// </summary>
        /// <param name="hwnd">The handle to the window from which the attribute value is to be retrieved.</param>
        /// <param name="dwAttribute">A flag describing which value to retrieve, specified as a value of the DWMWINDOWATTRIBUTE enumeration. This parameter specifies which attribute to retrieve, and the pvAttribute parameter points to an object into which the attribute value is retrieved.</param>
        /// <param name="pvAttribute">A pointer to a value which, when this function returns successfully, receives the current value of the attribute. The type of the retrieved value depends on the value of the dwAttribute parameter. The DWMWINDOWATTRIBUTE enumeration topic indicates, in the row for each flag, what type of value you should pass a pointer to in the pvAttribute parameter.</param>
        /// <param name="cbAttribute">The size, in bytes, of the attribute value being received via the pvAttribute parameter. The type of the retrieved value, and therefore its size in bytes, depends on the value of the dwAttribute parameter.</param>
        /// <returns>If the function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [DllImport(DLL_NAME)]
        public static extern int DwmGetWindowAttribute(IntPtr hwnd, uint dwAttribute, ref int pvAttribute, int cbAttribute);


    }
}
