using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
using System;
using System.Runtime.InteropServices;
// ReSharper disable IdentifierTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeMethods
{
    /// <summary>
    /// Gdi32 Win32 Api
    /// </summary>
    internal class Gdi32
    {
        /// <summary>
        /// Name of the DLL
        /// </summary>
        private const string DLL_NAME = "gdi32.dll";

        /// <summary>Deletes a logical pen, brush, font, bitmap, region, or palette, freeing all system resources associated with the object. After the object is deleted, the specified handle is no longer valid.</summary>
        /// <param name="hObject">A handle to a logical pen, brush, font, bitmap, region, or palette.</param>
        /// <returns>
        ///   <para>If the function succeeds, the return value is nonzero.</para>
        ///   <para>If the specified handle is not valid or is currently selected into a DC, the return value is zero.</para>
        /// </returns>
        /// <remarks>
        ///   <para>Do not delete a drawing object (pen or brush) while it is still selected into a DC.</para>
        ///   <para>When a pattern brush is deleted, the bitmap associated with the brush is not deleted. The bitmap must be deleted independently.</para>
        /// </remarks>
        [DllImport(DLL_NAME, EntryPoint = "DeleteObject")]
        internal static extern HRESULT DeleteObject([In] IntPtr hObject);
    }
}
