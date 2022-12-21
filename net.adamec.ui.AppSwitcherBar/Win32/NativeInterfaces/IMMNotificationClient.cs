using System.Runtime.InteropServices;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
using net.adamec.ui.AppSwitcherBar.Win32.Services.Shell.Properties;
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces;

/// <summary>
/// The IMMNotificationClient interface provides notifications when an audio endpoint device is added or removed, when the state or properties of an endpoint device change, or when there is a change in the default role assigned to an endpoint device.
/// </summary>
[ComImport]
[Guid(Win32Consts.IID_IMMNotificationClient)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IMMNotificationClient
{
    /// <summary>
    /// The OnDeviceStateChanged method indicates that the state of an audio endpoint device has changed.
    /// </summary>
    /// <param name="pwstrDeviceId">Pointer to the endpoint ID string that identifies the audio endpoint device. This parameter points to a null-terminated, wide-character string containing the endpoint ID. The string remains valid for the duration of the call.</param>
    /// <param name="dwNewState">Specifies the new state of the endpoint device. The value of this parameter is one of the AudioDeviceState constants</param>
    /// <returns>If the method succeeds, it returns S_OK</returns>
    [PreserveSig]
    HRESULT OnDeviceStateChanged([In, MarshalAs(UnmanagedType.LPWStr)] string pwstrDeviceId, [MarshalAs(UnmanagedType.I4)] AudioDeviceState dwNewState);
    /// <summary>
    /// The OnDeviceAdded method indicates that a new audio endpoint device has been added.
    /// </summary>
    /// <param name="pwstrDeviceId">Pointer to the endpoint ID string that identifies the audio endpoint device. This parameter points to a null-terminated, wide-character string containing the endpoint ID. The string remains valid for the duration of the call.</param>
    /// <returns>If the method succeeds, it returns S_OK</returns>
    [PreserveSig]
    HRESULT OnDeviceAdded([In, MarshalAs(UnmanagedType.LPWStr)] string pwstrDeviceId);
    /// <summary>
    /// The OnDeviceRemoved method indicates that an audio endpoint device has been removed.
    /// </summary>
    /// <param name="pwstrDeviceId">Pointer to the endpoint ID string that identifies the audio endpoint device. This parameter points to a null-terminated, wide-character string containing the endpoint ID. The string remains valid for the duration of the call.</param>
    /// <returns>If the method succeeds, it returns S_OK</returns>
    [PreserveSig]
    HRESULT OnDeviceRemoved([In, MarshalAs(UnmanagedType.LPWStr)] string pwstrDeviceId);
    /// <summary>
    /// The OnDefaultDeviceChanged method notifies the client that the default audio endpoint device for a particular device role has changed.
    /// </summary>
    /// <param name="flow">The data-flow direction of the endpoint device. This parameter is set to one of the EDataFlow enumeration values</param>
    /// <param name="role">The device role of the audio endpoint device. This parameter is set to one of the ERole enumeration values</param>
    /// <param name="pwstrDeviceId">Pointer to the endpoint ID string that identifies the audio endpoint device. This parameter points to a null-terminated, wide-character string containing the endpoint ID. The string remains valid for the duration of the call. If the user has removed or disabled the default device for a particular role, and no other device is available to assume that role, then pwstrDefaultDevice is NULL.</param>
    /// <returns>If the method succeeds, it returns S_OK</returns>
    [PreserveSig]
    HRESULT OnDefaultDeviceChanged(EDataFlow flow, ERole role, [In, MarshalAs(UnmanagedType.LPWStr)] string pwstrDeviceId);
    /// <summary>
    /// The OnPropertyValueChanged method indicates that the value of a property belonging to an audio endpoint device has changed.
    /// </summary>
    /// <param name="pwstrDeviceId">Pointer to the endpoint ID string that identifies the audio endpoint device. This parameter points to a null-terminated, wide-character string that contains the endpoint ID. The string remains valid for the duration of the call.</param>
    /// <param name="key">A PROPERTYKEY structure that specifies the property. </param>
    /// <returns>If the method succeeds, it returns S_OK</returns>
    [PreserveSig]
    HRESULT OnPropertyValueChanged([In, MarshalAs(UnmanagedType.LPWStr)] string pwstrDeviceId, PropertyKey key);
}