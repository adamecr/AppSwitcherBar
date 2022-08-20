using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;
using net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable UnusedMember.Global

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeMethods
{
    /// <summary>
    /// Shell32 Win32 Api
    /// </summary>
    internal class Shell32
    {
        /// <summary>
        /// Name of the DLL
        /// </summary>
        private const string DLL_NAME = "Shell32.dll";

        /// <summary>
        /// Sends an appbar message to the system.
        /// </summary>
        /// <param name="dwMessage">Appbar message value to send. This parameter can be one of the values from <see cref="ABMsg"/>.</param>
        /// <param name="pData">A pointer to an <see cref="APPBARDATA"/> structure. The content of the structure on entry and on exit depends on the value set in the <paramref name="dwMessage"/> parameter</param>
        /// <returns>This function returns a message-dependent value</returns>
        [DllImport(DLL_NAME, ExactSpelling = true)]
        internal static extern uint SHAppBarMessage(ABMsg dwMessage, ref APPBARDATA pData);

        /// <summary>
        /// Queries the dots per inch (dpi) of a display.
        /// </summary>
        /// <param name="hMonitor">Handle of the monitor being queried.</param>
        /// <param name="dpiType">The type of DPI being queried. Possible values are from the <see cref="MONITOR_DPI_TYPE"/> enumeration.</param>
        /// <param name="dpiX">The value of the DPI along the X axis. This value always refers to the horizontal edge, even when the screen is rotated.</param>
        /// <param name="dpiY">The value of the DPI along the Y axis. This value always refers to the vertical edge, even when the screen is rotated.</param>
        [DllImport(DLL_NAME, ExactSpelling = true, PreserveSig = false)]
        internal static extern void GetDpiForMonitor(IntPtr hMonitor, MONITOR_DPI_TYPE dpiType, out int dpiX, out int dpiY);

        /// <summary>
        /// Retrieves the full path of a known folder identified by the folder's KNOWNFOLDERID.
        /// </summary>
        /// <param name="rfid">A GUID of the KNOWNFOLDERID that identifies the folder.</param>
        /// <param name="dwFlags">Flags that specify special retrieval options. This value can be 0; otherwise, one or more of the KNOWN_FOLDER_FLAG values.</param>
        /// <param name="hToken">An access token that represents a particular user. If this parameter is NULL, which is the most common usage, the function requests the known folder for the current user.
        /// Request a specific user's folder by passing the hToken of that user.
        /// Assigning the hToken parameter a value of -1 indicates the Default User.</param>
        /// <returns>When this method returns, contains the address of a pointer to a null-terminated Unicode string that specifies the path of the known folder.
        /// The calling process is responsible for freeing this resource once it is no longer needed by calling CoTaskMemFree, whether SHGetKnownFolderPath succeeds or not. </returns>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
        internal static extern string SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken = default);

        /// <summary>
        /// Retrieves an object that represents a specific window's collection of properties, which allows those properties to be queried or set.
        /// </summary>
        /// <param name="handle">A handle to the window whose properties are being retrieved.</param>
        /// <param name="riid">A reference to the IID (interface GUID) of the property store object to retrieve through <paramref name="ppv"/>.
        /// This is typically <see cref="Win32Consts.IID_IPropertyStore"/>.</param>
        /// <param name="ppv">When this function returns, contains the interface pointer requested in <paramref name="riid"/>. </param>
        /// <returns></returns>
        [DllImport(DLL_NAME, SetLastError = true)]
        internal static extern HRESULT SHGetPropertyStoreForWindow(IntPtr handle, ref Guid riid, out IPropertyStore ppv);

        /// <summary>
        /// Retrieves the full path of a known folder identified by the folder's KNOWNFOLDERID. This extends SHGetKnownFolderPath by allowing you to set the initial size of the string buffer.
        /// </summary>
        /// <param name="rfid">A reference to the KNOWNFOLDERID that identifies the folder.</param>
        /// <param name="dwFlags">Flags that specify special retrieval options. This value can be 0; otherwise, one or more of the KNOWN_FOLDER_FLAG values.</param>
        /// <param name="hToken">An access token that represents a particular user. If this parameter is NULL, which is the most common usage, the function requests the known folder for the current user.</param>
        /// <param name="pszPath">A null-terminated, Unicode string. This buffer must be of size cchPath. When SHGetFolderPathEx returns successfully, this parameter contains the path for the known folder.</param>
        /// <param name="cchPath">The size of the ppszPath buffer, in characters.</param>
        /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
        [DllImport(DLL_NAME)]
        internal static extern HRESULT SHGetFolderPathEx([In] ref Guid rfid, KF_FLAG dwFlags, [In, Optional] IntPtr hToken, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszPath, uint cchPath);

        /// <summary>
        /// Retrieves an IShellItem object that represents a known folder.
        /// </summary>
        /// <param name="rfid">A reference to the KNOWNFOLDERID, a GUID that identifies the folder that contains the item.</param>
        /// <param name="dwFlags">Flags that specify special options used in the retrieval of the known folder IShellItem. This value can be KF_FLAG_DEFAULT; otherwise, one or more of the KNOWN_FOLDER_FLAG values.</param>
        /// <param name="hToken">An access token used to represent a particular user. This parameter is usually set to NULL, in which case the function tries to access the current user's instance of the folder. However, you may need to assign a value to hToken for those folders that can have multiple users but are treated as belonging to a single user. The most commonly used folder of this type is Documents.</param>
        /// <param name="riid">A reference to the IID of the interface that represents the item, usually IID_IShellItem or IID_IShellItem2.</param>
        /// <param name="ppv">When this method returns, contains the interface pointer requested in riid.</param>
        /// <returns>Returns S_OK if successful, or an error value otherwise</returns>
        [DllImport(DLL_NAME, ExactSpelling = true, SetLastError = false)]
        public static extern HRESULT SHGetKnownFolderItem(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid rfid,
            uint dwFlags,
            IntPtr hToken,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            out IShellItem ppv);

        /// <summary>
        /// Retrieves information about an object in the file system, such as a file, folder, directory, or drive root.
        /// </summary>
        /// <param name="pszPath">A pointer to a null-terminated string of maximum length MAX_PATH that contains the path and file name. Both absolute and relative paths are valid.</param>
        /// <param name="dwFileAttributes">A combination of one or more file attribute flags (FILE_ATTRIBUTE_ values as defined in Winnt.h). If uFlags does not include the SHGFI_USEFILEATTRIBUTES flag, this parameter is ignored.</param>
        /// <param name="psfi">Pointer to a SHFILEINFO structure to receive the file information.</param>
        /// <param name="cbfileInfo">The size, in bytes, of the SHFILEINFO structure pointed to by the psfi parameter.</param>
        /// <param name="uFlags">The flags that specify the file information to retrieve.</param>
        /// <returns>Returns a value whose meaning depends on the uFlags parameter.</returns>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode)]
        internal static extern int SHGetFileInfo(string pszPath, int dwFileAttributes, out SHFILEINFO psfi, uint cbfileInfo, SHGFI uFlags);


        /// <summary>
        /// Creates and initializes a Shell item object from a parsing name.
        /// </summary>
        /// <param name="pszPath">A pointer to a display name.</param>
        /// <param name="pbc">Optional. A pointer to a bind context used to pass parameters as inputs and outputs to the parsing function. </param>
        /// <param name="riid">A reference to the IID of the interface to retrieve through ppv, typically IID_IShellItem or IID_IShellItem2.</param>
        /// <param name="ppv">When this method returns successfully, contains the interface pointer requested in riid. This is typically IShellItem or IShellItem2.</param>
        /// <returns>If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [DllImport(DLL_NAME)]
        internal static extern HRESULT SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string pszPath, IBindCtx? pbc, [In] ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppv);

        /// <summary>
        /// Creates and initializes a Shell item object from a parsing name.
        /// </summary>
        /// <param name="pszPath">A pointer to a display name.</param>
        /// <param name="pbc">Optional. A pointer to a bind context used to pass parameters as inputs and outputs to the parsing function. </param>
        /// <param name="iIdIShellItem2">A reference to the IID of the interface to retrieve through ppv - IID_IShellItem2.</param>
        /// <param name="iShellItem">When this method returns successfully, contains the interface pointer requested in riid. This is IShellItem2.</param>
        /// <returns>If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>

        [DllImport(DLL_NAME, CharSet = CharSet.Unicode, PreserveSig = false)]
        internal static extern HRESULT SHCreateItemFromParsingName(
            [In][MarshalAs(UnmanagedType.LPWStr)] string pszPath,
            [In] IntPtr pbc,
            [In][MarshalAs(UnmanagedType.LPStruct)] Guid iIdIShellItem2,
            [Out][MarshalAs(UnmanagedType.Interface, IidParameterIndex = 2)] out IShellItem2 iShellItem);


        /// <summary>
        /// The ExtractIconEx function creates an array of handles to large or small icons extracted from the specified executable file, DLL, or icon file.
        /// When they are no longer needed, you must destroy all icons extracted by ExtractIconEx by calling the DestroyIcon function.
        /// </summary>
        /// <param name="szFileName">Pointer to a null-terminated string that specifies the name of an executable file, DLL, or icon file from which icons will be extracted.</param>
        /// <param name="nIconIndex">
        /// Specifies the zero-based index of the first icon to extract.
        /// If this value is zero, the function extracts the first icon in the specified file.
        /// If this value is –1 and phiconLarge and phiconSmall are both NULL, the function returns the total number of icons in the specified file. If the file is an executable file or DLL, the return value is the number of RT_GROUP_ICON resources.If the file is an.ico file, the return value is 1.
        /// If this value is a negative number and either phiconLarge or phiconSmall is not NULL, the function begins by extracting the icon whose resource identifier is equal to the absolute value of nIconIndex.
        /// For example, use -3 to extract the icon whose resource identifier is 3.
        /// </param>
        /// <param name="phiconLarge">
        /// Pointer to an array of icon handles that receives handles to the large icons extracted from the file.
        /// If this parameter is NULL, no large icons are extracted from the file.</param>
        /// <param name="phiconSmall">
        /// Pointer to an array of icon handles that receives handles to the small icons extracted from the file.
        /// If this parameter is NULL, no small icons are extracted from the file.</param>
        /// <param name="nIcons">The number of icons to extract from the file.</param>
        /// <returns>
        /// If the nIconIndex parameter is -1 and both the phiconLarge and phiconSmall parameters are NULL, then the return value is the number of icons contained in the specified file.
        /// If the nIconIndex parameter is any value other than -1 and either phiconLarge or phiconSmall is not NULL, the return value is the number of icons successfully extracted from the file.
        /// If the function encounters an error, it returns UINT_MAX. In this case, you can call GetLastError to retrieve the error code.
        /// </returns>
        [DllImport(DLL_NAME, CharSet = CharSet.Unicode)]
        internal static extern uint ExtractIconEx(string szFileName, int nIconIndex, IntPtr[]? phiconLarge, IntPtr[]? phiconSmall, uint nIcons);


        /// <summary>
        /// Creates and initializes a Shell item object from a pointer to an item identifier list (PIDL). The resulting shell item object supports the IShellItem2 interface.
        /// </summary>
        /// <param name="pidl">The source PIDL.</param>
        /// <param name="riid">A reference to the IID of the requested interface.</param>
        /// <param name="ppv">When this function returns, contains the interface pointer requested in riid. This will typically be IShellItem or IShellItem2.</param>
        /// <returns>If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [DllImport(DLL_NAME, PreserveSig = true)]
        internal static extern HRESULT SHCreateItemFromIDList(
            [In] IntPtr pidl,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            [Out, MarshalAs(UnmanagedType.Interface, IidParameterIndex = 2)] out IShellItem2? ppv);

        /// <summary>
        /// Retrieves the display name of an item identified by its IDList.
        /// </summary>
        /// <param name="pidl">A PIDL that identifies the item.</param>
        /// <param name="sigdnName">A value from the SIGDN enumeration that specifies the type of display name to retrieve.</param>
        /// <param name="ppszName">A value that, when this function returns successfully, receives the address of a pointer to the retrieved display name.</param>
        /// <returns>If this function succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [DllImport(DLL_NAME, SetLastError = true, PreserveSig = true)]
        internal static extern HRESULT SHGetNameFromIDList([In] IntPtr pidl,
            [In] SIGDN sigdnName,
            [Out, MarshalAs(UnmanagedType.LPWStr)] out string ppszName);

        /// <summary>
        /// Returns the ITEMIDLIST structure associated with a specified file path. Call ILFree to release the ITEMIDLIST when you are finished with it.
        /// </summary>
        /// <param name="pszPath">A pointer to a null-terminated Unicode string that contains the path. This string should be no more than MAX_PATH characters in length, including the terminating null character.></param>
        /// <returns>Returns a pointer to an ITEMIDLIST structure that corresponds to the path.</returns>
        [DllImport(DLL_NAME, CharSet = CharSet.Auto)]
        internal static extern IntPtr ILCreateFromPath([In, MarshalAs(UnmanagedType.LPWStr)] string pszPath);
    }

}
