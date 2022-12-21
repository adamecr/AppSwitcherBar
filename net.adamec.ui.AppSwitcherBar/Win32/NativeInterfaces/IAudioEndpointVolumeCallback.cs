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
/// The IAudioEndpointVolumeCallback interface provides notifications of changes in the volume level and muting state of an audio endpoint device.
/// </summary>
[ComImport]
[Guid(Win32Consts.IID_IAudioEndpointVolumeCallback)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IAudioEndpointVolumeCallback
{
    /// <summary>
    /// The OnNotify method notifies the client that the volume level or muting state of the audio endpoint device has changed.
    /// </summary>
    /// <param name="pNotifyData">Pointer to the volume-notification data. This parameter points to a structure of type AUDIO_VOLUME_NOTIFICATION_DATA.</param>
    /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
    [PreserveSig]
    HRESULT OnNotify(IntPtr pNotifyData);
}