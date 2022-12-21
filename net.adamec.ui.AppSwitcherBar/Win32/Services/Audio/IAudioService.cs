using System;
using System.Collections.Generic;
using net.adamec.ui.AppSwitcherBar.Dto;

namespace net.adamec.ui.AppSwitcherBar.Win32.Services.Audio;

/// <summary>
/// Interface for service for audio device management
/// </summary>
public interface IAudioService
{
    /// <summary>
    /// The capture device
    /// </summary>
    AudioDeviceInfo CaptureDevice { get; }

    /// <summary>
    /// The render device
    /// </summary>
    AudioDeviceInfo RenderDevice { get; }

    /// <summary>
    /// Capture communication device
    /// </summary>
    AudioDeviceInfo CaptureCommDevice { get; }

    /// <summary>
    /// Render communication device
    /// </summary>
    AudioDeviceInfo RenderCommDevice { get; }

    /// <summary>
    /// List of active device infos
    /// </summary>
    IReadOnlyList<AudioDeviceInfo> Devices { get; }

    /// <summary>
    /// Event raised when a device is added
    /// </summary>
    event EventHandler<DeviceInfoEventArgs>? DeviceAdded;

    /// <summary>
    /// Event raised when a device is removed
    /// </summary>
    event EventHandler<DeviceInfoEventArgs>? DeviceRemoved;

    /// <summary>
    /// Event raised when a device (volume/mute) is updated
    /// </summary>
    event EventHandler<DeviceInfoEventArgs>? DeviceUpdated;

    /// <summary>
    /// Event raised when a default capture device is changed
    /// </summary>
    event EventHandler<EventArgs>? CaptureDeviceChanged;

    /// <summary>
    /// Event raised when a default render device is changed
    /// </summary>
    event EventHandler<EventArgs>? RenderDeviceChanged;

    /// <summary>
    /// Event raised when a default communication capture device is changed
    /// </summary>
    event EventHandler<EventArgs>? CaptureCommDeviceChanged;

    /// <summary>
    /// Event raised when a default communication render device is changed
    /// </summary>
    event EventHandler<EventArgs>? RenderCommDeviceChanged;

    /// <summary>
    /// Toggle mute for given device
    /// </summary>
    /// <param name="deviceId">Device ID</param>
    void ToggleMuteDevice(string deviceId);
    /// <summary>
    /// Changes the device volume
    /// </summary>
    /// <param name="deviceId">Device ID</param>
    /// <param name="delta">Required change delta (volume is in scale 0.0-1.0, so the delta should reflect it)</param>
    void ChangeDeviceVolume(string deviceId, float delta);

    /// <summary>
    /// Sets default capture device
    /// </summary>
    /// <param name="deviceId">Device ID </param>
    void SetDefaultCaptureDevice(string deviceId);

    /// <summary>
    /// Sets default communication capture  device
    /// </summary>
    /// <param name="deviceId">Device ID</param>
    /// <returns>Result of the operation</returns>
    void SetDefaultCaptureCommDevice(string deviceId);

    /// <summary>
    /// Sets default communication render device
    /// </summary>
    /// <param name="deviceId">Device ID</param>
    /// <returns>Result of the operation</returns>
    void SetDefaultRenderCommDevice(string deviceId);

    /// <summary>
    /// Sets default render device
    /// </summary>
    /// <param name="deviceId">Device ID </param>
    public void SetDefaultRenderDevice(string deviceId);
}