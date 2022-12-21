using System.Runtime.InteropServices;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces;

/// <summary>
/// The IMMDeviceCollection interface represents a collection of multimedia device resources. In the current implementation, the only device resources that the MMDevice API can create collections of are audio endpoint devices.
/// </summary>
[ComImport]
[Guid(Win32Consts.IID_IMMDeviceCollection)]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IMMDeviceCollection
{
    /// <summary>
    /// The GetCount method retrieves a count of the devices in the device collection.
    /// </summary>
    /// <param name="pcDevices">Pointer to a UINT variable into which the method writes the number of devices in the device collection.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig]
    HRESULT GetCount(out uint pcDevices);

    /// <summary>
    /// The Item method retrieves a pointer to the specified item in the device collection.
    /// </summary>
    /// <param name="nDevice">The device number. If the collection contains n devices, the devices are numbered 0 to n– 1.</param>
    /// <param name="device">Pointer to a pointer variable into which the method writes the address of the IMMDevice interface of the specified item in the device collection. Through this method, the caller obtains a counted reference to the interface. The caller is responsible for releasing the interface, when it is no longer needed, by calling the interface's Release method. If the Item call fails, *ppDevice is NULL.</param>
    /// <returns>If the method succeeds, it returns S_OK.</returns>
    [PreserveSig]
    HRESULT Item(uint nDevice, out IMMDevice? device);
}