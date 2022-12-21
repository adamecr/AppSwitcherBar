using System.Runtime.InteropServices;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming


namespace net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces;

/// <summary>
/// The IAudioSessionEnumerator interface enumerates audio sessions on an audio device.
/// </summary>
[ComImport]
[Guid(Win32Consts.IID_IAudioSessionEnumerator)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IAudioSessionEnumerator
{
    /// <summary>
    /// The GetCount method gets the total number of audio sessions that are open on the audio device.
    /// </summary>
    /// <param name="sessionCount">Receives the total number of audio sessions.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig] HRESULT GetCount(out int sessionCount);

    /// <summary>
    /// The GetSession method gets the audio session specified by an audio session number.
    /// </summary>
    /// <param name="sessionNumber">The session number. If there are n sessions, the sessions are numbered from 0 to n – 1. To get the number of sessions, call the IAudioSessionEnumerator::GetCount method.</param>
    /// <param name="sessionControl">Receives a pointer to the IAudioSessionControl interface of the session object in the collection that is maintained by the session enumerator.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig] HRESULT GetSession(int sessionNumber, [Out, MarshalAs(UnmanagedType.Interface)] out IAudioSessionControl sessionControl);
}