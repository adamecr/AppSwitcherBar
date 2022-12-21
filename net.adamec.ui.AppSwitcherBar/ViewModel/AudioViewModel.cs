using System.Collections.ObjectModel;
using net.adamec.ui.AppSwitcherBar.Win32.Services.Audio;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using net.adamec.ui.AppSwitcherBar.Dto;
using net.adamec.ui.AppSwitcherBar.WpfExt;
using net.adamec.ui.AppSwitcherBar.Config;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.Options;

namespace net.adamec.ui.AppSwitcherBar.ViewModel
{
    /// <summary>
    /// View model for AudioControl
    /// </summary>
    public class AudioViewModel : INotifyPropertyChanged
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
        // 5900 LogWrongCommandParameter
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogWrongCommandParameter
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private static readonly Action<ILogger, string, Exception?> __LogWrongCommandParameterDefinition =
            LoggerMessage.Define<string>(
                LogLevel.Critical,
                new EventId(5900, nameof(LogWrongCommandParameter)),
                "Command parameter must be `{commandParameterTypeName}`",
                LogOptions);

        /// <summary>
        /// Logs record (Critical) about the wrong parameter of command (null or wrong type)
        /// </summary>
        /// <param name="commandParameterTypeName">Name of expected <see cref="Type"/> of the command parameter value</param>
        private void LogWrongCommandParameter(string commandParameterTypeName)
        {
            if (logger.IsEnabled(LogLevel.Critical))
            {
                __LogWrongCommandParameterDefinition(logger, commandParameterTypeName, null);

            }
        }

        #endregion

        /// <summary>
        /// Application settings
        /// </summary>
        public IAppSettings Settings { get; }

        /// <summary>
        /// Audio service
        /// </summary>
        private IAudioService AudioService { get; }

        /// <summary>
        /// Application dispatcher
        /// </summary>
        private Dispatcher Dispatcher { get; }

        /// <summary>
        /// Information about capture device
        /// </summary>
        private AudioDeviceInfo captureDevice = new();
        /// <summary>
        /// Information about capture device
        /// </summary>
        public AudioDeviceInfo CaptureDevice
        {
            get => captureDevice;
            private set
            {
                if (captureDevice.DeviceId == value.DeviceId) return;
                captureDevice = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Information about render device
        /// </summary>
        private AudioDeviceInfo renderDevice = new();
        /// <summary>
        /// Information about render device
        /// </summary>
        public AudioDeviceInfo RenderDevice
        {
            get => renderDevice;
            private set
            {
                if (renderDevice.DeviceId == value.DeviceId) return;
                renderDevice = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Information about communication capture device
        /// </summary>
        private AudioDeviceInfo captureCommDevice = new();
        /// <summary>
        /// Information about communication capture device
        /// </summary>
        public AudioDeviceInfo CaptureCommDevice
        {
            get => captureCommDevice;
            private set
            {
                if (captureCommDevice.DeviceId == value.DeviceId) return;
                captureCommDevice = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Information about communication render device
        /// </summary>
        private AudioDeviceInfo renderCommDevice = new();
        /// <summary>
        /// Information about communication render device
        /// </summary>
        public AudioDeviceInfo RenderCommDevice
        {
            get => renderCommDevice;
            private set
            {
                if (renderCommDevice.DeviceId == value.DeviceId) return;
                renderCommDevice = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Available capture devices
        /// </summary>
        public ObservableCollection<AudioDeviceInfo> CaptureDevices { get; } = new();

        /// <summary>
        /// Available render devices
        /// </summary>
        public ObservableCollection<AudioDeviceInfo> RenderDevices { get; } = new();

        /// <summary>
        /// Toggle mute command
        /// </summary>
        public ICommand DeviceToggleMuteCommand { get; }

        /// <summary>
        /// Change device volume command
        /// </summary>
        public ICommand DeviceChangeVolumeCommand { get; }

        /// <summary>
        /// Set default capture device command
        /// </summary>
        public ICommand DeviceSetDefaultCaptureCommand { get; }

        /// <summary>
        /// Set default render device command
        /// </summary>
        public ICommand DeviceSetDefaultRenderCommand { get; }

        /// <summary>
        /// Set default communication capture device command
        /// </summary>
        public ICommand DeviceSetDefaultCaptureCommCommand { get; }

        /// <summary>
        /// Set default communication render device command
        /// </summary>
        public ICommand DeviceSetDefaultRenderCommCommand { get; }


        /// <summary>
        /// Internal CTOR
        /// </summary>
        /// <param name="settings">Application setting</param>
        /// <param name="logger">Logger to be used</param>
        /// <param name="audioService">Audio service to be used</param>
        internal AudioViewModel(IAppSettings settings, ILogger logger, IAudioService audioService)
        {
            this.logger = logger;
            Settings = settings;

            Dispatcher = Application.Current.Dispatcher; // bit ugly, but needed for updating the collection from another thread

            DeviceToggleMuteCommand = new RelayCommand(DeviceToggleMute);
            DeviceChangeVolumeCommand = new RelayCommand(DeviceChangeVolume);
            DeviceSetDefaultCaptureCommand = new RelayCommand(DeviceSetDefaultCapture);
            DeviceSetDefaultRenderCommand = new RelayCommand(DeviceSetDefaultRender);
            DeviceSetDefaultCaptureCommCommand = new RelayCommand(DeviceSetDefaultCaptureComm);
            DeviceSetDefaultRenderCommCommand = new RelayCommand(DeviceSetDefaultRenderComm);

            AudioService = audioService;

            //hook to audio service
            audioService.DeviceAdded += AudioServiceDeviceAdded;
            audioService.DeviceRemoved += AudioServiceDeviceRemoved;
            audioService.DeviceUpdated += AudioServiceDeviceUpdated;
            audioService.CaptureDeviceChanged += AudioServiceCaptureDeviceChanged;
            audioService.RenderDeviceChanged += AudioServiceRenderDeviceChanged;
            audioService.CaptureCommDeviceChanged += AudioServiceCaptureCommDeviceChanged;
            audioService.RenderCommDeviceChanged += AudioServiceRenderCommDeviceChanged;

            //get current state
            foreach (var deviceInfo in AudioService.Devices)
            {
                switch (deviceInfo.Direction)
                {
                    case AudioDirection.Capture:
                        CaptureDevices.Add(deviceInfo);
                        break;
                    case AudioDirection.Render:
                        RenderDevices.Add(deviceInfo);
                        break;
                }
            }

            captureDevice.UpdateFrom(audioService.CaptureDevice);
            renderDevice.UpdateFrom(audioService.RenderDevice);
            captureCommDevice.UpdateFrom(audioService.CaptureCommDevice);
            renderCommDevice.UpdateFrom(audioService.RenderCommDevice);
        }

        /// <summary>
        /// CTOR used by DI
        /// </summary>
        /// <param name="logger">Logger to be used</param>
        /// <param name="options">Application settings configuration</param>
        /// <param name="audioService">Audio service to be used</param>
        // ReSharper disable once UnusedMember.Global
        public AudioViewModel(ILogger<AudioService> logger, IOptions<AppSettings> options, IAudioService audioService)
            : this(options.Value, logger, audioService)
        {
            //used from DI - DI populates the parameters and the internal CTOR is called then
        }

        #region Commands
        /// <summary>
        /// Toggle mute for device
        /// </summary>
        /// <param name="parameter">Device ID as string</param>
        private void DeviceToggleMute(object? parameter)
        {
            if (parameter is not string deviceId || string.IsNullOrEmpty(deviceId))
            {
                LogWrongCommandParameter(nameof(String));
                throw new ArgumentException($"Command parameter must be {nameof(String)}", nameof(parameter));
            }

            var device = CaptureDevices.FirstOrDefault(d => d.DeviceId == deviceId) ?? RenderDevices.FirstOrDefault(d => d.DeviceId == deviceId);

            if (device is null || device.IsEmpty || !device.IsMutable) return;
            AudioService.ToggleMuteDevice(deviceId);
        }

        /// <summary>
        /// Change volume of device
        /// </summary>
        /// <param name="parameter"><see cref="AudioVolumeChangeRequest"/> containing the device ID and requested delta</param>
        private void DeviceChangeVolume(object? parameter)
        {
            if (parameter is not AudioVolumeChangeRequest request)
            {
                LogWrongCommandParameter(nameof(AudioVolumeChangeRequest));
                throw new ArgumentException($"Command parameter must be {nameof(AudioVolumeChangeRequest)}", nameof(parameter));
            }

            if (string.IsNullOrEmpty(request.DeviceId)) return;

            var deviceId = request.DeviceId;
            var device = CaptureDevices.FirstOrDefault(d => d.DeviceId == deviceId) ?? RenderDevices.FirstOrDefault(d => d.DeviceId == deviceId);

            if (device is null || device.IsEmpty) return;

            AudioService.ChangeDeviceVolume(deviceId, request.Delta);

        }

        /// <summary>
        /// Set default capture device
        /// </summary>
        /// <param name="parameter">Device ID as string</param>
        private void DeviceSetDefaultCapture(object? parameter)
        {
            if (parameter is not string deviceId || string.IsNullOrEmpty(deviceId))
            {
                LogWrongCommandParameter(nameof(String));
                throw new ArgumentException($"Command parameter must be {nameof(String)}", nameof(parameter));
            }

            var device = CaptureDevices.FirstOrDefault(d => d.DeviceId == deviceId);

            if (device is null || device.IsEmpty) return;
            AudioService.SetDefaultCaptureDevice(deviceId);
        }

        /// <summary>
        /// Set default render device
        /// </summary>
        /// <param name="parameter">Device ID as string</param>
        private void DeviceSetDefaultRender(object? parameter)
        {
            if (parameter is not string deviceId || string.IsNullOrEmpty(deviceId))
            {
                LogWrongCommandParameter(nameof(String));
                throw new ArgumentException($"Command parameter must be {nameof(String)}", nameof(parameter));
            }

            var device = RenderDevices.FirstOrDefault(d => d.DeviceId == deviceId);

            if (device is null || device.IsEmpty) return;
            AudioService.SetDefaultRenderDevice(deviceId);
        }

        /// <summary>
        /// Set default communication capture device
        /// </summary>
        /// <param name="parameter">Device ID as string</param>
        private void DeviceSetDefaultCaptureComm(object? parameter)
        {
            if (parameter is not string deviceId || string.IsNullOrEmpty(deviceId))
            {
                LogWrongCommandParameter(nameof(String));
                throw new ArgumentException($"Command parameter must be {nameof(String)}", nameof(parameter));
            }

            var device = CaptureDevices.FirstOrDefault(d => d.DeviceId == deviceId);

            if (device is null || device.IsEmpty) return;
            AudioService.SetDefaultCaptureCommDevice(deviceId);
        }

        /// <summary>
        /// Set default communication render device
        /// </summary>
        /// <param name="parameter">Device ID as string</param>
        private void DeviceSetDefaultRenderComm(object? parameter)
        {
            if (parameter is not string deviceId || string.IsNullOrEmpty(deviceId))
            {
                LogWrongCommandParameter(nameof(String));
                throw new ArgumentException($"Command parameter must be {nameof(String)}", nameof(parameter));
            }

            var device = RenderDevices.FirstOrDefault(d => d.DeviceId == deviceId);

            if (device is null || device.IsEmpty) return;
            AudioService.SetDefaultRenderCommDevice(deviceId);
        }
        #endregion

        #region AudioService event handlers

        /// <summary>
        /// Handler for removed device event - remove from collection and make empty default if needed
        /// </summary>
        /// <param name="sender">Audio service</param>
        /// <param name="e">Event arguments</param>
        private void AudioServiceDeviceRemoved(object? sender, DeviceInfoEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                switch (e.Device.Direction)
                {
                    case AudioDirection.Capture:
                        {
                            var deviceInfo = CaptureDevices.FirstOrDefault(d => d.DeviceId == e.Device.DeviceId);
                            if (deviceInfo != null) CaptureDevices.Remove(deviceInfo);
                            if (CaptureDevice.DeviceId == e.Device.DeviceId)
                            {
                                CaptureDevice.MakeEmpty();
                            }
                            if (CaptureCommDevice.DeviceId == e.Device.DeviceId)
                            {
                                CaptureCommDevice.MakeEmpty();
                            }
                            break;
                        }
                    case AudioDirection.Render:
                        {
                            var deviceInfo = RenderDevices.FirstOrDefault(d => d.DeviceId == e.Device.DeviceId);
                            if (deviceInfo != null) RenderDevices.Remove(deviceInfo);
                            if (RenderDevice.DeviceId == e.Device.DeviceId)
                            {
                                RenderDevice.MakeEmpty();
                            }
                            if (RenderCommDevice.DeviceId == e.Device.DeviceId)
                            {
                                RenderCommDevice.MakeEmpty();
                            }
                            break;
                        }
                }
            }, DispatcherPriority.DataBind);
        }

        /// <summary>
        /// Handler for added device event - add to collection
        /// </summary>
        /// <param name="sender">Audio service</param>
        /// <param name="e">Event arguments</param>
        private void AudioServiceDeviceAdded(object? sender, DeviceInfoEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (e.Device.HasDevice && e.Device.Direction == AudioDirection.Capture)
                {
                    CaptureDevices.Add(e.Device);
                }

                if (e.Device.HasDevice && e.Device.Direction == AudioDirection.Render)
                {
                    RenderDevices.Add(e.Device);
                }
            }, DispatcherPriority.DataBind);
        }

        /// <summary>
        /// Handler for default capture device changed event - update <see cref="CaptureDevice"/>
        /// </summary>
        /// <param name="sender">Audio service</param>
        /// <param name="e">Event arguments</param>
        private void AudioServiceCaptureDeviceChanged(object? sender, EventArgs e)
        {
            var newCaptureDevice = new AudioDeviceInfo().UpdateFrom(AudioService.CaptureDevice);
            CaptureDevice = newCaptureDevice;
        }

        /// <summary>
        /// Handler for default render device changed event - update <see cref="RenderDevice"/>
        /// </summary>
        /// <param name="sender">Audio service</param>
        /// <param name="e">Event arguments</param>
        private void AudioServiceRenderDeviceChanged(object? sender, EventArgs e)
        {
            var newRenderDevice = new AudioDeviceInfo().UpdateFrom(AudioService.RenderDevice);
            RenderDevice = newRenderDevice;
        }

        /// <summary>
        /// Handler for default communication capture device changed event - update <see cref="CaptureCommDevice"/>
        /// </summary>
        /// <param name="sender">Audio service</param>
        /// <param name="e">Event arguments</param>
        private void AudioServiceCaptureCommDeviceChanged(object? sender, EventArgs e)
        {
            var newCaptureDevice = new AudioDeviceInfo().UpdateFrom(AudioService.CaptureDevice);
            CaptureCommDevice = newCaptureDevice;
        }

        /// <summary>
        /// Handler for default communication render device changed event - update <see cref="RenderCommDevice"/>
        /// </summary>
        /// <param name="sender">Audio service</param>
        /// <param name="e">Event arguments</param>
        private void AudioServiceRenderCommDeviceChanged(object? sender, EventArgs e)
        {
            var newRenderDevice = new AudioDeviceInfo().UpdateFrom(AudioService.RenderDevice);
            RenderCommDevice = newRenderDevice;
        }

        /// <summary>
        /// Handler for device (volume/mute) updated event - update in collection and in default if needed
        /// </summary>
        /// <param name="sender">Audio service</param>
        /// <param name="e">Event arguments</param>
        private void AudioServiceDeviceUpdated(object? sender, DeviceInfoEventArgs e)
        {
            if (e.Device.Direction == AudioDirection.Capture)
            {
                CaptureDevices.FirstOrDefault(d => d.DeviceId == e.Device.DeviceId)?.UpdateFrom(e.Device);
                if (CaptureDevice.DeviceId == e.Device.DeviceId) CaptureDevice.UpdateFrom(e.Device);
                if (CaptureCommDevice.DeviceId == e.Device.DeviceId) CaptureCommDevice.UpdateFrom(e.Device);
            }

            if (e.Device.Direction == AudioDirection.Render)
            {
                RenderDevices.FirstOrDefault(d => d.DeviceId == e.Device.DeviceId)?.UpdateFrom(e.Device);
                if (RenderDevice.DeviceId == e.Device.DeviceId) RenderDevice.UpdateFrom(e.Device);
                if (RenderCommDevice.DeviceId == e.Device.DeviceId) RenderCommDevice.UpdateFrom(e.Device);
            }
        }
        #endregion

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
}
