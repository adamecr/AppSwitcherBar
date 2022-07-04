using System;
using System.Windows;
using System.Windows.Media;

namespace net.adamec.ui.AppSwitcherBar.Wpf
{
    /// <summary>
    /// Helper utilities for WPF
    /// </summary>
    internal static class WpfTools
    {
        /// <summary>
        /// WPF <see cref="Visual"/> extension method.
        /// Converts the WPF dimension to screen dimension taking into the consideration current monitor DPI
        /// </summary>
        /// <param name="d">WPF Visual</param>
        /// <param name="dim">WPF dimension</param>
        /// <returns>Screen dimension</returns>
        public static int WpfDimensionToScreen(this Visual d, double dim)
        {
            var dpi = VisualTreeHelper.GetDpi(d);
            return (int)Math.Ceiling(dim * dpi.PixelsPerDip);
        }

        /// <summary>
        /// WPF <see cref="Visual"/> extension method.
        /// Converts the screen dimension to WPF dimension taking into the consideration current monitor DPI
        /// </summary>
        /// <param name="d">WPF Visual</param>
        /// <param name="dim">Screen dimension</param>
        /// <returns>WPF dimension</returns>
        public static double ScreenDimensionToWpf(this Visual d, double dim)
        {
            var dpi = VisualTreeHelper.GetDpi(d);
            return dim / dpi.PixelsPerDip;
        }

        public static DependencyObject? FindVisualRoot(DependencyObject item)
        {
            var parentObject = VisualTreeHelper.GetParent(item);
            return parentObject == null ? item : FindVisualRoot(parentObject);
        }

        /// <summary>
        /// <see cref="int"/> extension method.
        /// Checks whether the current <paramref name="value"/> is within <paramref name="min"/> and <paramref name="max"/> range.
        /// Returns the value with applied constraint
        /// </summary>
        /// <param name="value">int value</param>
        /// <param name="min">Min value constraint</param>
        /// <param name="max">Max value constraint</param>
        /// <returns>Value with applied constraint</returns>
        public static int EnsureMinMax(this int value, double min, double max)
        {
            if (value<min) return (int)Math.Ceiling(min);
            if (value>max) return (int)Math.Floor(max);

            return value;
        }
    }
}
