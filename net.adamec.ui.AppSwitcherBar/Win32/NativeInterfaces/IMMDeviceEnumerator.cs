using System.Runtime.InteropServices;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces
{
    /// <summary>
    /// The IMMDeviceEnumerator interface provides methods for enumerating multimedia device resources. In the current implementation of the MMDevice API, the only device resources that this interface can enumerate are audio endpoint devices.
    /// </summary>
    [ComImport]
    [Guid(Win32Consts.IID_IMMDeviceEnumerator)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMMDeviceEnumerator
    {
        /// <summary>
        /// The EnumAudioEndpoints method generates a collection of audio endpoint devices that meet the specified criteria.
        /// </summary>
        /// <param name="dataFlow">The data-flow direction for the endpoint devices in the collection. The caller should set this parameter to one of the EDataFlow enumeration value</param>
        /// <param name="stateMask">The state or states of the endpoints that are to be included in the collection. The caller should set this parameter to the bitwise OR of one or more of the AudioDeviceState constants</param>
        /// <param name="devices">Pointer to a pointer variable into which the method writes the address of the IMMDeviceCollection interface of the device-collection object. Through this method, the caller obtains a counted reference to the interface.</param>
        /// <returns>If the method succeeds, it returns S_OK.</returns>
        [PreserveSig]
        HRESULT EnumAudioEndpoints(EDataFlow dataFlow, AudioDeviceState stateMask, out IMMDeviceCollection? devices);
        /// <summary>
        /// The GetDefaultAudioEndpoint method retrieves the default audio endpoint for the specified data-flow direction and role.
        /// </summary>
        /// <param name="dataFlow">The data-flow direction for the endpoint devices in the collection. The caller should set this parameter to one of the EDataFlow enumeration value</param>
        /// <param name="role">The role of the endpoint device. The caller should set this parameter to one of the ERole enumeration values</param>
        /// <param name="ppEndpoint">Pointer to a pointer variable into which the method writes the address of the IMMDevice interface of the endpoint object for the default audio endpoint device. Through this method, the caller obtains a counted reference to the interface. </param>
        /// <returns>If the method succeeds, it returns S_OK.</returns>
        [PreserveSig]
        HRESULT GetDefaultAudioEndpoint(EDataFlow dataFlow, ERole role, out IMMDevice? ppEndpoint);
        /// <summary>
        /// The GetDevice method retrieves an audio endpoint device that is identified by an endpoint ID string.
        /// </summary>
        /// <param name="pwstrId">Pointer to a string containing the endpoint ID. The caller typically obtains this string from the IMMDevice::GetId method or from one of the methods in the IMMNotificationClient interface.</param>
        /// <param name="ppDevice">Pointer to a pointer variable into which the method writes the address of the IMMDevice interface for the specified device. Through this method, the caller obtains a counted reference to the interface. </param>
        /// <returns>If the method succeeds, it returns S_OK.</returns>
        [PreserveSig]
        HRESULT GetDevice(string pwstrId, out IMMDevice ppDevice);
        /// <summary>
        /// The RegisterEndpointNotificationCallback method registers a client's notification callback interface.
        /// </summary>
        /// <param name="pClient">Pointer to the IMMNotificationClient interface that the client is registering for notification callbacks.</param>
        /// <returns>If the method succeeds, it returns S_OK.</returns>
        [PreserveSig]
        HRESULT RegisterEndpointNotificationCallback([In][MarshalAs(UnmanagedType.Interface)] IMMNotificationClient pClient);
        /// <summary>
        /// The UnregisterEndpointNotificationCallback method deletes the registration of a notification interface that the client registered in a previous call to the IMMDeviceEnumerator::RegisterEndpointNotificationCallback method.
        /// </summary>
        /// <param name="pClient">Pointer to the client's IMMNotificationClient interface. The client passed this same interface pointer to the device enumerator in a previous call to the IMMDeviceEnumerator::RegisterEndpointNotificationCallback method.</param>
        /// <returns>If the method succeeds, it returns S_OK.</returns>
        [PreserveSig]
        HRESULT UnregisterEndpointNotificationCallback([In][MarshalAs(UnmanagedType.Interface)] IMMNotificationClient pClient);
    }
}
