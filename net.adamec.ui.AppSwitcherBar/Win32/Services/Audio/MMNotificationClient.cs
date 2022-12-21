using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;
using net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
using net.adamec.ui.AppSwitcherBar.Win32.Services.Shell.Properties;

namespace net.adamec.ui.AppSwitcherBar.Win32.Services.Audio
{
    /// <summary>
    /// Callback handler for notifications when an audio endpoint device is added or removed,
    /// when the state or properties of an endpoint device change
    /// or when there is a change in the default role assigned to an endpoint device.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    internal class MMNotificationClient : IMMNotificationClient
    {
        /// <summary>
        /// Callback action to be invoked when the list of devices changed (device added, removed, changed status, etc.)
        /// </summary>
        private Action? OnDevicesChangedAction { get; }
        /// <summary>
        /// Callback action to be invoked when the default device is changed (switched)
        /// </summary>
        private Action<AudioDirection, ERole, string?>? OnDefaultDeviceChangedAction { get; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="onDevicesChangedAction">Callback action to be invoked when the list of devices changed (device added, removed, changed status, etc.)</param>
        /// <param name="onDefaultDeviceChangedAction">Callback action to be invoked when the default device is changed (switched)</param>
        public MMNotificationClient(Action? onDevicesChangedAction, Action<AudioDirection, ERole, string?>? onDefaultDeviceChangedAction)
        {
            OnDevicesChangedAction = onDevicesChangedAction;
            OnDefaultDeviceChangedAction = onDefaultDeviceChangedAction;
        }

        /// <summary>
        /// Invokes <see cref="OnDevicesChangedAction"/> when the list of devices changed (device added, removed, changed status, etc.)
        /// </summary>
        private void OnDevicesChanged()
        {
            OnDevicesChangedAction?.Invoke();
        }

        /// <summary>
        /// Invokes <see cref="OnDefaultDeviceChangedAction"/> when the default device is changed (switched)
        /// </summary>
        /// <param name="direction">Audio flow direction for which the default device changed</param>
        /// <param name="role">Audio role for which the default device changed</param>
        /// <param name="deviceId">New default device ID</param>
        private void OnDefaultDeviceChanged(AudioDirection direction, ERole role, string? deviceId)
        {
            OnDefaultDeviceChangedAction?.Invoke(direction, role, deviceId);
        }

        #region IMMNotificationClient implementation

        /// <summary>
        /// The OnDeviceStateChanged method indicates that the state of an audio endpoint device has changed.
        /// Invokes <see cref="OnDevicesChangedAction"/>
        /// </summary>
        /// <param name="deviceId">Pointer to the endpoint ID string that identifies the audio endpoint device. This parameter points to a null-terminated, wide-character string containing the endpoint ID. The string remains valid for the duration of the call.</param>
        /// <param name="newState">Specifies the new state of the endpoint device. The value of this parameter is one of the AudioDeviceState constants</param>
        /// <returns>If the method succeeds, it returns S_OK</returns>

        [PreserveSig]
        public HRESULT OnDeviceStateChanged([In, MarshalAs(UnmanagedType.LPWStr)] string deviceId, [MarshalAs(UnmanagedType.I4)] AudioDeviceState newState)
        {
#if DEBUG
            Debug.WriteLine($"MMNotificationClient - DeviceStateChanged: {deviceId}, {newState}");
#endif
            OnDevicesChanged();
            return HRESULT.S_OK;
        }

        /// <summary>
        /// The OnDeviceAdded method indicates that a new audio endpoint device has been added.
        /// Invokes <see cref="OnDevicesChangedAction"/>
        /// </summary>
        /// <param name="deviceId">Pointer to the endpoint ID string that identifies the audio endpoint device. This parameter points to a null-terminated, wide-character string containing the endpoint ID. The string remains valid for the duration of the call.</param>
        /// <returns>If the method succeeds, it returns S_OK</returns>

        [PreserveSig]
        public HRESULT OnDeviceAdded([In, MarshalAs(UnmanagedType.LPWStr)] string deviceId)
        {
#if DEBUG
            Debug.WriteLine($"MMNotificationClient - DeviceAdded: {deviceId}");
#endif
            OnDevicesChanged();
            return HRESULT.S_OK;
        }

        /// <summary>
        /// The OnDeviceRemoved method indicates that an audio endpoint device has been removed.
        /// Invokes <see cref="OnDevicesChangedAction"/>
        /// </summary>
        /// <param name="deviceId">Pointer to the endpoint ID string that identifies the audio endpoint device. This parameter points to a null-terminated, wide-character string containing the endpoint ID. The string remains valid for the duration of the call.</param>
        /// <returns>If the method succeeds, it returns S_OK</returns>

        [PreserveSig]
        public HRESULT OnDeviceRemoved([In, MarshalAs(UnmanagedType.LPWStr)] string deviceId)
        {
#if DEBUG
            Debug.WriteLine($"MMNotificationClient - DeviceRemoved: {deviceId}");
#endif
            OnDevicesChanged();
            return HRESULT.S_OK;
        }

        /// <summary>
        /// The OnDefaultDeviceChanged method notifies the client that the default audio endpoint device for a particular device role has changed.
        /// 
        /// </summary>
        /// <param name="flow">The data-flow direction of the endpoint device.</param>
        /// <param name="role">The device role of the audio endpoint device.</param>
        /// <param name="deviceId">Endpoint ID string that identifies the audio endpoint device.</param>
        /// <returns>Method returns S_OK</returns>
        [PreserveSig]
        public HRESULT OnDefaultDeviceChanged(EDataFlow flow, ERole role, [In, MarshalAs(UnmanagedType.LPWStr)] string deviceId)
        {
#if DEBUG
            Debug.WriteLine($"MMNotificationClient - DefaultDeviceChanged: {deviceId}, {flow}, {role} ");
#endif

            OnDefaultDeviceChanged(flow == EDataFlow.Capture ? AudioDirection.Capture : AudioDirection.Render, role, deviceId);

            return HRESULT.S_OK;
        }

        /// <summary>
        /// The OnPropertyValueChanged method indicates that the value of a property belonging to an audio endpoint device has changed.
        /// The notification is not used
        /// </summary>
        /// <param name="deviceId">Endpoint ID string that identifies the audio endpoint device.</param>
        /// <param name="key">A PropertyKey structure that specifies the property. </param>
        /// <returns>Method returns S_OK</returns>
        [PreserveSig]
        public HRESULT OnPropertyValueChanged([In, MarshalAs(UnmanagedType.LPWStr)] string deviceId, PropertyKey key)
        {
#if DEBUG
            Debug.WriteLine($"MMNotificationClient - DevicePropertyChanged: {deviceId}, {key.FormatId},{key.PropertyId}");
#endif
            return HRESULT.S_OK;
        }
        #endregion
    }
}
