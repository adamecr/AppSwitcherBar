using System;
using System.Runtime.InteropServices;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;

// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces
{
    /// <summary>
    /// Enables clients to get pointers to other interfaces on a given object through the QueryInterface method, and manage the existence of the object through the AddRef and Release methods.
    /// All other COM interfaces are inherited, directly or indirectly, from IUnknown.
    /// Therefore, the three methods in IUnknown are the first entries in the vtable for every interface.
    /// </summary>
    [ComImport]
    [ComVisible(false)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid(Win32Consts.IID_IUnknown)]
    internal interface IUnknown
    {
        /// <summary>
        /// Queries a COM object for a pointer to one of its interface; identifying the interface by a reference to its interface identifier (IID).
        /// If the COM object implements the interface, then it returns a pointer to that interface after calling IUnknown::AddRef on it.
        /// </summary>
        /// <param name="riid">A reference to the interface identifier (IID) of the interface being queried for.</param>
        /// <param name="ppvObject">The address of a pointer to an interface with the IID specified in the riid parameter.
        /// Because you pass the address of an interface pointer, the method can overwrite that address with the pointer to the interface being queried for.
        /// Upon successful return, *ppvObject (the dereferenced address) contains a pointer to the requested interface.
        /// If the object doesn't support the interface, the method sets *ppvObject (the dereferenced address) to nullptr.</param>
        /// <returns>This method returns S_OK if the interface is supported, and E_NOINTERFACE otherwise. If ppvObject (the address) is nullptr, then this method returns E_POINTER.</returns>
        [PreserveSig]
        HRESULT QueryInterface(ref Guid riid, [Out] IntPtr ppvObject);

        /// <summary>
        /// Increments the reference count for an interface pointer to a COM object. You should call this method whenever you make a copy of an interface pointer
        /// </summary>
        /// <returns>The method returns the new reference count. This value is intended to be used only for test purposes.</returns>
        [PreserveSig]
        uint AddRef();

        /// <summary>
        /// Decrements the reference count for an interface on a COM object.
        /// </summary>
        /// <returns>The method returns the new reference count. This value is intended to be used only for test purposes.</returns>
        [PreserveSig]
        uint Release();
    }
}
