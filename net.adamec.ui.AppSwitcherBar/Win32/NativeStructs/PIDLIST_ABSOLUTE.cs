using System;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeStructs
{
    /// <summary>
    /// Fully qualified ITEMIDLIST relative to the root of the namespace. It may be multi-level.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct PIDLIST_ABSOLUTE
    {
        /// <summary>
        /// Pointer to fully qualified ITEMIDLIST relative to the root of the namespace. It may be multi-level.
        /// </summary>
        public IntPtr Ptr;
    }
}
