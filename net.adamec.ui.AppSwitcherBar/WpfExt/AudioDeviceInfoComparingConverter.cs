using System;
using System.Windows.Data;
using net.adamec.ui.AppSwitcherBar.Dto;

namespace net.adamec.ui.AppSwitcherBar.WpfExt
{

    /// <summary>
    /// <see cref="AudioDeviceInfo"/> comparing converter for <see cref="MultiBinding"/>
    /// </summary>
    class AudioDeviceInfoComparingConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts two source values (<see cref="AudioDeviceInfo"/>) to target value bool - they match by <see cref="AudioDeviceInfo.DeviceId"/>
        /// </summary>
        /// <param name="values">Source values</param>
        /// <param name="targetType">Required target type</param>
        /// <param name="parameter">Converter parameter</param>
        /// <param name="culture">Culture</param>
        /// <returns>True when two audio devices have same ID</returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (values.Length != 2 ||
                values[0] is not AudioDeviceInfo deviceInfo1 ||
                values[1] is not AudioDeviceInfo deviceInfo2)
                return false;

            return deviceInfo1.DeviceId == deviceInfo2.DeviceId;
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public object[]? ConvertBack(object value, Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
