using System;
using net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;
using net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
// ReSharper disable CommentTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.Services.Audio;

/// <summary>
/// Audion session callback client
/// The IAudioSessionNotification interface provides notification when an audio session is created.
/// The IAudioSessionEvents interface provides notifications of session-related events such as changes in the volume level, display name, and session state. 
/// </summary>
internal class AudioSessionCallback : IAudioSessionNotification, IAudioSessionEvents
    
{
    /// <summary>
    /// Device ID
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Local
    private string DeviceId { get; }

    /// <summary>
    /// Callback action to be invoked when a session is added
    /// </summary>
    private Action? OnDeviceSessionChangedAction { get; }

    /// <summary>
    /// CTOR
    /// </summary>
    /// <param name="deviceId">Device Id</param>
    /// <param name="onDeviceSessionChangedAction"> Callback action to be invoked when a session is added</param>
    public AudioSessionCallback(string deviceId, Action? onDeviceSessionChangedAction)
    {
        DeviceId = deviceId;
        OnDeviceSessionChangedAction = onDeviceSessionChangedAction;
    }

    /// <summary>
    /// Invokes <see cref="OnDeviceSessionChangedAction"/> when a session state is changed or session is disconnected
    /// </summary>
    private void OnDeviceSessionChanged()
    {
        OnDeviceSessionChangedAction?.Invoke();
    }

    #region IAudioSessionNotification
    //The IAudioSessionNotification interface provides notification when an audio session is created.

    /// <summary>
    /// The OnSessionCreated method notifies the registered processes that the audio session has been created.
    /// </summary>
    /// <param name="newSession">Pointer to the IAudioSessionControl interface of the audio session that was created.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    public HRESULT OnSessionCreated(IAudioSessionControl newSession)
    {
        OnDeviceSessionChangedAction?.Invoke();
        return HRESULT.S_OK;
    }

    #endregion


    #region IAudioSessionEvents
    //The IAudioSessionEvents interface provides notifications of session-related events such as changes in the volume level, display name, and session state. 

    /// <summary>
    /// The OnDisplayNameChanged method notifies the client that the display name for the session has changed.
    /// </summary>
    /// <param name="newDisplayName">The new display name for the session. This parameter points to a null-terminated, wide-character string containing the new display name. The string remains valid for the duration of the call.</param>
    /// <param name="eventContext">The event context value. This is the same value that the caller passed to IAudioSessionControl::SetDisplayName in the call that changed the display name for the session.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    public HRESULT OnDisplayNameChanged(string newDisplayName, ref Guid eventContext)
    {
        //do nothing
        return HRESULT.S_OK;
    }

    /// <summary>
    /// The OnIconPathChanged method notifies the client that the display icon for the session has changed.
    /// </summary>
    /// <param name="newIconPath">The path for the new display icon for the session. This parameter points to a string that contains the path for the new icon. The string pointer remains valid only for the duration of the call.</param>
    /// <param name="eventContext">The event context value. This is the same value that the caller passed to IAudioSessionControl::SetIconPath in the call that changed the display icon for the session. </param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    public HRESULT OnIconPathChanged(string newIconPath, ref Guid eventContext)
    {
        //do nothing
        return HRESULT.S_OK;
    }

    /// <summary>
    /// The OnSimpleVolumeChanged method notifies the client that the volume level or muting state of the audio session has changed.
    /// </summary>
    /// <param name="newVolume">The new volume level for the audio session. This parameter is a value in the range 0.0 to 1.0, where 0.0 is silence and 1.0 is full volume (no attenuation).</param>
    /// <param name="newMute">The new muting state. If TRUE, muting is enabled. If FALSE, muting is disabled.</param>
    /// <param name="eventContext">The event context value. This is the same value that the caller passed to ISimpleAudioVolume::SetMasterVolume or ISimpleAudioVolume::SetMute in the call that changed the volume level or muting state of the session. </param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    public HRESULT OnSimpleVolumeChanged(float newVolume, int newMute, ref Guid eventContext)
    {
        //do nothing
        return HRESULT.S_OK;
    }

    /// <summary>
    /// The OnChannelVolumeChanged method notifies the client that the volume level of an audio channel in the session submix has changed.
    /// </summary>
    /// <param name="channelCount">The channel count. This parameter specifies the number of audio channels in the session submix.</param>
    /// <param name="afNewChannelVolume">Pointer to an array of volume levels. Each element is a value of type float that specifies the volume level for a particular channel. Each volume level is a value in the range 0.0 to 1.0, where 0.0 is silence and 1.0 is full volume (no attenuation). The number of elements in the array is specified by the ChannelCount parameter. If an audio stream contains n channels, the channels are numbered from 0 to n– 1. The array element whose index matches the channel number, contains the volume level for that channel. Assume that the array remains valid only for the duration of the call.</param>
    /// <param name="changedChannel">The number of the channel whose volume level changed. Use this value as an index into the NewChannelVolumeArray array. If the session submix contains n channels, the channels are numbered from 0 to n– 1. If more than one channel might have changed (for example, as a result of a call to the IChannelAudioVolume::SetAllVolumes method), the value of ChangedChannel is (DWORD)(–1).</param>
    /// <param name="eventContext">The event context value. This is the same value that the caller passed to the IChannelAudioVolume::SetChannelVolume or IChannelAudioVolume::SetAllVolumes method in the call that initiated the change in volume level of the channel. </param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    public HRESULT OnChannelVolumeChanged(uint channelCount, IntPtr afNewChannelVolume, uint changedChannel,
        ref Guid eventContext)
    {
        //do nothing
        return HRESULT.S_OK;
    }

    /// <summary>
    /// The OnGroupingParamChanged method notifies the client that the grouping parameter for the session has changed.
    /// </summary>
    /// <param name="newGroupingParam">The new grouping parameter for the session. This parameter points to a grouping-parameter GUID.</param>
    /// <param name="eventContext">The event context value. This is the same value that the caller passed to IAudioSessionControl::SetGroupingParam in the call that changed the grouping parameter for the session.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    public HRESULT OnGroupingParamChanged(ref Guid newGroupingParam, ref Guid eventContext)
    {
        //do nothing
        return HRESULT.S_OK;
    }

    /// <summary>
    /// The OnStateChanged method notifies the client that the stream-activity state of the session has changed.
    /// </summary>
    /// <param name="newState">The new session state. The value of this parameter is one of the AudioSessionState enumeration values</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    public HRESULT OnStateChanged(AudioSessionState newState)
    {
        OnDeviceSessionChanged();
        return HRESULT.S_OK;
    }

    /// <summary>
    /// The OnSessionDisconnected method notifies the client that the audio session has been disconnected.
    /// </summary>
    /// <param name="disconnectReason">The reason that the audio session was disconnected. The caller sets this parameter to one of the AudioSessionDisconnectReason enumeration values</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    public HRESULT OnSessionDisconnected(AudioSessionDisconnectReason disconnectReason)
    {
        OnDeviceSessionChanged();
        return HRESULT.S_OK;
    }

    #endregion
}