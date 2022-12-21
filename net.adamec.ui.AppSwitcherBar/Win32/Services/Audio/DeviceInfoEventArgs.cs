using System;
using net.adamec.ui.AppSwitcherBar.Dto;

namespace net.adamec.ui.AppSwitcherBar.Win32.Services.Audio;

/// <summary>
/// Event arguments for <see cref="IAudioService.DeviceAdded"/>, <see cref="IAudioService.DeviceRemoved"/> and <see cref="IAudioService.DeviceUpdated"/> events
/// </summary>
public class DeviceInfoEventArgs : EventArgs
{
    /// <summary>
    /// Audio device information
    /// </summary>
    public AudioDeviceInfo Device { get; }

    /// <summary>
    /// Internal CTOR - fill the <see cref="Device"/> information from <see cref="AudioDevice"/>
    /// </summary>
    /// <param name="source"><see cref="AudioDevice"/> source. The <see cref="Device"/> will be set to empty if null</param>
    internal DeviceInfoEventArgs(AudioDevice? source)
    {
        Device = new AudioDeviceInfo().UpdateFrom(source);
    }
    /// <summary>
    /// Internal CTOR - fill the <see cref="Device"/> information from <see cref="AudioDeviceInfo"/>
    /// </summary>
    /// <param name="source"><see cref="AudioDeviceInfo"/> source.</param>
    internal DeviceInfoEventArgs(AudioDeviceInfo source)
    {
        Device = new AudioDeviceInfo().UpdateFrom(source);
    }
}