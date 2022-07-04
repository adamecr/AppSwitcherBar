using System;
using net.adamec.ui.AppSwitcherBar.Win32;

namespace net.adamec.ui.AppSwitcherBar.Wpf
{
    /// <summary>
    /// Thumbnail popup command parameters
    /// </summary>
    public class ThumbnailPopupCommandParams
    {
        /// <summary>
        /// Thumbnail target HWND (popup hwnd)
        /// </summary>
        public IntPtr TargetHwnd { get; }
        /// <summary>
        /// Thumbnail target rectangle (in popup)
        /// </summary>
        public RECT TargetRect { get; }

        /// <summary>
        /// Thumbnail source HWND (window to show in thumbnail)
        /// </summary>
        public IntPtr SourceHwnd { get; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="targetHwnd">Thumbnail target HWND (popup hwnd)</param>
        /// <param name="targetRect">Thumbnail target rectangle (in popup)</param>
        /// <param name="sourceHwnd">Thumbnail source HWND (window to show in thumbnail)</param>
        public ThumbnailPopupCommandParams(IntPtr targetHwnd, RECT targetRect, IntPtr sourceHwnd)
        {
            TargetHwnd = targetHwnd;
            TargetRect = targetRect;
            SourceHwnd = sourceHwnd;
        }

    }
}
