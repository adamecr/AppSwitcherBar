using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using net.adamec.ui.AppSwitcherBar.Dto;

namespace net.adamec.ui.AppSwitcherBar.Win32.Services.Audio;

/// <summary>
/// Dummy implementation of <see cref="IAudioService"/>
/// </summary>
public class DummyAudioService : IAudioService
{
    /// <summary>
    /// The empty capture device
    /// </summary>
    public AudioDeviceInfo CaptureDevice { get; } = new();

    /// <summary>
    /// The empty render device
    /// </summary>
    public AudioDeviceInfo RenderDevice { get; } = new();

    /// <summary>
    /// The empty capture communication device
    /// </summary>
    public AudioDeviceInfo CaptureCommDevice { get; } = new();

    /// <summary>
    /// The empty render communication device
    /// </summary>
    public AudioDeviceInfo RenderCommDevice { get; } = new();

    /// <summary>
    /// Empty list of active device infos
    /// </summary>
    public IReadOnlyList<AudioDeviceInfo> Devices { get; } = new List<AudioDeviceInfo>();

#pragma warning disable CS0067
    /// <summary>
    /// Event to be raised when a device is added - not raised at all in <see cref="DummyAudioService"/>
    /// </summary>
    public event EventHandler<DeviceInfoEventArgs>? DeviceAdded;

    /// <summary>
    /// Event to be raised when a device is removed - not raised at all in <see cref="DummyAudioService"/>
    /// </summary>
    public event EventHandler<DeviceInfoEventArgs>? DeviceRemoved;

    /// <summary>
    /// Event to be raised when a device (volume/mute) is updated - not raised at all in <see cref="DummyAudioService"/>
    /// </summary>
    public event EventHandler<DeviceInfoEventArgs>? DeviceUpdated;

    /// <summary>
    /// Event to be raised when a default capture device is changed - not raised at all in <see cref="DummyAudioService"/>
    /// </summary>
    public event EventHandler<EventArgs>? CaptureDeviceChanged;

    /// <summary>
    /// Event to be raised when a default render device is changed - not raised at all in <see cref="DummyAudioService"/>
    /// </summary>
    public event EventHandler<EventArgs>? RenderDeviceChanged;

    /// <summary>
    /// Event raised when a default communication capture device is changed - not raised at all in <see cref="DummyAudioService"/>
    /// </summary>
    public event EventHandler<EventArgs>? CaptureCommDeviceChanged;

    /// <summary>
    /// Event raised when a default communication render device is changed - not raised at all in <see cref="DummyAudioService"/>
    /// </summary>
    public event EventHandler<EventArgs>? RenderCommDeviceChanged;
#pragma warning restore CS0067

    /// <summary>
    /// Toggle mute for given device - does nothing in <see cref="DummyAudioService"/>
    /// </summary>
    /// <param name="deviceId">Device ID </param>
    public void ToggleMuteDevice(string deviceId) { }

    /// <summary>
    /// Changes the device volume - does nothing in <see cref="DummyAudioService"/>
    /// </summary>
    /// <param name="deviceId">Device ID</param>
    /// <param name="delta">Required change delta</param>
    public void ChangeDeviceVolume(string deviceId, float delta) { }

    /// <summary>
    /// Sets default capture device - does nothing in <see cref="DummyAudioService"/>
    /// </summary>
    /// <param name="deviceId">Device ID </param>
    public void SetDefaultCaptureDevice(string deviceId) { }

    /// <summary>
    /// Sets default render device - does nothing in <see cref="DummyAudioService"/>
    /// </summary>
    /// <param name="deviceId">Device ID </param>
    public void SetDefaultRenderDevice(string deviceId) { }

    /// <summary>
    /// Sets default communication capture  device - does nothing in <see cref="DummyAudioService"/>
    /// </summary>
    /// <param name="deviceId">Device ID</param>
    /// <returns>Result of the operation</returns>
    public void SetDefaultCaptureCommDevice(string deviceId) { }

    /// <summary>
    /// Sets default communication render device - does nothing in <see cref="DummyAudioService"/>
    /// </summary>
    /// <param name="deviceId">Device ID</param>
    /// <returns>Result of the operation</returns>
    public void SetDefaultRenderCommDevice(string deviceId) { }
    
    /// <summary>
    /// Occurs when a property value changes
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raise <see cref="PropertyChanged"/> event for given <paramref name="propertyName"/>
    /// </summary>
    /// <param name="propertyName">Name of the property changed</param>
    // ReSharper disable once UnusedMember.Global
    public void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}