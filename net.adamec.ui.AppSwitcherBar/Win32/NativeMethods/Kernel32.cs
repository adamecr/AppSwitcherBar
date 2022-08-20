using System;
using System.Runtime.InteropServices;
using System.Text;
// ReSharper disable IdentifierTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeMethods
{
    /// <summary>
    /// Kernel32 Win32 Api
    /// </summary>
    internal class Kernel32
    {
        /// <summary>
        /// Name of the DLL
        /// </summary>
        private const string DLL_NAME = "Kernel32.dll";

        /// <summary>
        /// Opens an existing local process object.
        /// </summary>
        /// <param name="dwDesiredAccess">The access to the process object. This access right is checked against the security descriptor for the process. This parameter can be one or more of the process access rights.
        /// If the caller has enabled the SeDebugPrivilege privilege, the requested access is granted regardless of the contents of the security descriptor.</param>
        /// <param name="bInheritHandle">If this value is TRUE, processes created by this process will inherit the handle. Otherwise, the processes do not inherit this handle.</param>
        /// <param name="dwProcessId">The identifier of the local process to be opened.</param>
        /// <returns>If the function succeeds, the return value is an open handle to the specified process.
        /// If the function fails, the return value is NULL.</returns>
        [DllImport(DLL_NAME)]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        /// <summary>
        /// Closes an open object handle.
        /// </summary>
        /// <param name="hHandle">A valid handle to an open object.</param>
        /// <returns>If the function succeeds, the return value is nonzero.If the function fails, the return value is zero</returns>
        [DllImport(DLL_NAME)]
        public static extern bool CloseHandle(IntPtr hHandle);

        /// <summary>
        /// Retrieves the full name of the executable image for the specified process.
        /// </summary>
        /// <param name="hProcess">A handle to the process. This handle must be created with the PROCESS_QUERY_INFORMATION or PROCESS_QUERY_LIMITED_INFORMATION access right.</param>
        /// <param name="dwFlags">This parameter can be one of the following values. 0 - The name should use the Win32 path format. 1 - The name should use the native system path format. </param>
        /// <param name="lpExeName">The path to the executable image. If the function succeeds, this string is null-terminated.</param>
        /// <param name="lpdwSize">On input, specifies the size of the lpExeName buffer, in characters. On success, receives the number of characters written to the buffer, not including the null-terminating character.</param>
        /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.</returns>
        [DllImport(DLL_NAME, SetLastError = true)]
        internal static extern bool QueryFullProcessImageName([In] IntPtr hProcess, [In] int dwFlags, [Out] StringBuilder lpExeName, ref int lpdwSize);

        /// <summary>
        /// Gets the application user model ID for the specified process.
        /// </summary>
        /// <param name="hProcess">A handle to the process. This handle must have the PROCESS_QUERY_LIMITED_INFORMATION access right.</param>
        /// <param name="appModelIdLength">On input, the size of the applicationUserModelId buffer, in wide characters. On success, the size of the buffer used, including the null terminator.</param>
        /// <param name="sbAppUserModelId">A buffer that receives the application user model ID.</param>
        /// <returns></returns>
        [DllImport(DLL_NAME, SetLastError = true)]
        internal static extern int GetApplicationUserModelId(IntPtr hProcess, ref uint appModelIdLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder sbAppUserModelId);


        /// <summary>
        /// Loads the specified module into the address space of the calling process. The specified module may cause other modules to be loaded.
        /// </summary>
        /// <param name="path">The name of the module. This can be either a library module (a .dll file) or an executable module (an .exe file).</param>
        /// <returns>If the function succeeds, the return value is a handle to the module.
        /// If the function fails, the return value is NULL. </returns>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr LoadLibrary(string path);

        /// <summary>
        /// Frees the loaded dynamic-link library (DLL) module and, if necessary, decrements its reference count.
        /// When the reference count reaches zero, the module is unloaded from the address space of the calling process and the handle is no longer valid.
        /// </summary>
        /// <param name="hInst">A handle to the loaded library module. </param>
        /// <returns>f the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero.</returns>
        [DllImport(DLL_NAME, CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool FreeLibrary(IntPtr hInst);
        
        /// <summary>
        /// Gets the path of the specified package.
        /// </summary>
        /// <param name="packageFullName">The full name of the package.</param>
        /// <param name="pathLength"> A pointer to a variable that holds the number of characters in the package path string, which includes the null-terminator.
        /// First you pass NULL to path to get the number of characters. You use this number to allocate memory space for path.
        /// Then you pass the address of this memory space to fill path. </param>
        /// <param name="path">A pointer to memory space that receives the package path string, which includes the null-terminator. </param>
        /// <returns> If the function succeeds it returns ERROR_SUCCESS Otherwise, the function returns an error code. </returns>
        [DllImport(DLL_NAME, SetLastError = false, ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern long GetPackagePathByFullName(string packageFullName, ref uint pathLength, [Optional] StringBuilder path);


        /// <summary>
        /// Retrieves the short path form of the specified path.
        /// </summary>
        /// <param name="path">The path string.</param>
        /// <param name="shortPath">A pointer to a buffer to receive the null-terminated short form of the path that lpszLongPath specifies.</param>
        /// <param name="shortPathLength">The size of the buffer that lpszShortPath points to, in TCHARs.</param>
        /// <returns>If the function succeeds, the return value is the length, in TCHARs, of the string that is copied to lpszShortPath, not including the terminating null character.
        /// If the lpszShortPath buffer is too small to contain the path, the return value is the size of the buffer, in TCHARs, that is required to hold the path and the terminating null character.
        /// If the function fails for any other reason, the return value is zero.</returns>
        [DllImport(DLL_NAME, CharSet = CharSet.Auto)]
        public static extern int GetShortPathName([MarshalAs(UnmanagedType.LPTStr)] string path, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder shortPath, int shortPathLength);
    }
}
