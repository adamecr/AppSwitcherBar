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
/// The IMMDevice interface encapsulates the generic features of a multimedia device resource. In the current implementation of the MMDevice API, the only type of device resource that an IMMDevice interface can represent is an audio endpoint device.
/// </summary>
[ComImport]
[Guid(Win32Consts.IID_IMMDevice)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IMMDevice
{
    /// <summary>
    /// The Activate method creates a COM object with the specified interface.
    /// </summary>
    /// <param name="iid">The interface identifier. This parameter is a reference to a GUID that identifies the interface that the caller requests be activated.
    /// The caller will use this interface to communicate with the COM object. Set this parameter to one of the following interface identifiers:
    /// IID_IAudioClient, IID_IAudioEndpointVolume, IID_IAudioMeterInformation, IID_IAudioSessionManager, IID_IAudioSessionManager2,
    /// IID_IBaseFilter, IID_IDeviceTopology, IID_IDirectSound, IID_IDirectSound8, IID_IDirectSoundCapture, IID_IDirectSoundCapture8,
    /// IID_IMFTrustedOutput,,IID_ISpatialAudioClient, IID_ISpatialAudioMetadataClient</param>
    /// <param name="dwClsCtx">The execution context in which the code that manages the newly created object will run. The caller can restrict the context by setting this parameter to the bitwise OR of one or more CLSCTX enumeration values. Alternatively, the client can avoid imposing any context restrictions by specifying CLSCTX_ALL. </param>
    /// <param name="pActivationParams">Set to NULL to activate an IAudioClient, IAudioEndpointVolume, IAudioMeterInformation, IAudioSessionManager, or IDeviceTopology interface on an audio endpoint device. When activating an IBaseFilter, IDirectSound, IDirectSound8, IDirectSoundCapture, or IDirectSoundCapture8 interface on the device, the caller can specify a pointer to a PROPVARIANT structure that contains stream-initialization information.</param>
    /// <param name="ppInterface">Pointer to a pointer variable into which the method writes the address of the interface specified by parameter iid. Through this method, the caller obtains a counted reference to the interface. The caller is responsible for releasing the interface, when it is no longer needed, by calling the interface's Release method. If the Activate call fails, *ppInterface is NULL.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig]
    HRESULT Activate(ref Guid iid, CLSCTX dwClsCtx, IntPtr pActivationParams, [Out, MarshalAs(UnmanagedType.IUnknown)] out object? ppInterface);
    /// <summary>
    /// The OpenPropertyStore method retrieves an interface to the device's property store.
    /// </summary>
    /// <param name="stgmAccess">The storage-access mode. This parameter specifies whether to open the property store in read mode, write mode, or read/write mode. </param>
    /// <param name="propertyStore">Pointer to a pointer variable into which the method writes the address of the IPropertyStore interface of the device's property store. </param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig]
    HRESULT OpenPropertyStore(StgmAccess stgmAccess, out IPropertyStore? propertyStore);
    /// <summary>
    /// The GetId method retrieves an endpoint ID string that identifies the audio endpoint device.
    /// </summary>
    /// <param name="ppstrId">Pointer to a pointer variable into which the method writes the address of a null-terminated, wide-character string containing the endpoint device ID. The method allocates the storage for the string.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig]
    HRESULT GetId([Out, MarshalAs(UnmanagedType.LPWStr)] out string? ppstrId);
    /// <summary>
    /// The GetState method retrieves the current device state.
    /// </summary>
    /// <param name="pdwState">Pointer to a DWORD variable into which the method writes the current state of the device. The device-state value is one of the AudioDeviceState constants</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig]
    HRESULT GetState(out AudioDeviceState pdwState);
}