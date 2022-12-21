using System;

// ReSharper disable InconsistentNaming
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable UnusedMember.Global

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeEnums
{
    /// <summary>
    /// The type of compression algorithm and mode to be used by (de)compressor.
    /// </summary>
    [Flags]
    internal enum COMPRESS_ALGORITHM
    {
        /// <summary>
        /// MSZIP compression algorithm
        /// </summary>
        COMPRESS_ALGORITHM_MSZIP = 2,
        /// <summary>
        /// XPRESS compression algorithm
        /// </summary>
        COMPRESS_ALGORITHM_XPRESS = 3,
        /// <summary>
        /// XPRESS compression algorithm with Huffman encoding
        /// </summary>
        COMPRESS_ALGORITHM_XPRESS_HUFF = 4,
        /// <summary>
        /// LZMS compression algorithm
        /// </summary>
        COMPRESS_ALGORITHM_LZMS = 5,
        /// <summary>
        /// Used to create a block mode compressor
        /// </summary>
        COMPRESS_RAW = 1 << 29
    }
}
