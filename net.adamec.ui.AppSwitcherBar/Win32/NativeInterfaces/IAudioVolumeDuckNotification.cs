using System.Runtime.InteropServices;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming


namespace net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces;

/// <summary>
/// The IAudioVolumeDuckNotification interface is used to by the system to send notifications about stream attenuation changes.Stream Attenuation, or ducking, is a feature introduced in Windows 7, where the system adjusts the volume of a non-communication stream when a new communication stream is opened.
/// </summary>
[ComImport]
[Guid(Win32Consts.IID_IAudioVolumeDuckNotification)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IAudioVolumeDuckNotification
{
    /// <summary>
    /// The OnVolumeDuckNotification method sends a notification about a pending system ducking event.
    /// </summary>
    /// <param name="sessionID">A string containing the session instance identifier of the communications session that raises the the auto-ducking event. To get the session instance identifier, call IAudioSessionControl2::GetSessionInstanceIdentifier.</param>
    /// <param name="countCommunicationSessions">The number of active communications sessions. If there are n sessions, the sessions are numbered from 0 to –1.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig] HRESULT OnVolumeDuckNotification([MarshalAs(UnmanagedType.LPWStr)] string sessionID, uint countCommunicationSessions);
    /// <summary>
    /// The OnVolumeUnduckNotification method sends a notification about a pending system unducking event. 
    /// </summary>
    /// <param name="sessionID">A string containing the session instance identifier of the terminating communications session that initiated the ducking. To get the session instance identifier, call IAudioSessionControl2::GetSessionInstanceIdentifier.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig] HRESULT OnVolumeUnduckNotification([MarshalAs(UnmanagedType.LPWStr)] string sessionID);
}