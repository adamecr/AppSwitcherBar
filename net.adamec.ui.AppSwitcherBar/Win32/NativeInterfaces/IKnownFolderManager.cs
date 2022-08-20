using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeMethods;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable UnusedMember.Global
// ReSharper disable CommentTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces
{
    /// <summary>
    /// Exposes methods that create, enumerate or manage existing known folders.
    /// </summary>
    [ComImport]
    [Guid(Win32Consts.IID_IKnownFolderManager)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IKnownFolderManager
    {
        /// <summary>
        /// Gets the KNOWNFOLDERID that is the equivalent of a legacy CSIDL value.
        /// </summary>
        /// <param name="csidl">The CSIDL value.</param>
        /// <param name="knownFolderID">When this method returns, contains a pointer to the KNOWNFOLDERID. This pointer is passed uninitialized.</param>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT FolderIdFromCsidl(int csidl, [Out] out Guid knownFolderID);

        /// <summary>
        /// Gets the legacy CSIDL value that is the equivalent of a given KNOWNFOLDERID.
        /// </summary>
        /// <param name="id">Reference to the KNOWNFOLDERID.</param>
        /// <param name="csidl">When this method returns, contains a pointer to the CSIDL value. This pointer is passed uninitialized.</param>
        /// <returns></returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT FolderIdToCsidl([In, MarshalAs(UnmanagedType.LPStruct)] Guid id, [Out] out int csidl);

        /// <summary>
        /// Gets an array of all registered known folder IDs. This can be used in enumerating all known folders.
        /// </summary>
        /// <param name="folders">When this method returns, contains a pointer to an array of all KNOWNFOLDERID values registered with the system.
        /// Use CoTaskMemFree to free these resources when they are no longer needed.</param>
        /// <param name="count">When this method returns, contains a pointer to the number of KNOWNFOLDERID values in the array at ppKFId. The [in] functionality of this parameter is not used.</param>
        /// <returns></returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetFolderIds([Out] out IntPtr folders, [Out] out uint count);

        /// <summary>
        /// Gets an object that represents a known folder identified by its KNOWNFOLDERID.
        /// The object allows you to query certain folder properties, get the current path of the folder, redirect the folder to another location, and get the path of the folder as an ITEMIDLIST.
        /// </summary>
        /// <param name="id">Reference to the KNOWNFOLDERID.</param>
        /// <param name="knownFolder">When this method returns, contains an interface pointer to the IKnownFolder object that represents the folder.</param>
        /// <returns></returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetFolder([In, MarshalAs(UnmanagedType.LPStruct)] Guid id, [Out, MarshalAs(UnmanagedType.Interface)] out IKnownFolder? knownFolder);

        /// <summary>
        /// Gets an object that represents a known folder identified by its canonical name.
        /// The object allows you to query certain folder properties, get the current path of the folder, redirect the folder to another location, and get the path of the folder as an ITEMIDLIST.
        /// </summary>
        /// <param name="canonicalName">A pointer to the non-localized, canonical name for the known folder, stored as a null-terminated Unicode string. If this folder is a common or per-user folder, this value is also used as the value name of the "User Shell Folders" registry settings.
        /// This value is retrieved through the pszName member of the folder's KNOWNFOLDER_DEFINITION structure.</param>
        /// <param name="knownFolder">When this method returns, contains the address of a pointer to the IKnownFolder object that represents the known folder.</param>
        /// <returns></returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetFolderByName(string canonicalName, [Out, MarshalAs(UnmanagedType.Interface)] out IKnownFolder? knownFolder);

        /// <summary>
        /// Adds a new known folder to the registry. Used particularly by independent software vendors (ISVs) that are adding one of their own folders to the known folder system.
        /// </summary>
        /// <param name="knownFolderGuid">A GUID that represents the known folder.</param>
        /// <param name="knownFolderDefinition">A pointer to a valid KNOWNFOLDER_DEFINITION structure that provides the details of the new folder.</param>
        /// <returns></returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT RegisterFolder([In, MarshalAs(UnmanagedType.LPStruct)] Guid knownFolderGuid, [In] ref NativeFolderDefinition knownFolderDefinition);

        /// <summary>
        /// Remove a known folder from the registry, which makes it unknown to the known folder system. This method does not remove the folder itself.
        /// </summary>
        /// <param name="knownFolderGuid">GUID or KNOWNFOLDERID that represents the known folder.</param>
        /// <returns></returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT UnregisterFolder([In, MarshalAs(UnmanagedType.LPStruct)] Guid knownFolderGuid);

        /// <summary>
        /// Gets an object that represents a known folder based on a file system path.
        /// The object allows you to query certain folder properties, get the current path of the folder, redirect the folder to another location, and get the path of the folder as an ITEMIDLIST.
        /// </summary>
        /// <param name="path">Pointer to a null-terminated Unicode string of length MAX_PATH that contains a path to a known folder.</param>
        /// <param name="mode">One of the following values that specify the precision of the match of path and known folder:
        /// FFFP_EXACTMATCH=0 - Retrieve only the specific known folder for the given file path.
        /// FFFP_NEARESTPARENTMATCH=1 - If an exact match is not found for the given file path, retrieve the first known folder that matches one of its parent folders walking up the parent tree.</param>
        /// <param name="knownFolder">When this method returns, contains the address of a pointer to the IKnownFolder object that represents the known folder.</param>
        /// <returns></returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT FindFolderFromPath([In, MarshalAs(UnmanagedType.LPWStr)] string path, [In] int mode, [Out, MarshalAs(UnmanagedType.Interface)] out IKnownFolder? knownFolder);

        /// <summary>
        /// Gets an object that represents a known folder based on an IDList. The object allows you to query certain folder properties, get the current path of the folder, redirect the folder to another location, and get the path of the folder as an ITEMIDLIST.
        /// </summary>
        /// <param name="pidl">A pointer to the IDList.</param>
        /// <param name="knownFolder">When this method returns, contains the address of a pointer to the IKnownFolder object that represents the known folder.</param>
        /// <returns></returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT FindFolderFromIDList(IntPtr pidl, [Out, MarshalAs(UnmanagedType.Interface)] out IKnownFolder? knownFolder);

        /// <summary>
        /// DONT USE, THE FUNC PARAMETERS ARE MISSING!!!
        /// Redirects folder requests for common and per-user folders.
        /// </summary>
        /// <returns></returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT Redirect();
    }
}
