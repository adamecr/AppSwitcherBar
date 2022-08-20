using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
// ReSharper disable CommentTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces
{
    /// <summary>
    /// Accessor to taskbar/start menu pinned applications
    /// Undocumented interface, the information and use hints collected from:
    /// https://github.com/vhanla/DelphiExperiments/blob/db38c526dc856f170a972a771569c3ef8f71533d/VCL/Undocumented/Pin2Taskbar/main.pas
    /// https://geelaw.blog/entries/msedge-pins/
    /// https://www.geoffchappell.com/studies/windows/shell/shell32/interfaces/ipinnedlist.htm
    /// </summary>
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid(Win32Consts.IID_IPinnedList3)]
    internal interface IPinnedList3
    {
        /// <summary>
        /// Enumerates pinned applications
        /// </summary>
        /// <param name="ppv">Enumerator</param>
        /// <returns>Result of the operation</returns>
        [PreserveSig]
        HRESULT EnumObjects([Out] out IEnumFullIDList ppv);

        /// <summary>
        /// Gets the pinnable information about application
        /// </summary>
        /// <param name="dataObject">Data object to get pinnable info for</param>
        /// <param name="pinnableFlag">Pinnable flag (?meaning)</param>
        /// <param name="ppsiApplication">Application shell item</param>
        /// <param name="ppsiDestination">Destination (pin) shell item</param>
        /// <param name="ppszAppID">Application it</param>
        /// <param name="pfLaunchable">Flag whether the pin is launchable</param>
        /// <returns>Returns S_FALSE if item is not pinnable</returns>
        [PreserveSig]
        HRESULT GetPinnableInfo(IDataObject dataObject, int pinnableFlag,
            [Out] out IShellItem2 ppsiApplication, [Out] out IShellItem ppsiDestination,
            [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder ppszAppID, [Out] uint pfLaunchable);

        /// <summary>
        /// Flag whether the data object is pinnable
        /// </summary>
        /// <param name="dataObject">Data object to test</param>
        /// <param name="pinnableFlag">Pinnable flag (?meaning</param>
        /// <returns>Returns S_FALSE if item is not pinnable</returns>
        [PreserveSig]
        HRESULT IsPinnable(IDataObject dataObject, int pinnableFlag);

        /// <summary>
        /// Find the pidl on the pin list and resolve the link/target.
        /// </summary>
        /// <param name="HWND">Owner (?) window</param>
        /// <param name="flags">?</param>
        /// <param name="pidl">Pidl to find</param>
        /// <param name="ppidlResolved">Resolved pidl (target)</param>
        /// <returns>Returns S_OK if the pidl changed and was resolved.
        /// Returns S_FALSE if the pidl did not change.
        /// Returns an error if the Resolve failed.</returns>
        [PreserveSig]
        HRESULT Resolve(IntPtr HWND, uint flags, PIDLIST_ABSOLUTE pidl, [Out] out PIDLIST_ABSOLUTE ppidlResolved);


        /// <summary>
        /// Modifies the pin pidl (target)
        /// Pin:     pidlFrom = NULL, pidlTo = pidl
        /// Unpin:   pidlFrom = pidl, pidlTo = NULL
        /// Update:  pidlFrom = old,  pidlTo = new
        /// Move:    pidlFrom = pidl, pidlTo = SMPINPOS(iPos)
        /// </summary>
        /// <param name="pidlFrom">Initial pin pidl</param>
        /// <param name="pidlTo">Required pin pidl</param>
        /// <returns>Result of the operation</returns>
        [PreserveSig]
        HRESULT LegacyModify(PIDLIST_ABSOLUTE pidlFrom, PIDLIST_ABSOLUTE pidlTo);

        /// <summary>
        /// Returns the number of changes in pin list (?)
        /// </summary>
        /// <param name="count">Number of changes</param>
        /// <returns>Result of the operation</returns>
        [PreserveSig]
        HRESULT GetChangeCount([Out] out int count);

        /// <summary>
        /// Returns S_OK if the pidl is pinned, error if pidl is not pinned
        /// </summary>
        /// <param name="pidl">Pidl to test</param>
        /// <returns>Returns S_OK if the pidl is pinned, error if pidl is not pinned</returns>
        [PreserveSig]
        HRESULT IsPinned(PIDLIST_ABSOLUTE pidl);

        /// <summary>
        /// Finds the first pinned item whose AppID matches pidl, and returns its pidl in ppidlPinned
        /// </summary>
        /// <param name="pidl">pidl to application</param>
        /// <param name="ppidlPinned">pidl of pin </param>
        /// <returns>Result of the operation</returns>
        [PreserveSig]
        HRESULT GetPinnedItem(PIDLIST_ABSOLUTE pidl, [Out] out PIDLIST_ABSOLUTE ppidlPinned);

        /// <summary>
        /// Returns the AppID for the pinned item.
        /// </summary>
        /// <param name="pidl">Pidl must be provided by the pin list, or the AppID will not be found</param>
        /// <param name="ppszAppID">Application ID</param>
        /// <returns>Result of the operation</returns>
        [PreserveSig]
        HRESULT GetAppIDForPinnedItem(PIDLIST_ABSOLUTE pidl, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder ppszAppID);


        /// <summary>
        /// Updates pinned items in response to change notifications. In particular, makes sure the cached AppID is correct.
        /// </summary>
        /// <param name="pidlFrom">Initial pid</param>
        /// <param name="pidlTo">Target pidl</param>
        /// <returns>Result of the operation</returns>
        [PreserveSig]
        HRESULT ItemChangeNotify(PIDLIST_ABSOLUTE pidlFrom, PIDLIST_ABSOLUTE pidlTo);


        /// <summary>
        /// Verifies all items in the list haven't been removed from the machine by resolving each link and removing it if it fails to resolve 
        /// </summary>
        /// <returns>Result of the operation</returns>
        [PreserveSig]
        HRESULT UpdateForRemovedItemsAsNecessary();


        /// <summary>
        /// Pins a shell link
        /// </summary>
        /// <param name="us">?</param>
        /// <param name="shellLinkW">Link to pin</param>
        /// <returns>Result of the operation</returns>
        [PreserveSig]
        HRESULT PinShellLink(ushort us, IShellLinkW shellLinkW);

        /// <summary>
        /// Gets pinned item for appId
        /// </summary>
        /// <param name="apID">?meaning</param>
        /// <param name="pidl">Pidl of pinned item</param>
        /// <returns>?</returns>
        [PreserveSig]
        HRESULT GetPinnedItemForAppID(ushort apID, [Out] out PIDLIST_ABSOLUTE pidl);

        /// <summary>
        /// Modifies the pin pidl (target)
        /// Pin:     pidlFrom = NULL, pidlTo = pidl
        /// Unpin:   pidlFrom = pidl, pidlTo = NULL
        /// Update:  pidlFrom = old,  pidlTo = new
        /// Move:    pidlFrom = pidl, pidlTo = SMPINPOS(iPos)
        /// </summary>
        /// <param name="pidlFrom">Initial pin pidl</param>
        /// <param name="pidlTo">Required pin pidl</param>
        /// <param name="caller">Caller tracking information (?probably no meaning)</param>
        /// <returns>Result of the operation</returns>
        [PreserveSig]
        HRESULT Modify(PIDLIST_ABSOLUTE pidlFrom, PIDLIST_ABSOLUTE pidlTo, int caller);

    }
}
