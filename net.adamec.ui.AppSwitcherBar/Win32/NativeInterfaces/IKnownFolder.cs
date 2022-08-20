using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces
{
    /// <summary>
    /// Exposes methods that allow an application to retrieve information about a known folder's category, type, GUID, pointer to an item identifier list (PIDL) value, redirection capabilities, and definition.
    /// It provides a method for the retrieval of a known folder's IShellItem object.
    /// It also provides methods to get or set the path of the known folder.
    /// </summary>
    [ComImport]
    [Guid(Win32Consts.IID_IKnownFolder)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IKnownFolder
    {
        /// <summary>
        /// Gets the ID of the selected folder.
        /// </summary>
        /// <param name="guid">ID of the selected folder</param>
        /// <returns></returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetId([Out] out Guid guid);

        /// <summary>
        /// Retrieves the category—virtual, fixed, common, or per-user—of the selected folder.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        FolderCategory GetCategory();

        /// <summary>
        /// Retrieves the location of a known folder in the Shell namespace in the form of a Shell item (IShellItem or derived interface).
        /// </summary>
        /// <param name="i">Flags that specify special retrieval options. This value can be 0; otherwise, one or more of the KNOWN_FOLDER_FLAG values.</param>
        /// <param name="interfaceGuid">A reference to the IID of the requested interface.</param>
        /// <param name="shellItem">When this method returns, contains the interface pointer requested in riid. This is typically IShellItem or IShellItem2.</param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [PreserveSig]
        HRESULT GetShellItem([In] int i, ref Guid interfaceGuid, [Out, MarshalAs(UnmanagedType.Interface)] out IShellItem2 shellItem);

        /// <summary>
        /// Retrieves the path of a known folder as a string.
        /// </summary>
        /// <param name="option">Flags that specify special retrieval options. This value can be 0; otherwise, one or more of the KNOWN_FOLDER_FLAG values.</param>
        /// <param name="path">The path of a known folder as a string.</param>
        /// <returns>If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.</returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetPath([In] uint option, [Out, MarshalAs(UnmanagedType.LPWStr)] out string? path);

        /// <summary>
        /// Assigns a new path to a known folder.
        /// </summary>
        /// <param name="i">Either zero or the following value:KF_FLAG_DONT_UNEXPAND</param>
        /// <param name="path">Pointer to the folder's new path. </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetPath([In] int i, [In] string path);

        /// <summary>
        /// Gets the location of the Shell namespace folder in the IDList (ITEMIDLIST) form.
        /// </summary>
        /// <param name="i">Flags that specify special retrieval options. This value can be 0; otherwise, one or more of the KNOWN_FOLDER_FLAG values.</param>
        /// <param name="itemIdentifierListPointer">When this method returns, contains the address of an absolute PIDL. This parameter is passed uninitialized. The calling application is responsible for freeing this resource when it is no longer needed.</param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetIDList([In] int i, [Out] out IntPtr itemIdentifierListPointer);

        /// <summary>
        /// Retrieves the folder type.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Guid GetFolderType();

        /// <summary>
        /// Gets a value that states whether the known folder can have its path set to a new value or what specific restrictions or prohibitions are placed on that redirection.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        RedirectionCapability GetRedirectionCapabilities();

        /// <summary>
        /// Retrieves a structure that contains the defining elements of a known folder, which includes the folder's category, name, path, description, tooltip, icon, and other properties.
        /// </summary>
        /// <param name="definition">When this method returns, contains a pointer to the KNOWNFOLDER_DEFINITION structure. When no longer needed, the calling application is responsible for calling FreeKnownFolderDefinitionFields to free this resource.</param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFolderDefinition([Out, MarshalAs(UnmanagedType.Struct)] out NativeFolderDefinition definition);

    }
}
