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
/// The IAudioSessionControl interface enables a client to configure the control parameters for an audio session and to monitor events in the session. 
/// </summary>
[ComImport]
[Guid(Win32Consts.IID_IAudioSessionControl)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IAudioSessionControl
{
    /// <summary>
    /// The GetState method retrieves the current state of the audio session.
    /// </summary>
    /// <param name="state">Pointer to a variable into which the method writes the current session state. The state must be one of the AudioSessionState enumeration values</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig] HRESULT GetState([Out, MarshalAs(UnmanagedType.U4)] out AudioSessionState state);
    /// <summary>
    /// The GetDisplayName method retrieves the display name for the audio session.
    /// </summary>
    /// <param name="displayName">Pointer to a pointer variable into which the method writes the address of a null-terminated, wide-character string that contains the display name. The method allocates the storage for the string. </param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig] HRESULT GetDisplayName([Out, MarshalAs(UnmanagedType.LPWStr)] out string displayName);
    /// <summary>
    /// The SetDisplayName method assigns a display name to the current session.
    /// </summary>
    /// <param name="Value">Pointer to a null-terminated, wide-character string that contains the display name for the session.</param>
    /// <param name="EventContext">Pointer to the event-context GUID. If a call to this method generates a name-change event, the session manager sends notifications to all clients that have registered IAudioSessionEvents interfaces with the session manager. The session manager includes the EventContext pointer value with each notification. Upon receiving a notification, a client can determine whether it or another client is the source of the event by inspecting the EventContext value. This scheme depends on the client selecting a value for this parameter that is unique among all clients in the session. If the caller supplies a NULL pointer for this parameter, the client's notification method receives a NULL context pointer.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig] HRESULT SetDisplayName([MarshalAs(UnmanagedType.LPWStr)] string Value, ref Guid EventContext);
    /// <summary>
    /// The GetIconPath method retrieves the path for the display icon for the audio session.
    /// </summary>
    /// <param name="path">Pointer to a pointer variable into which the method writes the address of a null-terminated, wide-character string that specifies the fully qualified path of an .ico, .dll, or .exe file that contains the icon. The method allocates the storage for the string.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig] HRESULT GetIconPath([Out, MarshalAs(UnmanagedType.LPWStr)] out string path);
    /// <summary>
    /// The SetIconPath method assigns a display icon to the current session.
    /// </summary>
    /// <param name="Value">Pointer to a null-terminated, wide-character string that specifies the path and file name of an .ico, .dll, or .exe file that contains the icon.</param>
    /// <param name="EventContext">Pointer to the event-context GUID. If a call to this method generates an icon-change event, the session manager sends notifications to all clients that have registered IAudioSessionEvents interfaces with the session manager. The session manager includes the EventContext pointer value with each notification. Upon receiving a notification, a client can determine whether it or another client is the source of the event by inspecting the EventContext value. This scheme depends on the client selecting a value for this parameter that is unique among all clients in the session. If the caller supplies a NULL pointer for this parameter, the client's notification method receives a NULL context pointer.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig] HRESULT SetIconPath([MarshalAs(UnmanagedType.LPWStr)] string Value, ref Guid EventContext);
    /// <summary>
    /// The GetGroupingParam method retrieves the grouping parameter of the audio session.
    /// </summary>
    /// <param name="guid">Output pointer for the grouping-parameter GUID. This parameter must be a valid, non-NULL pointer to a caller-allocated GUID variable. The method writes the grouping parameter into this variable.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig] HRESULT GetGroupingParam(out Guid guid);
    /// <summary>
    /// The SetGroupingParam method assigns a session to a grouping of sessions.
    /// </summary>
    /// <param name="Override">The new grouping parameter. This parameter must be a valid, non-NULL pointer to a grouping-parameter GUID.</param>
    /// <param name="EventContext">Pointer to the event-context GUID. If a call to this method generates a grouping-change event, the session manager sends notifications to all clients that have registered IAudioSessionEvents interfaces with the session manager. The session manager includes the EventContext pointer value with each notification. Upon receiving a notification, a client can determine whether it or another client is the source of the event by inspecting the EventContext value. This scheme depends on the client selecting a value for this parameter that is unique among all clients in the session. If the caller supplies a NULL pointer for this parameter, the client's notification method receives a NULL context pointer.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig] HRESULT SetGroupingParam(ref Guid Override, ref Guid EventContext);
    /// <summary>
    /// The RegisterAudioSessionNotification method registers the client to receive notifications of session events, including changes in the stream state.
    /// </summary>
    /// <param name="NewNotifications">Pointer to a client-implemented IAudioSessionEvents interface. If the method succeeds, it calls the AddRef method on the client's IAudioSessionEvents interface.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig] HRESULT RegisterAudioSessionNotification([MarshalAs(UnmanagedType.Interface)] IAudioSessionEvents NewNotifications);
    /// <summary>
    /// The UnregisterAudioSessionNotification method deletes a previous registration by the client to receive notifications.
    /// </summary>
    /// <param name="NewNotifications">Pointer to a client-implemented IAudioSessionEvents interface. The client passed this same interface pointer to the session manager in a previous call to the IAudioSessionControl::RegisterAudioSessionNotification method. If the UnregisterAudioSessionNotification method succeeds, it calls the Release method on the client's IAudioSessionEvents interface.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig] HRESULT UnregisterAudioSessionNotification([MarshalAs(UnmanagedType.Interface)] IAudioSessionEvents NewNotifications);
}