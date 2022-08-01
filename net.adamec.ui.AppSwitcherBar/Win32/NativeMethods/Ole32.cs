using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
using net.adamec.ui.AppSwitcherBar.Win32.Services.Shell.Properties;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeMethods
{
    /// <summary>
    /// Ole32 Win32 Api
    /// </summary>
    internal class Ole32
    {
        /// <summary>
        /// Name of the DLL
        /// </summary>
        private const string DLL_NAME = "ole32.dll";

        /// <summary>
        /// Loads an object from the stream.
        /// </summary>
        /// <param name="pStm">Pointer to the IStream interface on the stream from which the object is to be loaded.</param>
        /// <param name="riid">Interface identifier (IID) the caller wants to use to communicate with the object after it is loaded.</param>
        /// <param name="ppvObj">Address of pointer variable that receives the interface pointer requested in riid. Upon successful return, *ppvObj contains the requested interface pointer on the newly loaded object.</param>
        /// <returns>This function returns S_OK on success. Other possible values include the following:
        /// E_OUTOFMEMORY (Insufficient memory for the operation), E_NOINTERFACE (The object does not support the specified interface).</returns>
        [DllImport(DLL_NAME)]
        public static extern HRESULT OleLoadFromStream(IStream pStm, [In] ref Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppvObj);

        /// <summary>
        /// Clears a PROPVARIANT structure.
        /// </summary>
        /// <param name="pvar">Pointer to the PROPVARIANT structure to clear. When this function successfully returns, the PROPVARIANT is zeroed and the type is set to VT_EMPTY.</param>
        /// <returns>If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [DllImport(DLL_NAME)]
        internal static extern HRESULT PropVariantClear([In, Out] PropVariant pvar);
    }
}
