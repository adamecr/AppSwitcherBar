using System;
using System.Runtime.InteropServices;
using System.Text;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeMethods
{
    /// <summary>
    /// Shlwapi Win32 Api
    /// </summary>
    // ReSharper disable once IdentifierTypo
    internal class ShlwApi
    {
        /// <summary>
        /// Name of the DLL
        /// </summary>
        private const string DLL_NAME = "shlwapi.dll";

        /// <summary>
        /// Extracts a specified text resource when given that resource in the form of an indirect string (a string that begins with the '@' symbol).
        /// </summary>
        /// <param name="pszSource">A pointer to a buffer that contains the indirect string from which the resource will be retrieved. This string should begin with the '@' symbol and use one of the forms discussed in the Remarks section. This function will successfully accept a string that does not begin with an '@' symbol, but the string will be simply passed unchanged to pszOutBuf.</param>
        /// <param name="pszOutBuf">A pointer to a buffer that, when this function returns successfully, receives the text resource. Both pszOutBuf and pszSource can point to the same buffer, in which case the original string will be overwritten.</param>
        /// <param name="cchOutBuf">The size of the buffer pointed to by pszOutBuf, in characters.</param>
        /// <param name="ppvReserved">Not used; set to NULL.</param>
        /// <returns>If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [DllImport(DLL_NAME, BestFitMapping = false, CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = false, ThrowOnUnmappableChar = true)]
        internal static extern HRESULT SHLoadIndirectString([MarshalAs(UnmanagedType.LPWStr)] string pszSource, StringBuilder pszOutBuf, int cchOutBuf, IntPtr ppvReserved);

    }
}
