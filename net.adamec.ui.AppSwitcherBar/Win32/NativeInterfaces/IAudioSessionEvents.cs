using System;
using System.Runtime.InteropServices;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming


namespace net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces;

/// <summary>
/// The IAudioSessionEvents interface provides notifications of session-related events such as changes in the volume level, display name, and session state. 
/// </summary>
[ComImport]
[Guid(Win32Consts.IID_IAudioSessionEvents)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IAudioSessionEvents
{
    /// <summary>
    /// The OnDisplayNameChanged method notifies the client that the display name for the session has changed.
    /// </summary>
    /// <param name="NewDisplayName">The new display name for the session. This parameter points to a null-terminated, wide-character string containing the new display name. The string remains valid for the duration of the call.</param>
    /// <param name="EventContext">The event context value. This is the same value that the caller passed to IAudioSessionControl::SetDisplayName in the call that changed the display name for the session.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig] HRESULT OnDisplayNameChanged([MarshalAs(UnmanagedType.LPWStr)] string NewDisplayName, ref Guid EventContext);
    /// <summary>
    /// The OnIconPathChanged method notifies the client that the display icon for the session has changed.
    /// </summary>
    /// <param name="NewIconPath">The path for the new display icon for the session. This parameter points to a string that contains the path for the new icon. The string pointer remains valid only for the duration of the call.</param>
    /// <param name="EventContext">The event context value. This is the same value that the caller passed to IAudioSessionControl::SetIconPath in the call that changed the display icon for the session. </param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig] HRESULT OnIconPathChanged([MarshalAs(UnmanagedType.LPWStr)] string NewIconPath, ref Guid EventContext);
    /// <summary>
    /// The OnSimpleVolumeChanged method notifies the client that the volume level or muting state of the audio session has changed.
    /// </summary>
    /// <param name="NewVolume">The new volume level for the audio session. This parameter is a value in the range 0.0 to 1.0, where 0.0 is silence and 1.0 is full volume (no attenuation).</param>
    /// <param name="NewMute">The new muting state. If TRUE, muting is enabled. If FALSE, muting is disabled.</param>
    /// <param name="EventContext">The event context value. This is the same value that the caller passed to ISimpleAudioVolume::SetMasterVolume or ISimpleAudioVolume::SetMute in the call that changed the volume level or muting state of the session. </param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig] HRESULT OnSimpleVolumeChanged(float NewVolume, int NewMute, ref Guid EventContext);
    /// <summary>
    /// The OnChannelVolumeChanged method notifies the client that the volume level of an audio channel in the session submix has changed.
    /// </summary>
    /// <param name="ChannelCount">The channel count. This parameter specifies the number of audio channels in the session submix.</param>
    /// <param name="afNewChannelVolume">Pointer to an array of volume levels. Each element is a value of type float that specifies the volume level for a particular channel. Each volume level is a value in the range 0.0 to 1.0, where 0.0 is silence and 1.0 is full volume (no attenuation). The number of elements in the array is specified by the ChannelCount parameter. If an audio stream contains n channels, the channels are numbered from 0 to n– 1. The array element whose index matches the channel number, contains the volume level for that channel. Assume that the array remains valid only for the duration of the call.</param>
    /// <param name="ChangedChannel">The number of the channel whose volume level changed. Use this value as an index into the NewChannelVolumeArray array. If the session submix contains n channels, the channels are numbered from 0 to n– 1. If more than one channel might have changed (for example, as a result of a call to the IChannelAudioVolume::SetAllVolumes method), the value of ChangedChannel is (DWORD)(–1).</param>
    /// <param name="EventContext">The event context value. This is the same value that the caller passed to the IChannelAudioVolume::SetChannelVolume or IChannelAudioVolume::SetAllVolumes method in the call that initiated the change in volume level of the channel. </param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig] HRESULT OnChannelVolumeChanged(uint ChannelCount, IntPtr afNewChannelVolume, uint ChangedChannel, ref Guid EventContext);
    /// <summary>
    /// The OnGroupingParamChanged method notifies the client that the grouping parameter for the session has changed.
    /// </summary>
    /// <param name="NewGroupingParam">The new grouping parameter for the session. This parameter points to a grouping-parameter GUID.</param>
    /// <param name="EventContext">The event context value. This is the same value that the caller passed to IAudioSessionControl::SetGroupingParam in the call that changed the grouping parameter for the session.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig] HRESULT OnGroupingParamChanged(ref Guid NewGroupingParam, ref Guid EventContext);
    /// <summary>
    /// The OnStateChanged method notifies the client that the stream-activity state of the session has changed.
    /// </summary>
    /// <param name="NewState">The new session state. The value of this parameter is one of the AudioSessionState enumeration values</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig] HRESULT OnStateChanged(AudioSessionState NewState);
    /// <summary>
    /// The OnSessionDisconnected method notifies the client that the audio session has been disconnected.
    /// </summary>
    /// <param name="DisconnectReason">The reason that the audio session was disconnected. The caller sets this parameter to one of the AudioSessionDisconnectReason enumeration values</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig] HRESULT OnSessionDisconnected(AudioSessionDisconnectReason DisconnectReason);
}