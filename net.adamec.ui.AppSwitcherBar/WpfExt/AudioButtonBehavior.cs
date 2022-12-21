using System.Windows;
using System.Windows.Input;
using net.adamec.ui.AppSwitcherBar.Dto;
using net.adamec.ui.AppSwitcherBar.Win32.Services.Audio;
using Wpf.Ui.Common;
// ReSharper disable UnusedMember.Global


namespace net.adamec.ui.AppSwitcherBar.WpfExt
{

    /// <summary>
    /// Behavior extending the <see cref="Wpf.Ui.Controls.Button"/> when used to present the audio device
    /// </summary>
    public class AudioButtonBehavior
    {
        /// <summary>
        /// Audio device ID
        /// </summary>
        public static readonly DependencyProperty DeviceIdProperty = DependencyProperty.RegisterAttached(
            "DeviceId",
            typeof(string),
            typeof(AudioButtonBehavior),
            new UIPropertyMetadata(string.Empty, Attach));

        /// <summary>
        /// Gets the <see cref="DeviceIdProperty"/> value
        /// </summary>
        /// <param name="dependencyObject">Dependency object to get the value from</param>
        /// <returns>The value of <see cref="DeviceIdProperty"/></returns>
        [AttachedPropertyBrowsableForType(typeof(Wpf.Ui.Controls.Button))]
        public static string GetDeviceId(DependencyObject dependencyObject) => (string)dependencyObject.GetValue(DeviceIdProperty);

        /// <summary>
        /// Sets the <see cref="DeviceIdProperty"/> value
        /// </summary>
        /// <param name="dependencyObject">Dependency object to set the value to</param>
        /// <param name="value">New value</param>
        public static void SetDeviceId(DependencyObject dependencyObject, string value) => dependencyObject.SetValue(DeviceIdProperty, value);

        /// <summary>
        /// Icon to be used when the audio device is not muted
        /// </summary>
        public static readonly DependencyProperty DeviceIconProperty = DependencyProperty.RegisterAttached(
            "DeviceIcon",
            typeof(SymbolRegular),
            typeof(AudioButtonBehavior),
            new UIPropertyMetadata(SymbolRegular.Empty, Attach));

        /// <summary>
        /// Gets the <see cref="DeviceIconProperty"/> value
        /// </summary>
        /// <param name="dependencyObject">Dependency object to get the value from</param>
        /// <returns>The value of <see cref="DeviceIconProperty"/></returns>
        [AttachedPropertyBrowsableForType(typeof(Wpf.Ui.Controls.Button))]
        public static SymbolRegular GetDeviceIcon(DependencyObject dependencyObject) => (SymbolRegular)dependencyObject.GetValue(DeviceIconProperty);

        /// <summary>
        /// Sets the <see cref="DeviceIconProperty"/> value
        /// </summary>
        /// <param name="dependencyObject">Dependency object to set the value to</param>
        /// <param name="value">New value</param>
        public static void SetDeviceIcon(DependencyObject dependencyObject, SymbolRegular value) => dependencyObject.SetValue(DeviceIconProperty, value);

        /// <summary>
        /// Icon to be used when the audio device is muted
        /// </summary>
        public static readonly DependencyProperty MutedIconProperty = DependencyProperty.RegisterAttached(
            "MutedIcon",
            typeof(SymbolRegular),
            typeof(AudioButtonBehavior),
            new UIPropertyMetadata(SymbolRegular.Empty, Attach));

        /// <summary>
        /// Gets the <see cref="MutedIconProperty"/> value
        /// </summary>
        /// <param name="dependencyObject">Dependency object to get the value from</param>
        /// <returns>The value of <see cref="MutedIconProperty"/></returns>
        [AttachedPropertyBrowsableForType(typeof(Wpf.Ui.Controls.Button))]
        public static SymbolRegular GetMutedIcon(DependencyObject dependencyObject) => (SymbolRegular)dependencyObject.GetValue(MutedIconProperty);

        /// <summary>
        /// Sets the <see cref="MutedIconProperty"/> value
        /// </summary>
        /// <param name="dependencyObject">Dependency object to set the value to</param>
        /// <param name="value">New value</param>
        public static void SetMutedIcon(DependencyObject dependencyObject, SymbolRegular value) => dependencyObject.SetValue(MutedIconProperty, value);

        /// <summary>
        /// Icon to be used when the audio device is not available
        /// </summary>
        public static readonly DependencyProperty NoDeviceIconProperty = DependencyProperty.RegisterAttached(
            "NoDeviceIcon",
            typeof(SymbolRegular),
            typeof(AudioButtonBehavior),
            new UIPropertyMetadata(SymbolRegular.Empty, Attach));

        /// <summary>
        /// Gets the <see cref="NoDeviceIconProperty"/> value
        /// </summary>
        /// <param name="dependencyObject">Dependency object to get the value from</param>
        /// <returns>The value of <see cref="NoDeviceIconProperty"/></returns>
        [AttachedPropertyBrowsableForType(typeof(Wpf.Ui.Controls.Button))]
        public static SymbolRegular GetNoDeviceIcon(DependencyObject dependencyObject) => (SymbolRegular)dependencyObject.GetValue(NoDeviceIconProperty);

        /// <summary>
        /// Sets the <see cref="NoDeviceIconProperty"/> value
        /// </summary>
        /// <param name="dependencyObject">Dependency object to set the value to</param>
        /// <param name="value">New value</param>
        public static void SetNoDeviceIcon(DependencyObject dependencyObject, SymbolRegular value) => dependencyObject.SetValue(NoDeviceIconProperty, value);

        /// <summary>
        /// Command to be used when changing the volume of audio device
        /// </summary>
        public static readonly DependencyProperty VolumeChangeCommandProperty = DependencyProperty.RegisterAttached(
            "VolumeChangeCommand",
            typeof(ICommand),
            typeof(AudioButtonBehavior),
            new UIPropertyMetadata(null, Attach));

        /// <summary>
        /// Gets the <see cref="VolumeChangeCommandProperty"/> value
        /// </summary>
        /// <param name="dependencyObject">Dependency object to get the value from</param>
        /// <returns>The value of <see cref="VolumeChangeCommandProperty"/></returns>
        [AttachedPropertyBrowsableForType(typeof(Wpf.Ui.Controls.Button))]
        public static ICommand? GetVolumeChangeCommand(DependencyObject dependencyObject) => (ICommand?)dependencyObject.GetValue(VolumeChangeCommandProperty);

        /// <summary>
        /// Sets the <see cref="VolumeChangeCommandProperty"/> value
        /// </summary>
        /// <param name="dependencyObject">Dependency object to set the value to</param>
        /// <param name="value">New value</param>
        public static void SetVolumeChangeCommand(DependencyObject dependencyObject, ICommand? value) => dependencyObject.SetValue(VolumeChangeCommandProperty, value);

        /// <summary>
        /// Flag whether the popup is open
        /// </summary>
        public static readonly DependencyProperty IsPopupOpenProperty = DependencyProperty.RegisterAttached(
            "IsPopupOpen",
            typeof(bool),
            typeof(AudioButtonBehavior),
            new UIPropertyMetadata(false));

        /// <summary>
        /// Gets the <see cref="IsPopupOpenProperty"/> value
        /// </summary>
        /// <param name="dependencyObject">Dependency object to get the value from</param>
        /// <returns>The value of <see cref="IsPopupOpenProperty"/></returns>
        [AttachedPropertyBrowsableForType(typeof(Wpf.Ui.Controls.Button))]
        public static bool GetIsPopupOpen(DependencyObject dependencyObject) => (bool)dependencyObject.GetValue(IsPopupOpenProperty);

        /// <summary>
        /// Sets the <see cref="IsPopupOpenProperty"/> value
        /// </summary>
        /// <param name="dependencyObject">Dependency object to set the value to</param>
        /// <param name="value">New value</param>
        public static void SetIsPopupOpen(DependencyObject dependencyObject, bool value) => dependencyObject.SetValue(IsPopupOpenProperty, value);

        /// <summary>
        /// Flag whether button has a popup 
        /// </summary>
        public static readonly DependencyProperty HasPopupProperty = DependencyProperty.RegisterAttached(
            "HasPopup",
            typeof(bool),
            typeof(AudioButtonBehavior),
            new UIPropertyMetadata(false, Attach));

        /// <summary>
        /// Gets the <see cref="HasPopupProperty"/> value
        /// </summary>
        /// <param name="dependencyObject">Dependency object to get the value from</param>
        /// <returns>The value of <see cref="HasPopupProperty"/></returns>
        [AttachedPropertyBrowsableForType(typeof(Wpf.Ui.Controls.Button))]
        public static bool GetHasPopup(DependencyObject dependencyObject) => (bool)dependencyObject.GetValue(HasPopupProperty);

        /// <summary>
        /// Sets the <see cref="HasPopupProperty"/> value
        /// </summary>
        /// <param name="dependencyObject">Dependency object to set the value to</param>
        /// <param name="value">New value</param>
        public static void SetHasPopup(DependencyObject dependencyObject, bool value) => dependencyObject.SetValue(HasPopupProperty, value);

        /// <summary>
        /// Default audio device info
        /// </summary>
        public static readonly DependencyProperty DefaultDeviceProperty = DependencyProperty.RegisterAttached(
            "DefaultDevice",
            typeof(AudioDeviceInfo),
            typeof(AudioButtonBehavior),
            new UIPropertyMetadata(null, Attach));

        /// <summary>
        /// Gets the <see cref="DefaultDeviceProperty"/> value
        /// </summary>
        /// <param name="dependencyObject">Dependency object to get the value from</param>
        /// <returns>The value of <see cref="DefaultDeviceProperty"/></returns>
        [AttachedPropertyBrowsableForType(typeof(Wpf.Ui.Controls.Button))]
        public static AudioDeviceInfo GetDefaultDevice(DependencyObject dependencyObject) => (AudioDeviceInfo)dependencyObject.GetValue(DefaultDeviceProperty);

        /// <summary>
        /// Sets the <see cref="DefaultDeviceProperty"/> value
        /// </summary>
        /// <param name="dependencyObject">Dependency object to set the value to</param>
        /// <param name="value">New value</param>
        public static void SetDefaultDevice(DependencyObject dependencyObject, AudioDeviceInfo value) => dependencyObject.SetValue(DefaultDeviceProperty, value);

        /// <summary>
        /// Default communication audio device info
        /// </summary>
        public static readonly DependencyProperty DefaultCommDeviceProperty = DependencyProperty.RegisterAttached(
            "DefaultCommDevice",
            typeof(AudioDeviceInfo),
            typeof(AudioButtonBehavior),
            new UIPropertyMetadata(null));

        /// <summary>
        /// Gets the <see cref="DefaultCommDeviceProperty"/> value
        /// </summary>
        /// <param name="dependencyObject">Dependency object to get the value from</param>
        /// <returns>The value of <see cref="DefaultCommDeviceProperty"/></returns>
        [AttachedPropertyBrowsableForType(typeof(Wpf.Ui.Controls.Button))]
        public static AudioDeviceInfo GetDefaultCommDevice(DependencyObject dependencyObject) => (AudioDeviceInfo)dependencyObject.GetValue(DefaultCommDeviceProperty);

        /// <summary>
        /// Sets the <see cref="DefaultDeviceProperty"/> value
        /// </summary>
        /// <param name="dependencyObject">Dependency object to set the value to</param>
        /// <param name="value">New value</param>
        public static void SetDefaultCommDevice(DependencyObject dependencyObject, AudioDeviceInfo value) => dependencyObject.SetValue(DefaultCommDeviceProperty, value);

        /// <summary>
        /// Attaches the behavior to element - sets the command parameter to device id, primary command to toggle mute and mouse wheel handler (to send volume change command)
        /// </summary>
        /// <param name="dependencyObject">Dependency object to attach the behavior to</param>
        /// <param name="e">Event arguments containing the reference to changed (set) property and old+new value</param>
        private static void Attach(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is not Wpf.Ui.Controls.Button button) return;

            if (e.Property == DeviceIdProperty)
            {
                button.CommandParameter = e.NewValue;
            }

            if (e.Property == VolumeChangeCommandProperty)
            {
                button.MouseWheel -= ButtonMouseWheel;
                button.MouseWheel += ButtonMouseWheel;
            }

            if (e.Property == HasPopupProperty)
            {
                button.MouseRightButtonUp -= ButtonOnMouseRightButtonUp;
                if ((bool)e.NewValue)
                {
                    button.MouseRightButtonUp += ButtonOnMouseRightButtonUp;
                    SetIsPopupOpen(dependencyObject, false);
                }
            }

        }

        /// <summary>
        /// Mouse right click handler - toggle popup (<see cref="IsPopupOpenProperty"/>)
        /// </summary>
        /// <param name="sender">Button the behavior is attached to</param>
        /// <param name="e">Event arguments</param>
        private static void ButtonOnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Wpf.Ui.Controls.Button button) return;
            SetIsPopupOpen(button, !GetIsPopupOpen(button));
        }

        /// <summary>
        /// Mouse wheel handler - sends the volume changed command on wheel
        /// </summary>
        /// <param name="sender">Button the behavior is attached to</param>
        /// <param name="e">Event arguments</param>
        private static void ButtonMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is not Wpf.Ui.Controls.Button button || e.Delta == 0) return;

            var command = GetVolumeChangeCommand(button);


            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
            command?.Execute(new AudioVolumeChangeRequest(GetDeviceId(button) ?? string.Empty, e.Delta < 0 ? -0.01f : 0.01f)); //do change by 1%
        }
    }
}
