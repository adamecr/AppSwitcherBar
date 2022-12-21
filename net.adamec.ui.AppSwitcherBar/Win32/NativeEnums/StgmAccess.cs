
// ReSharper disable IdentifierTypo
// ReSharper disable UnusedMember.Global

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeEnums
{
    /// <summary>
    /// The storage-access mode.
    /// </summary>
    internal enum StgmAccess
    {
        /// <summary>
        /// Read mode
        /// </summary>
        STGM_READ = 0x00000000,
        /// <summary>
        /// Write mode
        /// </summary>
        STGM_WRITE = 0x00000001,
        /// <summary>
        /// Read/Write mode
        /// </summary>
        STGM_READWRITE = 0x00000002
    }
}
