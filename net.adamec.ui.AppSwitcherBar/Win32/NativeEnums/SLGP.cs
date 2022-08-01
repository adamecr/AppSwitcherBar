using System;
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeEnums
{
    /// <summary>IShellLinkW::GetPath flags.  SLGP_*</summary>
    [Flags]
    internal enum SLGP
    {
        /// <summary>
        /// Retrieves the standard short (8.3 format) file name.
        /// </summary>
        SHORTPATH = 0x1,
        /// <summary>
        /// Unsupported; do not use.
        /// </summary>
        UNCPRIORITY = 0x2,
        /// <summary>
        /// Retrieves the raw path name. A raw path is something that might not exist and may include environment variables that need to be expanded.
        /// </summary>
        RAWPATH = 0x4
    }
}
