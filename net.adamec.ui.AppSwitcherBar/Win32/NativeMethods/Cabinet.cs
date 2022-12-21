using System;
using System.Runtime.InteropServices;

// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Local

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeMethods
{
    /// <summary>
    /// Cabinet Win32 Api
    /// </summary>
    internal class Cabinet
    {
        /// <summary>
        /// Name of the DLL
        /// </summary>
        private const string DLL_NAME = "Cabinet.dll";

        /// <summary>
        /// Generates a new DECOMPRESSOR_HANDLE.
        /// </summary>
        /// <param name="Algorithm">The type of compression algorithm and mode to be used by this decompressor.</param>
        /// <param name="AllocationRoutines">Optional memory allocation and deallocation routines in a COMPRESS_ALLOCATION_ROUTINES structure.</param>
        /// <param name="DecompressorHandle">If the function succeeds, the handle to the specified decompressor.</param>
        /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.</returns>
        [DllImport(DLL_NAME)]
        internal static extern bool CreateDecompressor(uint Algorithm, IntPtr AllocationRoutines, out IntPtr DecompressorHandle);

        /// <summary>
        /// Call to close an open DECOMPRESSOR_HANDLE.
        /// </summary>
        /// <param name="DecompressorHandle">Handle to the decompressor to be closed. This is the handle to the compressor that was returned by CreateDecompressor.</param>
        /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.</returns>
        [DllImport(DLL_NAME)]
        internal static extern bool CloseDecompressor(IntPtr DecompressorHandle);

        /// <summary>
        /// Takes a block of compressed information and decompresses it.
        /// </summary>
        /// <param name="DecompressorHandle">Handle to a decompressor returned by CreateDecompressor.</param>
        /// <param name="CompressedData">Contains the block of information that is to be decompressed. The size in bytes of the compressed block is given by CompressedDataSize.</param>
        /// <param name="CompressedDataSize">The size in bytes of the compressed information.</param>
        /// <param name="UncompressedBuffer">The buffer that receives the uncompressed information. The size in bytes of the buffer is given by UncompressedBufferSize.</param>
        /// <param name="UncompressedBufferSize">Size in bytes of the buffer that receives the uncompressed information.</param>
        /// <param name="UncompressedDataSize">Actual size in bytes of the uncompressed information received.</param>
        /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.</returns>
        [DllImport(DLL_NAME)]
        internal static extern bool Decompress(IntPtr DecompressorHandle, byte[] CompressedData, uint CompressedDataSize, byte[] UncompressedBuffer, uint UncompressedBufferSize, out uint UncompressedDataSize);

    }
}
