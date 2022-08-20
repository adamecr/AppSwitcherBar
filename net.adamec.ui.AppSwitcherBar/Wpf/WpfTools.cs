using System;
using System.Collections.Generic;
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

        /// <summary>
        /// Finds the visual root of <paramref name="item"/>
        /// </summary>
        /// <param name="item">Item to find the visual root for</param>
        /// <returns>Visual root of <paramref name="item"/></returns>
        public static DependencyObject? FindVisualRoot(DependencyObject item)
        {
            var parentObject = VisualTreeHelper.GetParent(item);
            return parentObject == null ? item : FindVisualRoot(parentObject);
        }

        /// <summary>
        /// Finds the visual ancestor (direct or indirect) of <typeparamref name="TAncestor"/> type for <paramref name="item"/>
        /// </summary>
        /// <typeparam name="TAncestor">Type of the ancestor to look for</typeparam>
        /// <param name="item">Item to find the </param>
        /// <returns>Visual ancestor of given type or null when not found any</returns>
        public static TAncestor? FindVisualAncestor<TAncestor>(DependencyObject item) where TAncestor : DependencyObject
        {
            if (item is TAncestor tParent) return tParent;

            while (true)
            {
                var parentObject = VisualTreeHelper.GetParent(item);
                switch (parentObject)
                {
                    case null:
                        return null;
                    case TAncestor dependencyObject:
                        return dependencyObject;
                    default:
                        item = parentObject;
                        break;
                }
            }
        }

        /// <summary>
        /// Gets all children (direct and indirect) having <typeparamref name="TChildType"/>
        /// </summary>
        /// <typeparam name="TChildType">Type of children to look for</typeparam>
        /// <param name="item">Item to get the children for</param>
        /// <returns>Children (direct and indirect) having <typeparamref name="TChildType"/> or empty list if none found</returns>
        public static List<TChildType> AllChildren<TChildType>(DependencyObject item)
        {
            var list = new List<TChildType>();
            for (var count = 0; count < VisualTreeHelper.GetChildrenCount(item); count++)
            {
                var child = VisualTreeHelper.GetChild(item, count);
                if (child is TChildType tChild)
                {
                    list.Add(tChild);
                }
                list.AddRange(AllChildren<TChildType>(child));
            }
            return list;
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
            if (value < min) return (int)Math.Ceiling(min);
            if (value > max) return (int)Math.Floor(max);

            return value;
        }
    }
}
