using System;
using System.Collections.Generic;
using System.Diagnostics;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;
using net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces;
using net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces.Extensions;
using net.adamec.ui.AppSwitcherBar.Win32.Services.Shell.Properties;

namespace net.adamec.ui.AppSwitcherBar.Win32.Services.Audio
{
    /// <summary>
    /// Audio device - Wrapped over <see cref="IMMDevice"/> handling the state (properties) and notifications
    /// </summary>
    internal class AudioDevice : DisposableWithNotification
    {
        /// <summary>
        /// Endpoint volume interface used to get mute/volume info
        /// </summary>
        private IAudioEndpointVolume? EndpointVolume { get; set; }
        /// <summary>
        /// Callback wrapper for notifications about mute/volume change
        /// </summary>
        private IAudioEndpointVolumeCallback? VolumeCallback { get; set; }
        /// <summary>
        /// Flag whether the <see cref="VolumeCallback"/> is already registered
        /// </summary>
        private bool IsVolumeCallbackRegistered { get; set; }

        /// <summary>
        /// List of retrieved audio sessions
        /// </summary>
        private List<IAudioSessionControl2> AudioSessions { get; } = new();

        /// <summary>
        /// Session manager for device
        /// </summary>
        private IAudioSessionManager2? AudioSessionManager { get; set; }
        /// <summary>
        /// Callback wrapper for notifications about new sessions and session changes
        /// </summary>
        private AudioSessionCallback? AudioSessionCallback { get; set; }
        /// <summary>
        /// Flag whether the <see cref="AudioSessionCallback"/> is already registered
        /// </summary>
        private bool IsSessionCallbackRegistered { get; set; }

        /// <summary>
        /// Device ID
        /// </summary>
        public string Id { get; }
        /// <summary>
        /// Name of the device - for example Speakers (SB Katana V2)
        /// </summary>
        public string Name { get; } 
        /// <summary>
        /// Audion flow direction
        /// </summary>
        public AudioDirection Direction { get; set; }
        /// <summary>
        /// Name of the interface (physical device) - for example SB Katana V2
        /// </summary>
        public string? InterfaceName { get; } 
        /// <summary>
        /// Name of the device enumerator - for example USB
        /// </summary>
        public string? EnumeratorName { get; }
        /// <summary>
        /// Device description - for example Speakers
        /// </summary>
        public string? Description { get; }


        /// <summary>
        /// Flag whether the device is mutable
        /// </summary>
        public bool IsMutable { get; }

        /// <summary>
        /// Flag whether the device is muted
        /// </summary>
        private bool isMuted;
        /// <summary>
        /// Flag whether the device is muted
        /// </summary>
        public bool IsMuted
        {
            get => isMuted;
            set
            {
                if (isMuted != value)
                {
                    isMuted = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Device volume between 0 and 1
        /// </summary>
        private float volume;

        /// <summary>
        /// Device volume between 0 and 1
        /// </summary>
        public float Volume
        {
            get => volume;
            set
            {
                if (Math.Abs(volume - value) > 0.001) //0.1pct
                {
                    volume = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Flag whether the device is active (has any active session)
        /// </summary>
        private bool isActive;
        /// <summary>
        /// Flag whether the device is active (has any active session)
        /// </summary>
        public bool IsActive
        {
            get => isActive;
            set
            {
                if (value == isActive) return;
                isActive = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// CTOR - get data from native device and initialize/set the notifications
        /// </summary>
        /// <param name="device">Native device</param>
        /// <param name="direction">Audio flow direction</param>
        public AudioDevice(IMMDevice device, AudioDirection direction)
        {
            //Get device attributes
            Id = string.Empty;
            Name = string.Empty;
            Direction = direction;

            var deviceId = device.Id();
            if (deviceId == null) return;

            var hr = device.OpenPropertyStore(StgmAccess.STGM_READ, out var ps);
            if (!hr.IsSuccess || ps == null) return;

            var name = ShellProperty.Get(ps, PropertyKey.PKEY_Device_FriendlyName).ValueAsString;
            if (name == null) return;

            Id = deviceId;
            Name = name;
            InterfaceName = ShellProperty.Get(ps, PropertyKey.PKEY_DeviceInterface_FriendlyName).ValueAsString;
            EnumeratorName = ShellProperty.Get(ps, PropertyKey.PKEY_Device_EnumeratorName).ValueAsString;
            Description = ShellProperty.Get(ps, PropertyKey.PKEY_Device_DeviceDesc).ValueAsString;

            //Initialize notifications
            var g = new Guid(Win32Consts.IID_IAudioEndpointVolume);
            hr = device.Activate(ref g, CLSCTX.ALL, IntPtr.Zero, out var result);
            if (hr.IsSuccess && result is IAudioEndpointVolume endpointVolume)
            {
                IsMutable = true;
                EndpointVolume = endpointVolume;
                isMuted = endpointVolume.GetMute(out var isMute).IsSuccess && isMute;
                if (endpointVolume.GetMasterVolumeLevelScalar(out var level).IsSuccess) Volume = level;
                VolumeCallback = new AudioEndpointVolumeCallback(deviceId, EndpointVolume, (cbIsMuted, cbVolume) =>
                {
                    IsMuted = cbIsMuted;
                    Volume = cbVolume;
                });
            }

            g = new Guid(Win32Consts.IID_IAudioSessionManager2);
            hr = device.Activate(ref g, CLSCTX.ALL, IntPtr.Zero, out result);
            if (hr.IsSuccess && result is IAudioSessionManager2 sessionManager)
            {
                AudioSessionCallback = new AudioSessionCallback(deviceId, () =>
                {
                    // refresh session active
                    ClearSessions();
                    GetSessions();
                });
                AudioSessionManager = sessionManager;
                GetSessions();
            }

            //Set notifications
            Notify(true);
#if DEBUG
            Debug.WriteLine($"Audio device: {Name}, {Direction}, {InterfaceName ?? "[NULL]"}, {EnumeratorName ?? "[NULL]"}, {Description ?? "[NULL]"},{(IsMutable ? "mutable" : "non-mutable")}, {(IsMuted ? "MUTED" : "unmuted")}, {Volume:P}, {deviceId}");
#endif
        }

        /// <summary>
        /// Set/remove the device notifications
        /// </summary>
        /// <param name="set">Flag whether to set (true) or remove (false) notifications</param>
        public void Notify(bool set)
        {
            if (set)
            {
                if (!IsVolumeCallbackRegistered && EndpointVolume != null && VolumeCallback != null)
                {
                    EndpointVolume.RegisterControlChangeNotify(VolumeCallback);
                    IsVolumeCallbackRegistered = true;
                }

                if (!IsSessionCallbackRegistered && AudioSessionManager != null && AudioSessionCallback != null)
                {
                    AudioSessionManager.RegisterSessionNotification(AudioSessionCallback);
                    IsSessionCallbackRegistered = true;
                }

                return;
            }

            if (VolumeCallback != null && EndpointVolume != null)
            {
                EndpointVolume.UnregisterControlChangeNotify(VolumeCallback);
                IsVolumeCallbackRegistered = false;
            }

            if (AudioSessionManager != null && AudioSessionCallback != null)
            {
                AudioSessionManager.UnregisterSessionNotification(AudioSessionCallback);
                IsSessionCallbackRegistered = false;
            }

        }

        /// <summary>
        /// Toggle device mute
        /// </summary>
        /// <returns>The result of operation</returns>
        public bool ToggleMute()
        {
            if (EndpointVolume == null || !IsMutable) return false;
            var newValue = !IsMuted;
            var r = EndpointVolume.SetMute(newValue, Guid.Empty).IsSuccess;

#if DEBUG
            Debug.WriteLine($"Toggle mute {(r?"OK":"NOK")}: {Name}, {Direction}, {(newValue ? "MUTE" : "unmute")}, {Id}");
#endif
            return true;
        }

        /// <summary>
        /// Change device volume
        /// </summary>
        /// <param name="delta">Requested delta (for volume being scalar 0.0-0.1)</param>
        /// <returns>The result of operation</returns>
        public bool ChangeVolume(float delta)  
        {
            if (EndpointVolume == null || !IsMutable) return false;
            var newValue = Volume + delta;
            if (newValue < 0) newValue = 0;
            if (newValue > 1) newValue = 1;
            if (Math.Abs(newValue - Volume) < 0.0001) return false; //no change - usually at min/max

            var r = EndpointVolume.SetMasterVolumeLevelScalar(newValue, Guid.Empty).IsSuccess;

            if (IsMuted && newValue > 0)
            {
                //un-mute
                EndpointVolume.SetMute(false, Guid.Empty);
            }

            if (newValue < 0.005)
            {
                //mute
                EndpointVolume.SetMute(true, Guid.Empty);
            }

#if DEBUG
            Debug.WriteLine($"Change volume {(r ? "OK" : "NOK")}: {Name}, {Direction}, {newValue:P}, {Id}");
#endif
            return true;
        }

        /// <summary>
        /// Get device sessions, registers the session change callbacks and set <see cref="IsActive"/> flag
        /// </summary>
        private void GetSessions()
        {
            if (AudioSessionManager == null) return;
            var hasActiveSession = false;

            if (AudioSessionManager.GetSessionEnumerator(out var sessionEnumerator).IsSuccess)
            {
                if (sessionEnumerator.GetCount(out var sessionCount).IsSuccess)
                {
                    for (var i = 0; i < sessionCount; i++)
                    {
                        // ReSharper disable once SuspiciousTypeConversion.Global
                        if (!sessionEnumerator.GetSession(i, out var sessionControl).IsSuccess ||
                            sessionControl is not IAudioSessionControl2 sessionControl2) continue;
                        {
                            if (sessionControl2.GetState(out var sessionState).IsSuccess &&
                                sessionState == AudioSessionState.Active)
                                hasActiveSession = true;

#if DEBUG
                            sessionControl2.GetDisplayName(out var sessionName);
                            Debug.WriteLine($"AUDIO DEVICE: {Name}, {Direction}, session: {sessionName}:{sessionState}");
#endif

                            AudioSessions.Add(sessionControl2);
                            if (AudioSessionCallback != null) sessionControl2.RegisterAudioSessionNotification(AudioSessionCallback);
                        }
                    }
                }
            }

            IsActive = hasActiveSession;
        }
        
        /// <summary>
        /// Un-register the session change callbacks and clear the sessions collection
        /// </summary>
        private void ClearSessions()
        {

            if (AudioSessionCallback != null)
            {
                foreach (var audioSession in AudioSessions)
                {
                    audioSession.UnregisterAudioSessionNotification(AudioSessionCallback);
                }
            }
            AudioSessions.Clear();
        }

        
        /// <summary>
        /// Dispose unmanaged resources
        /// </summary>
        protected override void DoDispose()
        {
            ClearSessions();
            Notify(false);

            EndpointVolume = null;
            VolumeCallback = null;

            AudioSessionCallback = null;
            AudioSessionManager = null;
        }
    }
}
