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
/// The ISimpleAudioVolume interface enables a client to control the master volume level of an audio session.
/// </summary>
[ComImport]
[Guid(Win32Consts.IID_ISimpleAudioVolume)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface ISimpleAudioVolume
{
    /// <summary>
    /// The SetMasterVolume method sets the master volume level for the audio session.
    /// </summary>
    /// <param name="fLevel">The new master volume level. Valid volume levels are in the range 0.0 to 1.0.</param>
    /// <param name="EventContext">Pointer to the event-context GUID. If a call to this method generates a volume-change event, the session manager sends notifications to all clients that have registered IAudioSessionEvents interfaces with the session manager. The session manager includes the EventContext pointer value with each notification. Upon receiving a notification, a client can determine whether it or another client is the source of the event by inspecting the EventContext value. This scheme depends on the client selecting a value for this parameter that is unique among all clients in the session. If the caller supplies a NULL pointer for this parameter, the client's notification method receives a NULL context pointer.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig] HRESULT SetMasterVolume(float fLevel, ref Guid EventContext);
    /// <summary>
    /// The GetMasterVolume method retrieves the client volume level for the audio session.
    /// </summary>
    /// <param name="pfLevel">Pointer to a float variable into which the method writes the client volume level. The volume level is a value in the range 0.0 to 1.0.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig] HRESULT GetMasterVolume(out float pfLevel);
    /// <summary>
    /// The SetMute method sets the muting state for the audio session.
    /// </summary>
    /// <param name="bMute">The new muting state. TRUE enables muting. FALSE disables muting.</param>
    /// <param name="EventContext">Pointer to the event-context GUID. If a call to this method generates a volume-change event, the session manager sends notifications to all clients that have registered IAudioSessionEvents interfaces with the session manager. The session manager includes the EventContext pointer value with each notification. Upon receiving a notification, a client can determine whether it or another client is the source of the event by inspecting the EventContext value. This scheme depends on the client selecting a value for this parameter that is unique among all clients in the session. If the caller supplies a NULL pointer for this parameter, the client's notification method receives a NULL context pointer.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig] HRESULT SetMute(int bMute, ref Guid EventContext);
    /// <summary>
    /// The GetMute method retrieves the current muting state for the audio session.
    /// </summary>
    /// <param name="mute">Pointer to a BOOL variable into which the method writes the muting state. TRUE indicates that muting is enabled. FALSE indicates that it is disabled.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig] HRESULT GetMute(out int mute);
}