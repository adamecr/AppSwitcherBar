using System;
using System.Runtime.InteropServices;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeMethods
{
    /// <summary>
    /// OleAut32 Win32 Api
    /// </summary>
    internal class OleAut32
    {
        /// <summary>
        /// Name of the DLL
        /// </summary>
        private const string DLL_NAME = "OleAut32.dll";

        /// <summary>
        /// Gets the number of dimensions in the array.
        /// </summary>
        /// <param name="psa">An array descriptor created by SafeArrayCreate.</param>
        /// <returns>The number of dimensions in the array.</returns>
        [DllImport(DLL_NAME, PreserveSig = true)]
        internal static extern uint SafeArrayGetDim(IntPtr psa);


        /// <summary>
        /// Gets the lower bound for any dimension of the specified safe array.
        /// </summary>
        /// <param name="psa">An array descriptor created by SafeArrayCreate.</param>
        /// <param name="nDim">The array dimension for which to get the lower bound.</param>
        /// <param name="lLBound">The lower bound.</param>
        /// <returns>This function can return one of these values S_OK (Success),E_INVALIDARG (One of the arguments is not valid.),DISP_E_BADINDEX (The specified index is out of bounds)</returns>
        [DllImport(DLL_NAME)]
        internal static extern HRESULT SafeArrayGetLBound(IntPtr psa, uint nDim, [Out] out int lLBound);

        /// <summary>
        /// Gets the upper bound for any dimension of the specified safe array.
        /// </summary>
        /// <param name="psa">An array descriptor created by SafeArrayCreate.</param>
        /// <param name="nDim">The array dimension for which to get the upper bound.</param>
        /// <param name="lUBound">The upper bound.</param>
        /// <returns>This function can return one of these values S_OK (Success), DISP_E_BADINDEX (The specified index is out of bounds), DISP_E_OVERFLOW (Overflow occurred while computing the upper bound), E_INVALIDARG</returns>
        [DllImport(DLL_NAME)]
        internal static extern HRESULT SafeArrayGetUBound(IntPtr psa, uint nDim, [Out] out int lUBound);

        /// <summary>
        /// Retrieves a single element of the array.
        /// This decl for SafeArrayGetElement is only valid for cDims==1! (the <paramref name="pv"/> is marshaled as single interface/object)
        /// </summary>
        /// <param name="psa">An array descriptor created by SafeArrayCreate.</param>
        /// <param name="rgIndices">A vector of indexes for each dimension of the array. The right-most (least significant) dimension is rgIndices[0]. The left-most dimension is stored at rgIndices[psa->cDims – 1].</param>
        /// <param name="pv">The element of the array.</param>
        /// <returns>This function can return one of these values S_OK (Success), DISP_E_BADINDEX (The specified index is not valid), E_INVALIDARG (One of the arguments is not valid), E_OUTOFMEMORY (Memory could not be allocated for the element)</returns>
        [DllImport(DLL_NAME)]
        internal static extern HRESULT SafeArrayGetElement(IntPtr psa, ref int rgIndices, [Out, MarshalAs(UnmanagedType.IUnknown)] out object? pv);
    }
}
