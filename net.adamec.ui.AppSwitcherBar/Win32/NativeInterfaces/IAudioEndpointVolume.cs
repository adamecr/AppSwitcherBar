using System;
using System.Runtime.InteropServices;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces;

/// <summary>
/// The IAudioEndpointVolume interface represents the volume controls on the audio stream to or from an audio endpoint device.
/// A client obtains a reference to the IAudioEndpointVolume interface of an endpoint device by calling the IMMDevice::Activate
/// method with parameter iid set to REFIID IID_IAudioEndpointVolume.
/// </summary>
[ComImport]
[Guid(Win32Consts.IID_IAudioEndpointVolume)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IAudioEndpointVolume
{
    /// <summary>
    /// The RegisterControlChangeNotify method registers a client's notification callback interface.
    /// </summary>
    /// <param name="pNotify">Pointer to the IAudioEndpointVolumeCallback interface that the client is registering for notification callbacks.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig]
    HRESULT RegisterControlChangeNotify(IAudioEndpointVolumeCallback pNotify);
    /// <summary>
    /// The UnregisterControlChangeNotify method deletes the registration of a client's notification callback interface that the client registered in a previous call to the IAudioEndpointVolume::RegisterControlChangeNotify method.
    /// </summary>
    /// <param name="pNotify">Pointer to the client's IAudioEndpointVolumeCallback interface. The client passed this same interface pointer to the endpoint volume object in a previous call to the IAudioEndpointVolume::RegisterControlChangeNotify method. If the UnregisterControlChangeNotify method succeeds, it calls the Release method on the client's IAudioEndpointVolumeCallback interface.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig]
    HRESULT UnregisterControlChangeNotify(IAudioEndpointVolumeCallback pNotify);
    /// <summary>
    /// The GetChannelCount method gets a count of the channels in the audio stream that enters or leaves the audio endpoint device.
    /// </summary>
    /// <param name="pnChannelCount">Pointer to a UINT variable into which the method writes the channel count.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig]
    HRESULT GetChannelCount(out int pnChannelCount);
    /// <summary>
    /// The SetMasterVolumeLevel method sets the master volume level, in decibels, of the audio stream that enters or leaves the audio endpoint device.
    /// </summary>
    /// <param name="fLevelDB">The new master volume level in decibels. To obtain the range and granularity of the volume levels that can be set by this method, call the IAudioEndpointVolume::GetVolumeRange method.</param>
    /// <param name="pguidEventContext">Context value for the IAudioEndpointVolumeCallback::OnNotify method. This parameter points to an event-context GUID. If the SetMasterVolumeLevel call changes the volume level of the endpoint, all clients that have registered IAudioEndpointVolumeCallback interfaces with that endpoint will receive notifications. In its implementation of the OnNotify method, a client can inspect the event-context GUID to discover whether it or another client is the source of the volume-change event. If the caller supplies a NULL pointer for this parameter, the notification routine receives the context GUID value GUID_NULL.</param>
    /// <returns>If the method succeeds, it returns S_OK</returns>
    [PreserveSig]
    HRESULT SetMasterVolumeLevel(float fLevelDB, Guid pguidEventContext);
    /// <summary>
    /// The SetMasterVolumeLevelScalar method sets the master volume level of the audio stream that enters or leaves the audio endpoint device. The volume level is expressed as a normalized, audio-tapered value in the range from 0.0 to 1.0.
    /// </summary>
    /// <param name="fLevel">The new master volume level. The level is expressed as a normalized value in the range from 0.0 to 1.0.</param>
    /// <param name="pguidEventContext">Context value for the IAudioEndpointVolumeCallback::OnNotify method. This parameter points to an event-context GUID. If the SetMasterVolumeLevelScalar call changes the volume level of the endpoint, all clients that have registered IAudioEndpointVolumeCallback interfaces with that endpoint will receive notifications. In its implementation of the OnNotify method, a client can inspect the event-context GUID to discover whether it or another client is the source of the volume-change event. If the caller supplies a NULL pointer for this parameter, the notification routine receives the context GUID value GUID_NULL.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig]
    HRESULT SetMasterVolumeLevelScalar(float fLevel, Guid pguidEventContext);
    /// <summary>
    /// The GetMasterVolumeLevel method gets the master volume level, in decibels, of the audio stream that enters or leaves the audio endpoint device.
    /// </summary>
    /// <param name="pfLevelDB">Pointer to the master volume level. This parameter points to a float variable into which the method writes the volume level in decibels. To get the range of volume levels obtained from this method, call the IAudioEndpointVolume::GetVolumeRange method.</param>
    /// <returns>If the method succeeds, it returns S_OK</returns>
    [PreserveSig]
    HRESULT GetMasterVolumeLevel(out float pfLevelDB);
    /// <summary>
    /// The GetMasterVolumeLevelScalar method gets the master volume level of the audio stream that enters or leaves the audio endpoint device. The volume level is expressed as a normalized, audio-tapered value in the range from 0.0 to 1.0.
    /// </summary>
    /// <param name="pfLevel">Pointer to the master volume level. This parameter points to a float variable into which the method writes the volume level. The level is expressed as a normalized value in the range from 0.0 to 1.0.</param>
    /// <returns>If the method succeeds, it returns S_OK</returns>
    [PreserveSig]
    HRESULT GetMasterVolumeLevelScalar(out float pfLevel);
    /// <summary>
    /// The SetChannelVolumeLevel method sets the volume level, in decibels, of the specified channel of the audio stream that enters or leaves the audio endpoint device.
    /// </summary>
    /// <param name="nChannel">The channel number. If the audio stream contains n channels, the channels are numbered from 0 to n– 1. To obtain the number of channels, call the IAudioEndpointVolume::GetChannelCount method.</param>
    /// <param name="fLevelDB">The new volume level in decibels. To obtain the range and granularity of the volume levels that can be set by this method, call the IAudioEndpointVolume::GetVolumeRange method.</param>
    /// <param name="pguidEventContext">Context value for the IAudioEndpointVolumeCallback::OnNotify method. This parameter points to an event-context GUID. If the SetChannelVolumeLevel call changes the volume level of the endpoint, all clients that have registered IAudioEndpointVolumeCallback interfaces with that endpoint will receive notifications. In its implementation of the OnNotify method, a client can inspect the event-context GUID to discover whether it or another client is the source of the volume-change event. If the caller supplies a NULL pointer for this parameter, the notification routine receives the context GUID value GUID_NULL.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig]
    HRESULT SetChannelVolumeLevel(uint nChannel, float fLevelDB, Guid pguidEventContext);
    /// <summary>
    /// The SetChannelVolumeLevelScalar method sets the normalized, audio-tapered volume level of the specified channel in the audio stream that enters or leaves the audio endpoint device.
    /// </summary>
    /// <param name="nChannel">The channel number. If the audio stream contains n channels, the channels are numbered from 0 to n– 1. To obtain the number of channels, call the IAudioEndpointVolume::GetChannelCount method.</param>
    /// <param name="fLevel">The volume level. The volume level is expressed as a normalized value in the range from 0.0 to 1.0.</param>
    /// <param name="pguidEventContext">Context value for the IAudioEndpointVolumeCallback::OnNotify method. This parameter points to an event-context GUID. If the SetChannelVolumeLevelScalar call changes the volume level of the endpoint, all clients that have registered IAudioEndpointVolumeCallback interfaces with that endpoint will receive notifications. In its implementation of the OnNotify method, a client can inspect the event-context GUID to discover whether it or another client is the source of the volume-change event. If the caller supplies a NULL pointer for this parameter, the notification routine receives the context GUID value GUID_NULL.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig]
    HRESULT SetChannelVolumeLevelScalar(uint nChannel, float fLevel, Guid pguidEventContext);
    /// <summary>
    /// The GetChannelVolumeLevel method gets the volume level, in decibels, of the specified channel in the audio stream that enters or leaves the audio endpoint device.
    /// </summary>
    /// <param name="nChannel">The channel number. If the audio stream has n channels, the channels are numbered from 0 to n– 1. To obtain the number of channels in the stream, call the IAudioEndpointVolume::GetChannelCount method.</param>
    /// <param name="pfLevelDB">Pointer to a float variable into which the method writes the volume level in decibels. To get the range of volume levels obtained from this method, call the IAudioEndpointVolume::GetVolumeRange method.</param>
    /// <returns>If the method succeeds, it returns S_OK. </returns>
    [PreserveSig]
    HRESULT GetChannelVolumeLevel(uint nChannel, out float pfLevelDB);
    /// <summary>
    /// The GetChannelVolumeLevelScalar method gets the normalized, audio-tapered volume level of the specified channel of the audio stream that enters or leaves the audio endpoint device.
    /// </summary>
    /// <param name="nChannel">The channel number. If the audio stream contains n channels, the channels are numbered from 0 to n– 1. To obtain the number of channels, call the IAudioEndpointVolume::GetChannelCount method.</param>
    /// <param name="pfLevel">Pointer to a float variable into which the method writes the volume level. The level is expressed as a normalized value in the range from 0.0 to 1.0.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig]
    HRESULT GetChannelVolumeLevelScalar(uint nChannel, out float pfLevel);
    /// <summary>
    /// The SetMute method sets the muting state of the audio stream that enters or leaves the audio endpoint device.
    /// </summary>
    /// <param name="bMute">The new muting state. If bMute is TRUE, the method mutes the stream. If FALSE, the method turns off muting.</param>
    /// <param name="pguidEventContext"></param>
    /// <returns>If the method succeeds and the muting state changes, the method returns S_OK.
    /// If the method succeeds and the new muting state is the same as the previous muting state, the method returns S_FALSE.
    /// If the method fails, possible return codes include, but are not limited to E_OUTOFMEMORY</returns>
    [PreserveSig]
    HRESULT SetMute([MarshalAs(UnmanagedType.Bool)] bool bMute, Guid pguidEventContext);
    /// <summary>
    /// The GetMute method gets the muting state of the audio stream that enters or leaves the audio endpoint device.
    /// </summary>
    /// <param name="pbMute">Pointer to a BOOL variable into which the method writes the muting state. If *pbMute is TRUE, the stream is muted. If FALSE, the stream is not muted.</param>
    /// <returns>If the method succeeds, it returns S_OK. </returns>
    [PreserveSig]
    HRESULT GetMute(out bool pbMute);
    /// <summary>
    /// The GetVolumeStepInfo method gets information about the current step in the volume range.
    /// </summary>
    /// <param name="pnStep">Pointer to a UINT variable into which the method writes the current step index. This index is a value in the range from 0 to *pStepCount– 1, where 0 represents the minimum volume level and *pStepCount– 1 represents the maximum level.</param>
    /// <param name="pnStepCount">Pointer to a UINT variable into which the method writes the number of steps in the volume range. This number remains constant for the lifetime of the IAudioEndpointVolume interface instance.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig]
    HRESULT GetVolumeStepInfo(out uint pnStep, out uint pnStepCount);
    /// <summary>
    /// The VolumeStepUp method increments, by one step, the volume level of the audio stream that enters or leaves the audio endpoint device.
    /// </summary>
    /// <param name="pguidEventContext">Context value for the IAudioEndpointVolumeCallback::OnNotify method. This parameter points to an event-context GUID. If the VolumeStepUp call changes the volume level of the endpoint, all clients that have registered IAudioEndpointVolumeCallback interfaces with that endpoint will receive notifications. In its implementation of the OnNotify method, a client can inspect the event-context GUID to discover whether it or another client is the source of the volume-change event. If the caller supplies a NULL pointer for this parameter, the client's notification method receives a NULL context pointer.</param>
    /// <returns>If the method succeeds, it returns S_OK</returns>
    [PreserveSig]
    HRESULT VolumeStepUp(Guid pguidEventContext);
    /// <summary>
    /// The VolumeStepDown method decrements, by one step, the volume level of the audio stream that enters or leaves the audio endpoint device.
    /// </summary>
    /// <param name="pguidEventContext">Context value for the IAudioEndpointVolumeCallback::OnNotify method. This parameter points to an event-context GUID. If the VolumeStepDown call changes the volume level of the endpoint, all clients that have registered IAudioEndpointVolumeCallback interfaces with that endpoint will receive notifications. In its implementation of the OnNotify method, a client can inspect the event-context GUID to discover whether it or another client is the source of the volume-change event. If the caller supplies a NULL pointer for this parameter, the client's notification method receives a NULL context pointer.</param>
    /// <returns>If the method succeeds, it returns S_OK. </returns>
    [PreserveSig]
    HRESULT VolumeStepDown(Guid pguidEventContext);
    /// <summary>
    /// The QueryHardwareSupport method queries the audio endpoint device for its hardware-supported functions.
    /// </summary>
    /// <param name="pdwHardwareSupportMask">Pointer to a DWORD variable into which the method writes a hardware support mask that indicates the hardware capabilities of the audio endpoint device. The method can set the mask to 0 or to the bitwise-OR combination of one or more ENDPOINT_HARDWARE_SUPPORT_XXX constants.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig]
    HRESULT QueryHardwareSupport(out uint pdwHardwareSupportMask);
    /// <summary>
    /// The GetVolumeRange method gets the volume range, in decibels, of the audio stream that enters or leaves the audio endpoint device.
    /// </summary>
    /// <param name="pflVolumeMindB">Pointer to the minimum volume level. This parameter points to a float variable into which the method writes the minimum volume level in decibels. This value remains constant for the lifetime of the IAudioEndpointVolume interface instance.</param>
    /// <param name="pflVolumeMaxdB">Pointer to the maximum volume level. This parameter points to a float variable into which the method writes the maximum volume level in decibels. This value remains constant for the lifetime of the IAudioEndpointVolume interface instance.</param>
    /// <param name="pflVolumeIncrementdB">Pointer to the volume increment. This parameter points to a float variable into which the method writes the volume increment in decibels. This increment remains constant for the lifetime of the IAudioEndpointVolume interface instance.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig]
    HRESULT GetVolumeRange(out float pflVolumeMindB, out float pflVolumeMaxdB, out float pflVolumeIncrementdB);
}