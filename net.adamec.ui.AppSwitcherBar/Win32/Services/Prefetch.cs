using System;
using System.Diagnostics;
using System.IO;
using net.adamec.ui.AppSwitcherBar.Dto;
using net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;
using net.adamec.ui.AppSwitcherBar.Win32.NativeMethods;

namespace net.adamec.ui.AppSwitcherBar.Win32.Services
{
    /// <summary>
    /// Helper class for retrieving Windows prefetch information
    /// </summary>
    public class Prefetch
    {
        /// <summary>
        /// Retrieves the windows prefetch information for package.
        /// It first tries to get the package and application executable from package manifest (<see cref="Package.GetPackageExecutable"/>) and
        /// then to retrieve the prefetch info for the executable (<see cref="GetPrefetchInfoNoHash(string)"/>
        /// </summary>
        /// <param name="packageInstallPath">Package installation path (directory)</param>
        /// <param name="packageName">Package name (including the !AppName)</param>
        /// <returns>Windows prefetch information for package or null if not available</returns>
        public static RunStats? GetPrefetchInfoNoHash(string packageInstallPath, string packageName)
        {
            var packageExec = Package.GetPackageExecutable(packageInstallPath, packageName);
            if (packageExec == null) return null;

            return GetPrefetchInfoNoHash(packageExec);
        }

        /// <summary>
        /// Retrieves the windows prefetch information for <paramref name="executableFile"/> if available
        /// It checks for all available prefetch files for executable and returns the <see cref="RunStats"/> with
        /// the last launch date (the latest from all prefetch files that match the executable name) and
        /// the total count of runs (from all prefetch files that match the executable name)
        /// </summary>
        /// <param name="executableFile">File to check for prefetch information</param>
        /// <returns>Windows prefetch information for <paramref name="executableFile"/> or null if not available</returns>
        public static RunStats? GetPrefetchInfoNoHash(string executableFile)
        {
            if (!File.Exists(executableFile)) return null;

            var executable = Path.GetFileName(executableFile);
            if (executable.Length > 29) executable = executable[..29]; //truncate to 29 chars

            //Get prefetch files 
            var prefetchFileMask = $@"{executable.ToUpper()}-????????.pf";
            var prefetchDir = new DirectoryInfo(@"C:\Windows\Prefetch");
            if (!prefetchDir.Exists) return null;

            FileInfo[] prefetchFiles;

            try
            {
                prefetchFiles = prefetchDir.GetFiles(prefetchFileMask);
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Can't get the prefetch files for {executableFile} - {ex.GetType().Name}: {ex.Message}");
#endif
                return null;
            }

            var lastRun = DateTime.MinValue;
            var count = 0;
            foreach (var prefetchFileInfo in prefetchFiles)
            {
                if (!prefetchFileInfo.Exists) continue;
                var prefetchFile = prefetchFileInfo.FullName;

                //Read prefetch file header
                var prefetchFileBytes = File.ReadAllBytes(prefetchFile);

                var fileHeader = new byte[4]; //MAM\x04
                Array.Copy(prefetchFileBytes, fileHeader, 4);

                var decompressedDataSizeArray = new byte[4];
                Array.Copy(prefetchFileBytes, 4, decompressedDataSizeArray, 0, 4);
                var decompressedDataSize = BitConverter.ToInt32(decompressedDataSizeArray);

                var compressedData = new byte[prefetchFileBytes.Length - 8];
                Array.Copy(prefetchFileBytes, 8, compressedData, 0, compressedData.Length);

                //Decompress the prefetch file data
                // ReSharper disable once IdentifierTypo
                if (!Cabinet.CreateDecompressor((uint)(COMPRESS_ALGORITHM.COMPRESS_ALGORITHM_XPRESS_HUFF | COMPRESS_ALGORITHM.COMPRESS_RAW), IntPtr.Zero, out var decompressor) ||
                    decompressor == IntPtr.Zero) return null;

                var buffer = new byte[decompressedDataSize];
                if (!Cabinet.Decompress(
                        decompressor,
                        compressedData, (uint)compressedData.Length,
                        buffer, (uint)buffer.Length,
                        out uint realDecompressedDataSize) || realDecompressedDataSize < 84 + 224) return null; //?should I CloseDecompressor

                Cabinet.CloseDecompressor(decompressor);

                //Extract prefetch file data

                //header - 84 bytes
                //var header = new byte[84];
                //Array.Copy(buffer, header, header.Length);

                //info block - ~224 bytes (size can differ a bit, but we need just a part anyway)
                var infoBlock = new byte[224];
                Array.Copy(buffer, 84, infoBlock, 0, infoBlock.Length);

                var metricsOffset = new byte[4];
                Array.Copy(infoBlock, 0, metricsOffset, 0, metricsOffset.Length);
                var metricsOffsetVal = BitConverter.ToInt32(metricsOffset);

                if (metricsOffsetVal == 296) //just to make sure it's the right schema, the offset itself is not used
                {
                    //last execution time - offset 44 in info
                    var fileTime = new byte[8];
                    Array.Copy(infoBlock, 44, fileTime, 0, fileTime.Length);
                    var fileTimeVal = BitConverter.ToInt64(fileTime);
                    var fileTimeDate = DateTime.FromFileTime(fileTimeVal);

                    //last execution time - offset 116 in info
                    var runCount = new byte[4];
                    Array.Copy(infoBlock, 116, runCount, 0, runCount.Length);
                    var runCountVal = BitConverter.ToInt32(runCount);

                    if (lastRun < fileTimeDate) lastRun = fileTimeDate; //get the latest
                    count += runCountVal;//additive counter
                }
            }
#if DEBUG
            // Debug.WriteLine($"PREFETCH: #{count}/{lastRun:dd.MM.yy HHmm} for {executable}");
#endif
            return new RunStats().UpdateRunInfo(lastRun, count);
        }
    }
}
