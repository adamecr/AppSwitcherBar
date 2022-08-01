using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
// ReSharper disable InconsistentNaming
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeStructs
{
    /// <summary>
    /// Contains information about the file that is found by the FindFirstFile, FindFirstFileEx, or FindNextFile function.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    [BestFitMapping(false)]
    internal struct WIN32_FIND_DATAW
    {
        /// <summary>
        /// The file attributes of a file.
        /// </summary>
        public FileAttributes dwFileAttributes;
        /// <summary>
        /// A FILETIME structure that specifies when a file or directory was created.
        /// If the underlying file system does not support creation time, this member is zero.
        /// </summary>
        public FILETIME ftCreationTime;
        /// <summary>
        /// For a file, the structure specifies when the file was last read from, written to, or for executable files, run.
        /// For a directory, the structure specifies when the directory is created.If the underlying file system does not support last access time, this member is zero.
        /// On the FAT file system, the specified date for both files and directories is correct, but the time of day is always set to midnight.
        /// </summary>
        public FILETIME ftLastAccessTime;
        /// <summary>
        /// For a file, the structure specifies when the file was last written to, truncated, or overwritten, for example, when WriteFile or SetEndOfFile are used. The date and time are not updated when file attributes or security descriptors are changed.
        /// For a directory, the structure specifies when the directory is created.If the underlying file system does not support last write time, this member is zero.
        /// </summary>
        public FILETIME ftLastWriteTime;
        /// <summary>
        /// The high-order DWORD value of the file size, in bytes.
        /// This value is zero unless the file size is greater than MAXDWORD.
        /// The size of the file is equal to(nFileSizeHigh* (MAXDWORD+1)) + nFileSizeLow.
        /// </summary>
        public int nFileSizeHigh;
        /// <summary>
        /// The low-order DWORD value of the file size, in bytes.
        /// </summary>
        public int nFileSizeLow;
        /// <summary>
        /// If the dwFileAttributes member includes the FILE_ATTRIBUTE_REPARSE_POINT attribute, this member specifies the reparse point tag.
        /// Otherwise, this value is undefined and should not be used.
        /// </summary>
        public int dwReserved0;
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        public int dwReserved1;
        /// <summary>
        /// The name of the file.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string cFileName;
        /// <summary>
        /// An alternative name for the file.This name is in the classic 8.3 file name format.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
        public string cAlternateFileName;
    }

}
