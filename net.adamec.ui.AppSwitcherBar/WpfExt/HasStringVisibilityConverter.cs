using System;
using System.Windows.Data;
using System.Windows;

namespace net.adamec.ui.AppSwitcherBar.WpfExt
{
    /// <summary>
    /// Visibility converter - visible when a string value is not null nor empty. Otherwise it's collapsed
    /// </summary>
    public class HasStringVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="targetType">Type to convert to</param>
        /// <param name="parameter">Converter parameter</param>
        /// <param name="culture">Culture to be used</param>
        /// <returns>Visible when a string value is not null nor empty. Otherwise it's collapsed</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.IsNullOrEmpty(value as string) ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Converts a value back - not used
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="targetType">Type to convert to</param>
        /// <param name="parameter">Converter parameter</param>
        /// <param name="culture">Culture to be used</param>
        /// <returns>Always null</returns>
        public object? ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
      
    }
}
