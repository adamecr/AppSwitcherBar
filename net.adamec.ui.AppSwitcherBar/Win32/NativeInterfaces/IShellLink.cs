using System;
using System.Runtime.InteropServices;
using System.Text;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;

// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces
{
    /// <summary>
    /// Exposes methods that create, modify, and resolve Shell links.
    /// </summary>
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid(Win32Consts.IID_IShellLink)]
    internal interface IShellLinkW
    {
        /// <summary>
        /// Gets the path and file name of the target of a Shell link object.
        /// </summary>
        /// <param name="pszFile">The address of a buffer that receives the path and file name of the target of the Shell link object.</param>
        /// <param name="cchMaxPath">The size, in characters, of the buffer pointed to by the pszFile parameter, including the terminating null character. The maximum path size that can be returned is MAX_PATH. This parameter is commonly set by calling ARRAYSIZE(pszFile). The ARRAYSIZE macro is defined in Winnt.h.</param>
        /// <param name="pfd">A pointer to a WIN32_FIND_DATA structure that receives information about the target of the Shell link object. If this parameter is NULL, then no additional information is returned.</param>
        /// <param name="fFlags">Flags that specify the type of path information to retrieve. This parameter can be a combination of the following values.</param>
        /// <returns>Returns S_OK if the operation is successful and a valid path is retrieved. If the operation is successful but no path is retrieved, it returns S_FALSE and pszFile will be empty. Otherwise, it returns one of the standard HRESULT error values.</returns>
        [PreserveSig]
        HRESULT GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, [In, Out] WIN32_FIND_DATAW pfd, SLGP fFlags);

        /// <summary>
        /// Gets the list of item identifiers for the target of a Shell link object.
        /// </summary>
        /// <returns>When this method returns, contains the address of a PIDL.</returns>
        IntPtr GetIDList();

        /// <summary>
        /// Sets the pointer to an item identifier list (PIDL) for a Shell link object.
        /// </summary>
        /// <param name="pidl">The object's fully qualified PIDL.</param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [PreserveSig]
        HRESULT SetIDList(IntPtr pidl);

        /// <summary>
        /// Gets the description string for a Shell link object.
        /// </summary>
        /// <param name="pszName">A pointer to the buffer that receives the description string.</param>
        /// <param name="cchMaxName">The maximum number of characters to copy to the buffer pointed to by the pszName parameter.</param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [PreserveSig]
        HRESULT GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);

        /// <summary>
        /// Sets the description for a Shell link object. The description can be any application-defined string.
        /// </summary>
        /// <param name="pszName">A pointer to a buffer containing the new description string.</param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [PreserveSig]
        HRESULT SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);

        /// <summary>
        /// Gets the name of the working directory for a Shell link object.
        /// </summary>
        /// <param name="pszDir">The address of a buffer that receives the name of the working directory.</param>
        /// <param name="cchMaxPath">The maximum number of characters to copy to the buffer pointed to by the pszDir parameter. The name of the working directory is truncated if it is longer than the maximum specified by this parameter.</param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [PreserveSig]
        HRESULT GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);

        /// <summary>
        /// Sets the name of the working directory for a Shell link object.
        /// </summary>
        /// <param name="pszDir">The address of a buffer that contains the name of the new working directory.</param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [PreserveSig]
        HRESULT SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);

        /// <summary>
        /// Gets the command-line arguments associated with a Shell link object.
        /// </summary>
        /// <param name="pszArgs">A pointer to the buffer that, when this method returns successfully, receives the command-line arguments.</param>
        /// <param name="cchMaxPath">The maximum number of characters that can be copied to the buffer supplied by the pszArgs parameter. In the case of a Unicode string, there is no limitation on maximum string length. In the case of an ANSI string, the maximum length of the returned string varies depending on the version of Windows—MAX_PATH prior to Windows 2000 and INFOTIPSIZE (defined in Commctrl.h) in Windows 2000 and later.</param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [PreserveSig]
        HRESULT GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);

        /// <summary>
        /// Sets the command-line arguments for a Shell link object.
        /// </summary>
        /// <param name="pszArgs">A pointer to a buffer that contains the new command-line arguments. In the case of a Unicode string, there is no limitation on maximum string length. In the case of an ANSI string, the maximum length of the returned string varies depending on the version of Windows—MAX_PATH prior to Windows 2000 and INFOTIPSIZE (defined in Commctrl.h) in Windows 2000 and later.</param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [PreserveSig]
        HRESULT SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);

        /// <summary>
        /// Gets the keyboard shortcut (hot key) for a Shell link object.
        /// </summary>
        /// <returns>The keyboard shortcut. The virtual key code is in the low-order byte, and the modifier flags are in the high-order byte. The modifier flags can be a combination of the following values.</returns>
        short GetHotKey();

        /// <summary>
        /// Sets a keyboard shortcut (hot key) for a Shell link object.
        /// </summary>
        /// <param name="wHotKey">The new keyboard shortcut. The virtual key code is in the low-order byte, and the modifier flags are in the high-order byte. The modifier flags can be a combination of the values specified in the description of the IShellLink::GetHotkey method.</param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [PreserveSig]
        HRESULT SetHotKey(short wHotKey);

        /// <summary>
        /// Gets the show command for a Shell link object.
        /// </summary>
        /// <returns>The show command for a Shell link object</returns>
        uint GetShowCmd();

        /// <summary>
        /// Sets the show command for a Shell link object. The show command sets the initial show state of the window.
        /// </summary>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        /// <param name="iShowCmd">Command. SetShowCmd accepts one of the ShowWindow commands.</param>
        [PreserveSig]
        HRESULT SetShowCmd(uint iShowCmd);

        /// <summary>
        /// Gets the location (path and index) of the icon for a Shell link object.
        /// </summary>
        /// <param name="pszIconPath">The address of a buffer that receives the path of the file containing the icon.</param>
        /// <param name="cchIconPath">The maximum number of characters to copy to the buffer pointed to by the pszIconPath parameter.</param>
        /// <param name="piIcon">The address of a value that receives the index of the icon.</param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [PreserveSig]
        HRESULT GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath, out int piIcon);

        /// <summary>
        /// Sets the location (path and index) of the icon for a Shell link object.
        /// </summary>
        /// <param name="pszIconPath">The address of a buffer to contain the path of the file containing the icon.</param>
        /// <param name="iIcon">The index of the icon.</param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [PreserveSig]
        HRESULT SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);

        /// <summary>
        /// Sets the relative path to the Shell link object.
        /// </summary>
        /// <param name="pszPathRel">The address of a buffer that contains the fully-qualified path of the shortcut file, relative to which the shortcut resolution should be performed. It should be a file name, not a folder name.</param>
        /// <param name="dwReserved">Reserved. Set this parameter to zero.</param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [PreserveSig]
        HRESULT SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, uint dwReserved);

        /// <summary>
        /// Attempts to find the target of a Shell link, even if it has been moved or renamed.
        /// </summary>
        /// <param name="hwnd">A handle to the window that the Shell will use as the parent for a dialog box. The Shell displays the dialog box if it needs to prompt the user for more information while resolving a Shell link.</param>
        /// <param name="fFlags">Action flags. </param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [PreserveSig]
        HRESULT Resolve(IntPtr hwnd, uint fFlags);

        /// <summary>
        /// Sets the path and file name for the target of a Shell link object.
        /// </summary>
        /// <param name="pszFile">The address of a buffer that contains the new path.</param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [PreserveSig]
        HRESULT SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
    }
}
