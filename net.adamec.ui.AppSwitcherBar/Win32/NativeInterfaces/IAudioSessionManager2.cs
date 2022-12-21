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
/// The IAudioSessionManager2 interface enables an application to manage submixes for the audio device.
/// </summary>
[ComImport]
[Guid(Win32Consts.IID_IAudioSessionManager2)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IAudioSessionManager2
{
    /// <summary>
    /// The GetAudioSessionControl method retrieves an audio session control.
    /// </summary>
    /// <param name="AudioSessionGuid">Pointer to a session GUID. If the GUID does not identify a session that has been previously opened, the call opens a new but empty session. The Sndvol program does not display a volume-level control for a session unless it contains one or more active streams. If this parameter is NULL or points to the value GUID_NULL, the method assigns the stream to the default session.</param>
    /// <param name="StreamFlags">Specifies the status of the flags for the audio stream.</param>
    /// <param name="SessionControl">Pointer to a pointer variable into which the method writes a pointer to the IAudioSessionControl interface of the audio session control object. The caller is responsible for releasing the interface, when it is no longer needed, by calling the interface's Release method. If the call fails, *SessionControl is NULL.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig] HRESULT GetAudioSessionControl(ref Guid AudioSessionGuid, uint StreamFlags, [MarshalAs(UnmanagedType.Interface)] out IAudioSessionControl SessionControl);
    /// <summary>
    /// The GetSimpleAudioVolume method retrieves a simple audio volume control.
    /// </summary>
    /// <param name="AudioSessionGuid">Pointer to a session GUID. If the GUID does not identify a session that has been previously opened, the call opens a new but empty session. The Sndvol program does not display a volume-level control for a session unless it contains one or more active streams. If this parameter is NULL or points to the value GUID_NULL, the method assigns the stream to the default session.</param>
    /// <param name="StreamFlags">Specifies the status of the flags for the audio stream.</param>
    /// <param name="AudioVolume">Pointer to a pointer variable into which the method writes a pointer to the ISimpleAudioVolume interface of the audio volume control object. This interface represents the simple audio volume control for the current process. The caller is responsible for releasing the interface, when it is no longer needed, by calling the interface's Release method. If the Activate call fails, *AudioVolume is NULL.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig] HRESULT GetSimpleAudioVolume(ref Guid AudioSessionGuid, uint StreamFlags, [MarshalAs(UnmanagedType.Interface)] out ISimpleAudioVolume AudioVolume);
    /// <summary>
    /// The GetSessionEnumerator method gets a pointer to the audio session enumerator object.
    /// </summary>
    /// <param name="sessionEnumerator">Receives a pointer to the IAudioSessionEnumerator interface of the session enumerator object that the client can use to enumerate audio sessions on the audio device.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig] HRESULT GetSessionEnumerator([Out, MarshalAs(UnmanagedType.Interface)] out IAudioSessionEnumerator sessionEnumerator);
    /// <summary>
    /// The RegisterSessionNotification method registers the application to receive a notification when a session is created.
    /// </summary>
    /// <param name="SessionNotification">A pointer to the application's implementation of the IAudioSessionNotification interface. If the method call succeeds, it calls the AddRef method on the application's IAudioSessionNotification interface.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig] HRESULT RegisterSessionNotification([MarshalAs(UnmanagedType.Interface)] IAudioSessionNotification SessionNotification);
    /// <summary>
    /// The UnregisterSessionNotification method deletes the registration to receive a notification when a session is created.
    /// </summary>
    /// <param name="SessionNotification">A pointer to the application's implementation of the IAudioSessionNotification interface. Pass the same interface pointer that was specified to the session manager in a previous call to IAudioSessionManager2::RegisterSessionNotification to register for notification.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig] HRESULT UnregisterSessionNotification([MarshalAs(UnmanagedType.Interface)] IAudioSessionNotification SessionNotification);
    /// <summary>
    /// The RegisterDuckNotification method registers the application with the session manager to receive ducking notifications.
    /// </summary>
    /// <param name="sessionID">Pointer to a null-terminated string that contains a session instance identifier. Applications that are playing a media stream and want to provide custom stream attenuation or ducking behavior, pass their own session instance identifier. Other applications that do not want to alter their streams but want to get all the ducking notifications must pass NULL.</param>
    /// <param name="duckNotification">Pointer to the application's implementation of the IAudioVolumeDuckNotification interface. The implementation is called when ducking events are raised by the audio system and notifications are sent to the registered applications.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig] HRESULT RegisterDuckNotification([MarshalAs(UnmanagedType.LPWStr)] string sessionID, [MarshalAs(UnmanagedType.Interface)] IAudioVolumeDuckNotification duckNotification);
    /// <summary>
    /// The UnregisterDuckNotification method deletes a previous registration by the application to receive notifications.
    /// </summary>
    /// Pointer to the IAudioVolumeDuckNotification interface that is implemented by the application. Pass the same interface pointer that was specified to the session manager in a previous call to the IAudioSessionManager2::RegisterDuckNotification method. If the UnregisterDuckNotification method succeeds, it calls the Release method on the application's IAudioVolumeDuckNotification interface.
    /// <param name="duckNotification"></param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig] HRESULT UnregisterDuckNotification([MarshalAs(UnmanagedType.Interface)] IAudioVolumeDuckNotification duckNotification);
}