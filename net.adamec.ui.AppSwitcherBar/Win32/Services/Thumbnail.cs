using System;
using System.Windows;
using net.adamec.ui.AppSwitcherBar.Win32.NativeMethods;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
using static net.adamec.ui.AppSwitcherBar.Win32.NativeConstants.Win32Consts;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.Services
{
    /// <summary>
    /// Encapsulates the native DWM thumbnail functionality
    /// </summary>
    internal static class Thumbnail
    {
        /// <summary>
        /// Unregisters thumbnail with given <paramref name="thumbnailHandle"/>. This removes the thumbnail
        /// </summary>
        /// <param name="thumbnailHandle">Handle of the thumbnail to be unregistered</param>
        public static void HideThumbnail(IntPtr thumbnailHandle)
        {
            if (thumbnailHandle != IntPtr.Zero) _ = DwmApi.DwmUnregisterThumbnail(thumbnailHandle);
        }

        /// <summary>
        /// Shows thumbnail of <paramref name="sourceHwnd"/> within <paramref name="targetHwnd"/>.
        /// </summary>
        /// <param name="sourceHwnd">Window to show in thumbnail</param>
        /// <param name="targetHwnd">Window to place the thumbnail to</param>
        /// <param name="targetRect">Available bounding box within the target window</param>
        /// <param name="thumbnailRect">Final bounding box of the thumbnail within the target window</param>
        /// <returns>Thumbnail handle or <see cref="IntPtr.Zero"/> when the function fails</returns>
        public static IntPtr ShowThumbnail(IntPtr sourceHwnd, IntPtr targetHwnd, Rect targetRect, out Rect thumbnailRect)
        {
            thumbnailRect=Rect.Empty;

            //Register thumbnail
            if (DwmApi.DwmRegisterThumbnail(targetHwnd, sourceHwnd, out var thumbnailHandle) != 0 ||
                thumbnailHandle == IntPtr.Zero) return IntPtr.Zero;

            //Get the size of the thumbnail source window
            _ = DwmApi.DwmQueryThumbnailSourceSize(thumbnailHandle, out var size);

            //Scale and center the thumbnail within the bounding box
            var thumbSize = new Size(size.x, size.y);
            var thumbSizeScaled = ScaleToBound(thumbSize, targetRect.Size);
            var thumbCentered = CenterInRectangle(thumbSizeScaled, targetRect);

            //Provide the visibility, destination rectangle and opacity parameters to the thumbnail to show it
            var props = new DWM_THUMBNAIL_PROPERTIES
            {
                dwFlags = DWM_TNP_VISIBLE | DWM_TNP_RECTDESTINATION | DWM_TNP_OPACITY,
                fVisible = true,
                rcDestination = (RECT)thumbCentered,
                opacity = 255,
            };
            _ = DwmApi.DwmUpdateThumbnailProperties(thumbnailHandle, ref props);

            return thumbnailHandle;
        }

        /// <summary>
        /// Centers given <paramref name="thumbSize"/> within the <paramref name="boundingBox"/>
        /// </summary>
        /// <param name="thumbSize"><see cref="Size"/> of the thumbnail</param>
        /// <param name="boundingBox">Bounding box rectangle</param>
        /// <returns>Rectangle having the <paramref name="thumbSize"/> that is centered within the <paramref name="boundingBox"/></returns>
        private static Rect CenterInRectangle(Size thumbSize, Rect boundingBox)
        {

            var left = boundingBox.Left + (boundingBox.Width - thumbSize.Width) / 2;
            var top = boundingBox.Top + (boundingBox.Height - thumbSize.Height) / 2;

            return new Rect(left, top, thumbSize.Width, thumbSize.Height);
        }

        /// <summary>
        /// Scale given <paramref name="thumbSize"/> into the <paramref name="boundingBox"/> while keeping the aspect ratio
        /// </summary>
        /// <param name="thumbSize"><see cref="Size"/> of the thumbnail</param>
        /// <param name="boundingBox"><see cref="Size"/> of the bounding box</param>
        /// <returns>Scaled <see cref="Size"/> of the thumbnail</returns>
        private static Size ScaleToBound(Size thumbSize, Size boundingBox)
        {
            if (thumbSize.Width == 0 || thumbSize.Height == 0) return new Size(0, 0);
            var widthScale = boundingBox.Width / thumbSize.Width;
            var heightScale = boundingBox.Height / thumbSize.Height;

            var scale = Math.Min(widthScale, heightScale);
            return new Size(thumbSize.Width * scale, thumbSize.Height * scale);
        }
    }
}
