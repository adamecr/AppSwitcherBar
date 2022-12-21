using System;
using System.Runtime.InteropServices;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming


namespace net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces
{

    /// <summary>
    /// The IAudioSessionManager interface enables a client to access the session controls and volume controls for both cross-process and process-specific audio sessions. 
    /// </summary>
    [ComImport]
    [Guid(Win32Consts.IID_IAudioSessionManager)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioSessionManager
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
    }
}
