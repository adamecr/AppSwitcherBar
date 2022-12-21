using net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace net.adamec.ui.AppSwitcherBar.Win32.Services.Audio
{
    /// <summary>
    /// Callback handler for notifications of changes in the volume level and muting state of an audio endpoint device.
    /// </summary>
    internal class AudioEndpointVolumeCallback : IAudioEndpointVolumeCallback
    {
        /// <summary>
        /// Device ID
        /// </summary>
        private string DeviceId{get;}
        
        /// <summary>
        /// Audio endpoint volume registering the callback
        /// </summary>
        private  IAudioEndpointVolume EndpointVolume { get; }

        /// <summary>
        /// Callback action to be invoked when a mute/volume changed
        /// </summary>
        private Action<bool,float>? OnEndpointVolumeChangedAction { get; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="deviceId">Device Id</param>
        /// <param name="endpointVolume">Audio endpoint volume registering the callback</param>
        /// <param name="onEndpointVolumeChangedAction">Callback action to be invoked when a mute/volume changed</param>
        internal AudioEndpointVolumeCallback(string deviceId,IAudioEndpointVolume endpointVolume, Action<bool, float>? onEndpointVolumeChangedAction)
        {
            DeviceId=deviceId;
            EndpointVolume = endpointVolume ?? throw new ArgumentNullException(nameof(endpointVolume));
            OnEndpointVolumeChangedAction = onEndpointVolumeChangedAction;
        }

        /// <summary>
        /// The OnNotify method notifies the client that the volume level or muting state of the audio endpoint device has changed.
        /// Get's the updated volume/mute information from <see cref="EndpointVolume"/>
        /// </summary>
        /// <param name="notifyData">Pointer to the volume-notification data. It's ignored</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        [PreserveSig]
        public HRESULT OnNotify(IntPtr notifyData)
        {
            if (EndpointVolume.GetMute(out var isMute).IsSuccess &&
                EndpointVolume.GetMasterVolumeLevelScalar(out var level).IsSuccess)
            {
#if DEBUG
                // ReSharper disable once StringLiteralTypo
                Debug.WriteLine(
                    $"AudioEndpointVolumeCallback {DeviceId}: {(isMute ? "MUTED" : "unmuted")}, {level:P}");
#endif
                OnEndpointVolumeChangedAction?.Invoke(isMute,level);
            }

            return HRESULT.S_OK;
        }
    }
}
