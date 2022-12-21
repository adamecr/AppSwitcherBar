using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using net.adamec.ui.AppSwitcherBar.Config;
using net.adamec.ui.AppSwitcherBar.Dto;
using net.adamec.ui.AppSwitcherBar.Win32.NativeClasses;
using net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;
using net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces;
using net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces.Extensions;

namespace net.adamec.ui.AppSwitcherBar.Win32.Services.Audio
{
    /// <summary>
    /// Service for audio device management
    /// </summary>
    public class AudioService : Disposable, IAudioService
    {
        #region Logging
        /// <summary>
        /// Logger used
        /// </summary>
        private readonly ILogger logger;


        //EventIds:
        // 1xx - Windows API "interactions"
        // 2xx - Application Window Button interactions
        // 3xx - Telemetry
        // 9xx - Errors/Exceptions
        // ----
        // 1xxx - JumpList Service (19xx Errors/Exceptions)
        // 2xxx - Startup Service (29xx Errors/Exceptions)
        // 3xxx - AppBarWindow (39xx Errors/Exceptions)
        // 4xxx - BackgroundData Service (49xx Errors/Exceptions)
        // 5xxx - Audio Service + Audio API (59xx Errors/Exceptions)

        /// <summary>
        /// Log definition options
        /// </summary>
        private static readonly LogDefineOptions LogOptions = new() { SkipEnabledCheck = true };
        //----------------------------------------------
        // 5001 MuteToggle
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for MuteToggle
        /// </summary>
        private static readonly Action<ILogger, string, string, string, string, string, Exception?> LogShowThumbnailDefinition =
            LoggerMessage.Define<string, string, string, string, string>(
                LogLevel.Debug,
                new EventId(5001, nameof(LogToggleMute)),
                "Audio device mute {result}: device:{deviceName}, direction:{deviceDirection}, was:{before}, device id:{deviceId}",
                LogOptions);

        /// <summary>
        /// Logs record (Debug) when an audio device mute toggled
        /// </summary>
        /// <param name="result">Description of result</param>
        /// <param name="deviceName">Name of device</param>
        /// <param name="deviceDirection">Direction of audio flow</param>
        /// <param name="before">Status of mute before</param>
        /// <param name="deviceId">Id of the device</param>

        private void LogToggleMute(string result, string deviceName, string deviceDirection, string before, string deviceId)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                LogShowThumbnailDefinition(logger, result, deviceName, deviceDirection, before, deviceId, null);
            }
        }
        //----------------------------------------------
        // 5002 ChangeVolume
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for ChangeVolume
        /// </summary>
        private static readonly Action<ILogger, string, string, string, float, float, string, Exception?> LogChangeVolumeDefinition =
            LoggerMessage.Define<string, string, string, float, float, string>(
                LogLevel.Debug,
                new EventId(5002, nameof(LogChangeVolume)),
                "Audio device change volume {result}: device:{deviceName}, direction:{deviceDirection}, was:{before}, delta:{delta}, device id:{deviceId}",
                LogOptions);

        /// <summary>
        /// Logs record (Debug) when an audio device volume is to be changed
        /// </summary>
        /// <param name="result">Description of result</param>
        /// <param name="deviceName">Name of device</param>
        /// <param name="deviceDirection">Direction of audio flow</param>
        /// <param name="before">Volume before</param>
        /// <param name="delta">Requested change</param>
        /// <param name="deviceId">Id of the device</param>

        private void LogChangeVolume(string result, string deviceName, string deviceDirection, float before, float delta, string deviceId)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                LogChangeVolumeDefinition(logger, result, deviceName, deviceDirection, before, delta, deviceId, null);
            }
        }

        //----------------------------------------------
        // 5003 SetDefaultDevice
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for SetDefaultDevice
        /// </summary>
        private static readonly Action<ILogger, string, string, string, string, ERole, string, Exception?> LogSetDefaultDeviceDefinition =
            LoggerMessage.Define<string, string, string, string, ERole, string>(
                LogLevel.Debug,
                new EventId(5002, nameof(LogSetDefaultDevice)),
                "Audio set default device {result}: device:{deviceName}, direction:{deviceDirection}, was:{before}, delta:{delta}, device id:{deviceId}",
                LogOptions);

        /// <summary>
        /// Logs record (Debug) when an audio device is to be set as default
        /// </summary>
        /// <param name="result">Description of result</param>
        /// <param name="deviceName">Name of device</param>
        /// <param name="deviceDirection">Direction of audio flow</param>
        /// <param name="defaultType">Type of default</param>
        /// <param name="role">Device role</param>
        /// <param name="deviceId">Id of the device</param>

        private void LogSetDefaultDevice(string result, string deviceName, string deviceDirection, string defaultType, ERole role, string deviceId)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                LogSetDefaultDeviceDefinition(logger, result, deviceName, deviceDirection, defaultType, role, deviceId, null);
            }
        }
        //LogSetDefaultDevice(r ? "OK" : "NOK", CaptureDevice.Device, CaptureDevice.Direction.ToString(), "Capture","Console", deviceId);
        #endregion

        /// <summary>
        /// Notification client for added/removed/default change audio devices
        /// </summary>
        private readonly IMMNotificationClient? deviceNotificationClient;

        /// <summary>
        /// Audio device enumerator
        /// </summary>
        private IMMDeviceEnumerator? deviceEnumerator;

        /// <summary>
        /// Application settings
        /// </summary>
        private IAppSettings Settings { get; }

        #region Events
        /// <summary>
        /// Event raised when a device is added
        /// </summary>
        public event EventHandler<DeviceInfoEventArgs>? DeviceAdded;
        /// <summary>
        /// Raise <see cref="DeviceAdded"/> event
        /// </summary>
        /// <param name="device">Information about the device</param>
        private void OnDeviceAdded(AudioDeviceInfo device)
        {
            DeviceAdded?.Invoke(this, new DeviceInfoEventArgs(device));
        }

        /// <summary>
        /// Event raised when a device is removed
        /// </summary>
        public event EventHandler<DeviceInfoEventArgs>? DeviceRemoved;
        /// <summary>
        /// Raise <see cref="DeviceRemoved"/> event
        /// </summary>
        /// <param name="device">Information about the device</param>
        private void OnDeviceRemoved(AudioDeviceInfo device)
        {
            DeviceRemoved?.Invoke(this, new DeviceInfoEventArgs(device));
        }

        /// <summary>
        /// Event raised when a device (volume/mute) is updated
        /// </summary>
        public event EventHandler<DeviceInfoEventArgs>? DeviceUpdated;
        /// <summary>
        /// Raise <see cref="DeviceUpdated"/> event
        /// </summary>
        /// <param name="device">Information about the device</param>
        private void OnDeviceUpdated(AudioDevice? device)
        {
            DeviceUpdated?.Invoke(this, new DeviceInfoEventArgs(device));
        }

        /// <summary>
        /// Event raised when a default capture device is changed
        /// </summary>
        public event EventHandler<EventArgs>? CaptureDeviceChanged;
        /// <summary>
        /// Raise <see cref="CaptureDeviceChanged"/> event
        /// </summary>
        private void OnCaptureDeviceChanged()
        {
            CaptureDeviceChanged?.Invoke(this, EventArgs.Empty);
        }


        /// <summary>
        /// Event raised when a default render device is changed
        /// </summary>
        public event EventHandler<EventArgs>? RenderDeviceChanged;
        /// <summary>
        /// Raise <see cref="RenderDeviceChanged"/> event
        /// </summary>
        private void OnRenderDeviceChanged()
        {
            RenderDeviceChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event raised when a default communication capture device is changed
        /// </summary>
        public event EventHandler<EventArgs>? CaptureCommDeviceChanged;
        /// <summary>
        /// Raise <see cref="CaptureCommDeviceChanged"/> event
        /// </summary>
        private void OnCaptureCommDeviceChanged()
        {
            CaptureCommDeviceChanged?.Invoke(this, EventArgs.Empty);
        }


        /// <summary>
        /// Event raised when a default communication render device is changed
        /// </summary>
        public event EventHandler<EventArgs>? RenderCommDeviceChanged;
        /// <summary>
        /// Raise <see cref="RenderCommDeviceChanged"/> event
        /// </summary>
        private void OnRenderCommDeviceChanged()
        {
            RenderCommDeviceChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        /// <summary>
        /// Internal list of active devices
        /// </summary>
        private List<AudioDevice> ActiveDevices { get; } = new();
        /// <summary>
        /// Internal editable list of active device infos
        /// </summary>
        private List<AudioDeviceInfo> InternalDevices { get; } = new();
        /// <summary>
        /// Public read only list of active device infos
        /// </summary>
        public IReadOnlyList<AudioDeviceInfo> Devices => InternalDevices;

        #region Default devices
        /// <summary>
        /// Capture device
        /// </summary>
        public AudioDeviceInfo CaptureDevice { get; } = new();

        /// <summary>
        /// Render device
        /// </summary>
        public AudioDeviceInfo RenderDevice { get; } = new();

        /// <summary>
        /// Capture communication device
        /// </summary>
        public AudioDeviceInfo CaptureCommDevice { get; } = new();

        /// <summary>
        /// Render communication device
        /// </summary>
        public AudioDeviceInfo RenderCommDevice { get; } = new();

        /// <summary>
        /// Default Capture device for <see cref="ERole.Console"/>
        /// </summary>
        private AudioDevice? defaultCaptureConsoleDevice;
        /// <summary>
        /// Default Capture device for <see cref="ERole.Console"/> - this is the "default capture device" mirrored to <see cref="CaptureDevice"/>
        /// </summary>
        private AudioDevice? DefaultCaptureConsoleDevice
        {
            get => defaultCaptureConsoleDevice;
            set
            {
                if (defaultCaptureConsoleDevice?.Id == value?.Id) return;
                defaultCaptureConsoleDevice = value;
                CaptureDevice.UpdateFrom(defaultCaptureConsoleDevice);
                OnCaptureDeviceChanged();
            }
        }

        /// <summary>
        /// Default Capture device for <see cref="ERole.Multimedia"/>
        /// </summary>
        private AudioDevice? defaultCaptureMultimediaDevice;
        /// <summary>
        /// Default Capture device for <see cref="ERole.Multimedia"/>
        /// </summary>
        private AudioDevice? DefaultCaptureMultimediaDevice
        {
            get => defaultCaptureMultimediaDevice;
            set
            {
                if (defaultCaptureMultimediaDevice?.Id == value?.Id) return;
                defaultCaptureMultimediaDevice = value;
            }
        }

        /// <summary>
        /// Default Capture device for <see cref="ERole.Communications"/>
        /// </summary>
        private AudioDevice? defaultCaptureCommDevice;
        /// <summary>
        /// Default Capture device for <see cref="ERole.Communications"/>
        /// </summary>
        private AudioDevice? DefaultCaptureCommDevice
        {
            get => defaultCaptureCommDevice;
            set
            {
                if (defaultCaptureCommDevice?.Id == value?.Id) return;
                defaultCaptureCommDevice = value;
                CaptureCommDevice.UpdateFrom(defaultCaptureCommDevice);
                OnCaptureCommDeviceChanged();
            }
        }


        /// <summary>
        /// Default Render device for <see cref="ERole.Console"/>
        /// </summary>
        private AudioDevice? defaultRenderConsoleDevice;
        /// <summary>
        /// Default Render device for <see cref="ERole.Console"/> - this is the "default render device" mirrored to <see cref="RenderDevice"/>
        /// </summary>
        private AudioDevice? DefaultRenderConsoleDevice
        {
            get => defaultRenderConsoleDevice;
            set
            {
                if (defaultRenderConsoleDevice?.Id == value?.Id) return;
                defaultRenderConsoleDevice = value;

                RenderDevice.UpdateFrom(defaultRenderConsoleDevice);
                OnRenderDeviceChanged();
            }
        }
        /// <summary>
        /// Default Render device for <see cref="ERole.Multimedia"/>
        /// </summary>
        private AudioDevice? defaultRenderMultimediaDevice;
        /// <summary>
        /// Default Render device for <see cref="ERole.Multimedia"/>
        /// </summary>
        private AudioDevice? DefaultRenderMultimediaDevice
        {
            get => defaultRenderMultimediaDevice;
            set
            {
                if (defaultRenderMultimediaDevice?.Id == value?.Id) return;
                defaultRenderMultimediaDevice = value;
            }
        }
        /// <summary>
        /// Default Render device for <see cref="ERole.Communications"/>
        /// </summary>
        private AudioDevice? defaultRenderCommDevice;
        /// <summary>
        /// Default Render device for <see cref="ERole.Communications"/>
        /// </summary>
        private AudioDevice? DefaultRenderCommDevice
        {
            get => defaultRenderCommDevice;
            set
            {
                if (defaultRenderCommDevice?.Id == value?.Id) return;
                defaultRenderCommDevice = value;
                RenderCommDevice.UpdateFrom(defaultRenderCommDevice);
                OnRenderCommDeviceChanged();
            }
        }
        #endregion

        /// <summary>
        /// Internal CTOR
        /// </summary>
        /// <param name="settings">Application setting</param>
        /// <param name="logger">Logger to be used</param>
        internal AudioService(IAppSettings settings, ILogger logger)
        {
            this.logger = logger;
            Settings = settings;

            if (!Settings.ShowAudioControls) return; //if the setting is off, don't init nor hook the notifications

            deviceNotificationClient = new MMNotificationClient(OnDevicesChanged, OnDefaultDeviceChanged);
            var e = Activator.CreateInstance(typeof(CMMDeviceEnumerator));
            deviceEnumerator = e as IMMDeviceEnumerator;
            if (deviceNotificationClient != null) deviceEnumerator?.RegisterEndpointNotificationCallback(deviceNotificationClient);

            GetActiveDevices();
        }

        /// <summary>
        /// CTOR used by DI
        /// </summary>
        /// <param name="logger">Logger to be used</param>
        /// <param name="options">Application settings configuration</param>
        // ReSharper disable once UnusedMember.Global
        public AudioService(ILogger<AudioService> logger, IOptions<AppSettings> options)
               : this(options.Value, logger)
        {
            //used from DI - DI populates the parameters and the internal CTOR is called then
        }

        #region Notification Client handlers

        /// <summary>
        /// Refreshes active devices when the <see cref="deviceNotificationClient"/> signals devices change
        /// </summary>
        private void OnDevicesChanged()
        {
            GetActiveDevices();
        }

        /// <summary>
        /// Sets default device when the <see cref="deviceNotificationClient"/> signals default device change
        /// </summary>
        /// <param name="direction">Audio flow direction for which the default device changed</param>
        /// <param name="role">Audio role for which the default device changed</param>
        /// <param name="deviceId">New default device ID</param>
        private void OnDefaultDeviceChanged(AudioDirection direction, ERole role, string? deviceId)
        {
            UpdateDefaultDevice(direction, role, deviceId);
        }

        #endregion

        #region Volume/Mute changes
        /// <summary>
        /// Toggle mute for given device
        /// </summary>
        /// <param name="deviceId">Device ID </param>
        public void ToggleMuteDevice(string deviceId)
        {
            var device = ActiveDevices.FirstOrDefault(d => d.Id == deviceId);
            if (device == null) return;

            var r = device.ToggleMute();
            LogToggleMute(r ? "OK" : "NOK", device.Name, device.Direction.ToString(), device.IsMuted ? "muted" : "unmuted", deviceId);
        }

        /// <summary>
        /// Changes the device volume
        /// </summary>
        /// <param name="deviceId">Device ID</param>
        /// <param name="delta">Required change delta (volume is in scale 0.0-1.0, so the delta should reflect it)</param>
        public void ChangeDeviceVolume(string deviceId, float delta)
        {
            var device = ActiveDevices.FirstOrDefault(d => d.Id == deviceId);
            if (device == null) return;

            var r = device.ChangeVolume(delta);
            LogChangeVolume(r ? "OK" : "NOK", device.Name, device.Direction.ToString(), device.Volume, delta, deviceId);
        }
        #endregion

        #region Set default devices
        /// <summary>
        /// Sets default capture device
        /// </summary>
        /// <param name="deviceId">Device ID</param>
        /// <returns>Result of the operation</returns>
        public void SetDefaultCaptureDevice(string deviceId)
        {
            if (CaptureDevice.DeviceId == deviceId) return; //already there
            var r = SetDefaultDevice(deviceId, ERole.Console);
            LogSetDefaultDevice(r ? "OK" : "NOK", CaptureDevice.Device, CaptureDevice.Direction.ToString(), "Capture", ERole.Console, deviceId);
        }

        /// <summary>
        /// Sets default render device
        /// </summary>
        /// <param name="deviceId">Device ID</param>
        /// <returns>Result of the operation</returns>
        public void SetDefaultRenderDevice(string deviceId)
        {
            if (RenderDevice.DeviceId == deviceId) return; //already there
            var r = SetDefaultDevice(deviceId, ERole.Console);
            LogSetDefaultDevice(r ? "OK" : "NOK", CaptureDevice.Device, CaptureDevice.Direction.ToString(), "Render", ERole.Console, deviceId);
        }

        /// <summary>
        /// Sets default communication capture  device
        /// </summary>
        /// <param name="deviceId">Device ID</param>
        /// <returns>Result of the operation</returns>
        public void SetDefaultCaptureCommDevice(string deviceId)
        {
            if (CaptureCommDevice.DeviceId == deviceId) return; //already there
            var r = SetDefaultDevice(deviceId, ERole.Communications);
            LogSetDefaultDevice(r ? "OK" : "NOK", CaptureDevice.Device, CaptureDevice.Direction.ToString(), "Capture", ERole.Communications, deviceId);
        }

        /// <summary>
        /// Sets default communication render device
        /// </summary>
        /// <param name="deviceId">Device ID</param>
        /// <returns>Result of the operation</returns>
        public void SetDefaultRenderCommDevice(string deviceId)
        {
            if (RenderCommDevice.DeviceId == deviceId) return; //already there
            var r = SetDefaultDevice(deviceId, ERole.Communications);
            LogSetDefaultDevice(r ? "OK" : "NOK", CaptureDevice.Device, CaptureDevice.Direction.ToString(), "Render", ERole.Communications, deviceId);
        }

        /// <summary>
        /// Sets the default device for given <paramref name="role"/>
        /// </summary>
        /// <param name="deviceId">Device ID</param>
        /// <param name="role">Required role</param>
        /// <returns>Result of the operation</returns>
        private bool SetDefaultDevice(string deviceId, ERole role)
        {
            var p = Activator.CreateInstance(typeof(CPolicyConfigClient));
            if (p is IPolicyConfig policyConfig)
            {
                return policyConfig.SetDefaultEndpoint(deviceId, role).IsSuccess;
            }

            return false;
        }

        #endregion

        #region Get devices from AudioService
        /// <summary>
        /// Gets all active devices
        /// </summary>
        private void GetActiveDevices()
        {
            GetActiveDevices(EDataFlow.Capture);
            GetActiveDevices(EDataFlow.Render);
        }

        /// <summary>
        /// Gets active devices for given <paramref name="flow"/> (direction)
        /// Updates <see cref="InternalDevices"/> list with changes and raises <see cref="DeviceAdded"/> and <see cref="DeviceRemoved"/> events for deltas
        /// Updates default devices (through <see cref="GetDefaultDevices"/> method)
        /// When a new active device is added to the list, it subscribe for changes (volume/mute) notification
        /// </summary>
        /// <param name="flow">Flow (direction) of audio</param>
        private void GetActiveDevices(EDataFlow flow)
        {
            if (deviceEnumerator == null) return;
            var direction = flow == EDataFlow.Capture ? AudioDirection.Capture : AudioDirection.Render;
            var hr = deviceEnumerator.EnumAudioEndpoints(flow, AudioDeviceState.Active, out var devices);

            if (!hr.IsSuccess || devices == null) return;

            hr = devices.GetCount(out var devicesCount);
            if (!hr.IsSuccess) return;
            var activeDeviceIds = new List<string>();
            for (uint i = 0; i < devicesCount; i++)
            {
                if (!devices.Item(i, out var device).IsSuccess || device == null) continue;
                if (!device.GetId(out var deviceId).IsSuccess || string.IsNullOrEmpty(deviceId)) continue;

                activeDeviceIds.Add(deviceId);
                var activeDevice = ActiveDevices.FirstOrDefault(d => d.Id == deviceId);
                if (activeDevice != null)
                {
                    //already registered, just to ensure the notifications
                    activeDevice.Notify(true);
                }
                else
                {
                    //new device
                    activeDevice = new AudioDevice(device, direction);
                    activeDevice.PropertyChanged += AudioDevicePropertyChanged;
                    ActiveDevices.Add(activeDevice);

                    var deviceInfo = new AudioDeviceInfo().UpdateFrom(activeDevice);
                    InternalDevices.Add(deviceInfo);
                    OnDeviceAdded(deviceInfo);
                }
            }

            var toRemove = ActiveDevices.Where(d => !activeDeviceIds.Contains(d.Id) && d.Direction == direction).ToArray();
            foreach (var audioDevice in toRemove)
            {
                //deactivate
                var deviceInfo = new AudioDeviceInfo().UpdateFrom(audioDevice);
                DisposeActiveDevice(audioDevice.Id);
                OnDeviceRemoved(deviceInfo);
            }

            GetDefaultDevices();
        }

        /// <summary>
        /// Handles the property changed event from audio devices
        /// Raises <see cref="DeviceUpdated"/> event and update the device in <see cref="InternalDevices"/> collection
        /// If the property change impacts default <see cref="CaptureDevice"/>, <see cref="CaptureCommDevice"/>, <see cref="RenderDevice"/> or <see cref="RenderCommDevice"/>,
        /// it also updates their properties.
        /// </summary>
        /// <param name="sender">Audio device</param>
        /// <param name="e">Event arguments</param>
        private void AudioDevicePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is AudioDevice audioDevice)
            {
                OnDeviceUpdated(audioDevice);

                var device = InternalDevices.FirstOrDefault(d => d.DeviceId == audioDevice.Id);
                device?.UpdateFrom(audioDevice);

                if (audioDevice.Id == CaptureDevice.DeviceId)
                {
                    CaptureDevice.UpdateFrom(audioDevice);
                }
                if (audioDevice.Id == RenderDevice.DeviceId)
                {
                    RenderDevice.UpdateFrom(audioDevice);
                }

                if (audioDevice.Id == CaptureCommDevice.DeviceId)
                {
                    CaptureCommDevice.UpdateFrom(audioDevice);
                }
                if (audioDevice.Id == RenderCommDevice.DeviceId)
                {
                    RenderCommDevice.UpdateFrom(audioDevice);
                }
            }
        }

        /// <summary>
        /// Retrieves all default audio devices
        /// </summary>
        private void GetDefaultDevices()
        {
            if (deviceEnumerator == null) return;

            if (deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.Capture, ERole.Console, out var defaultDevice).IsSuccess)
                DefaultCaptureConsoleDevice = ActiveDevices.FirstOrDefault(d => d.Id == defaultDevice?.Id());
            if (deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.Capture, ERole.Multimedia, out defaultDevice).IsSuccess)
                DefaultCaptureMultimediaDevice = ActiveDevices.FirstOrDefault(d => d.Id == defaultDevice?.Id());
            if (deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.Capture, ERole.Communications, out defaultDevice).IsSuccess)
                DefaultCaptureCommDevice = ActiveDevices.FirstOrDefault(d => d.Id == defaultDevice?.Id());

            if (deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.Render, ERole.Console, out defaultDevice).IsSuccess)
                DefaultRenderConsoleDevice = ActiveDevices.FirstOrDefault(d => d.Id == defaultDevice?.Id());
            if (deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.Render, ERole.Multimedia, out defaultDevice).IsSuccess)
                DefaultRenderMultimediaDevice = ActiveDevices.FirstOrDefault(d => d.Id == defaultDevice?.Id());
            if (deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.Render, ERole.Communications, out defaultDevice).IsSuccess)
                DefaultRenderCommDevice = ActiveDevices.FirstOrDefault(d => d.Id == defaultDevice?.Id());

        }

        /// <summary>
        /// Updates single default device based on the when the <see cref="deviceNotificationClient"/> signals default device change (through <see cref="OnDefaultDeviceChanged"/> handler)
        /// </summary>
        /// <param name="direction">Audio flow direction for which the default device changed</param>
        /// <param name="role">Audio role for which the default device changed</param>
        /// <param name="deviceId">New default device ID</param>
        private void UpdateDefaultDevice(AudioDirection direction, ERole role, string? deviceId)
        {
            if (deviceEnumerator == null) return;

            if (direction == AudioDirection.Capture)
            {
                switch (role)
                {
                    case ERole.Console:
                        DefaultCaptureConsoleDevice = ActiveDevices.FirstOrDefault(d => d.Id == deviceId);
                        break;
                    case ERole.Multimedia:
                        DefaultCaptureMultimediaDevice = ActiveDevices.FirstOrDefault(d => d.Id == deviceId);
                        break;
                    case ERole.Communications:
                        DefaultCaptureCommDevice = ActiveDevices.FirstOrDefault(d => d.Id == deviceId);
                        break;
                }
            }
            else
            {

                switch (role)
                {
                    case ERole.Console:
                        DefaultRenderConsoleDevice = ActiveDevices.FirstOrDefault(d => d.Id == deviceId);
                        break;
                    case ERole.Multimedia:
                        DefaultRenderMultimediaDevice = ActiveDevices.FirstOrDefault(d => d.Id == deviceId);
                        break;
                    case ERole.Communications:
                        DefaultRenderCommDevice = ActiveDevices.FirstOrDefault(d => d.Id == deviceId);
                        break;
                }
            }
        }

        #endregion

        #region Dispose
        /// <summary>
        /// Dispose a device - unsubscribe the change notifications, remove from the <see cref="InternalDevices"/> collection and from defaults (if applicable), dispose the <see cref="AudioDevice"/>
        /// </summary>
        /// <param name="deviceId">Device ID</param>
        private void DisposeActiveDevice(string? deviceId)
        {
            if (string.IsNullOrEmpty(deviceId)) return;

            //main collection
            var activeDevice = ActiveDevices.FirstOrDefault(d => d.Id == deviceId);
            if (activeDevice != null)
            {
                activeDevice.Notify(false);
                activeDevice.PropertyChanged -= AudioDevicePropertyChanged;
                ActiveDevices.Remove(activeDevice);
            }

            //info collection
            var deviceInfo = InternalDevices.FirstOrDefault(d => d.DeviceId == deviceId);
            if (deviceInfo != null)
            {
                InternalDevices.Remove(deviceInfo);
            }

            //defaults
            if (DefaultCaptureConsoleDevice?.Id == deviceId) DefaultCaptureConsoleDevice = null;
            if (DefaultCaptureMultimediaDevice?.Id == deviceId) DefaultCaptureMultimediaDevice = null;
            if (DefaultCaptureCommDevice?.Id == deviceId) DefaultCaptureCommDevice = null;

            if (DefaultRenderConsoleDevice?.Id == deviceId) DefaultRenderConsoleDevice = null;
            if (DefaultRenderMultimediaDevice?.Id == deviceId) DefaultRenderMultimediaDevice = null;
            if (DefaultRenderCommDevice?.Id == deviceId) DefaultRenderCommDevice = null;

            //there should be no reference now, so can be disposed
            activeDevice?.Dispose();
        }

        /// <summary>
        /// Dispose the managed and unmanaged resources
        /// </summary>
        protected override void DoDispose()
        {
            foreach (var activeDevice in ActiveDevices.ToArray())
            {
                DisposeActiveDevice(activeDevice.Id);
            }
            ActiveDevices.Clear();

            if (deviceNotificationClient != null) deviceEnumerator?.UnregisterEndpointNotificationCallback(deviceNotificationClient);
            deviceEnumerator = null;
        }
        #endregion
    }
}
