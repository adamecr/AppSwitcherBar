using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using net.adamec.ui.AppSwitcherBar.Dto;
using net.adamec.ui.AppSwitcherBar.Win32.NativeClasses;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;
using net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces;
using net.adamec.ui.AppSwitcherBar.Win32.NativeMethods;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.Services.Shell
{
    /// <summary>
    /// Encapsulates the Shell related native API
    /// </summary>
    internal static class Shell
    {
        /// <summary>
        /// Required to retrieve certain information about a process (GetExitCodeProcess, GetPriorityClass, IsProcessInJob, QueryFullProcessImageName).
        /// A handle that has the PROCESS_QUERY_INFORMATION access right is automatically granted PROCESS_QUERY_LIMITED_INFORMATION.
        /// </summary>
        internal const int QueryLimitedInformation = 0x1000;

        /// <summary>
        /// GUIDs of known folders defined as constants in <see cref="KnownFolderId"/>
        /// </summary>
        private static Guid[]? knownFolderIds;
        /// <summary>
        /// GUIDs and paths for know folders defined as GUID constants in <see cref="KnownFolderId"/>
        /// </summary>
        private static StringGuidPair[]? knownFolderPaths;

        /// <summary>
        /// Retrieves the <see cref="IPropertyStore"/> of the window
        /// </summary>
        /// <param name="handle">A handle to the window whose properties are being retrieved.</param>
        /// <returns>Property store or null when the store can't be retrieved</returns>
        // ReSharper disable once InconsistentNaming
        internal static IPropertyStore? GetPropertyStoreForWindow(IntPtr handle)
        {
            var g = new Guid(Win32Consts.IID_IPropertyStore);

            var hr = Shell32.SHGetPropertyStoreForWindow(handle, ref g, out var store);
            return hr.IsSuccess ? store : null;
        }

        /// <summary>
        /// Get the GUIDs of known folders defined as constants in <see cref="KnownFolderId"/>
        /// </summary>
        /// <returns>GUIDs of known folders defined as constants in <see cref="KnownFolderId"/></returns>
        private static Guid[] GetKnownFolderIds()
        {
            return typeof(KnownFolderId)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(Guid))
                .Select(f => (Guid)f.GetValue(null)!)
                .ToArray();
        }

        /// <summary>
        /// Get the GUIDs and paths for know folders defined as GUID constants in <see cref="KnownFolderId"/>
        /// </summary>
        /// <returns>GUIDs and paths for know folders defined as GUID constants in <see cref="KnownFolderId"/></returns>
        internal static StringGuidPair[] GetKnownFolders()
        {
            if (knownFolderPaths != null) return knownFolderPaths;

            knownFolderIds ??= GetKnownFolderIds();

            knownFolderPaths = (
                    from guid in knownFolderIds
                    let knownFolderPath = GetPathForKnownFolder(guid)
                    where knownFolderPath != null
                    select new StringGuidPair(knownFolderPath, guid)
                )
                .ToList()
                .OrderByDescending(i => i.String)
                .ToArray();

            return knownFolderPaths;
        }

        /// <summary>
        /// Retrieves a (real) path of the known folder
        /// </summary>
        /// <param name="knownFolder">GUID of the known folder to get the path for</param>
        /// <returns>Path to the known folder or null when the path can't be retrieved</returns>
        internal static string? GetPathForKnownFolder(Guid knownFolder)
        {
            if (knownFolder == default) return null;

            var pathBuilder = new StringBuilder(Win32Consts.MAX_PATH);
            var hr = Shell32.SHGetFolderPathEx(ref knownFolder, 0, IntPtr.Zero, pathBuilder, (uint)pathBuilder.Capacity);

            return hr.IsSuccess ? pathBuilder.ToString() : null;
        }

        /// <summary>
        /// Replaces the known folder within the path with it's guid (if any)
        /// </summary>
        /// <param name="path">Path to check</param>
        /// <returns>Path with known folder represented by its GUID (if applicable)</returns>
        internal static string? ReplaceKnownFolderWithGuid(string? path)
        {
            if (path == null) return null;

            var knownFolderManagerType = Type.GetTypeFromCLSID(new Guid(Win32Consts.CLSID_KnownFolderManager));
            if (knownFolderManagerType == null) return path;
            if (Activator.CreateInstance(knownFolderManagerType) is not IKnownFolderManager knownFolderManager) return path;

            //FFP_EXACTMATCH = 0, FFFP_NEARESTPARENTMATCH=1
            var hrs = knownFolderManager.FindFolderFromPath(path, 1, out var knownFolder);
            if (!hrs.IsSuccess || knownFolder == null) return path;

            hrs = knownFolder.GetPath(0, out var knownFolderPath);
            if (!hrs.IsSuccess || knownFolderPath == null) return path;

            hrs = knownFolder.GetId(out var knownFolderGuid);
            if (!hrs.IsSuccess) return path;

            if (knownFolderGuid == KnownFolderId.ProgramFiles)
                knownFolderGuid = Environment.Is64BitOperatingSystem
                    ? KnownFolderId.ProgramFilesX64
                    : KnownFolderId.ProgramFilesX86;

            if (knownFolderGuid == KnownFolderId.ProgramFilesCommon)
                knownFolderGuid = Environment.Is64BitOperatingSystem
                    ? KnownFolderId.ProgramFilesCommonX64
                    : KnownFolderId.ProgramFilesCommonX86;

            path = path.Replace(knownFolderPath, knownFolderGuid.ToString("B"));
            return path;
        }

        /// <summary>
        /// Retrieve <see cref="IShellItem2"/> of the known folder
        /// </summary>
        /// <param name="knownFolderId">GUID of the known folder to get the shell item for</param>
        /// <returns><see cref="IShellItem2"/> of the known folder or null when it can't be retrieved</returns>
        internal static IShellItem2? GetKnownFolderItem(Guid knownFolderId)
        {
            var hr = Shell32.SHGetKnownFolderItem(knownFolderId, 0, IntPtr.Zero, Guid.Parse(Win32Consts.IID_IShellItem2), out var shellItem);

            return hr.IsSuccess && shellItem is IShellItem2 shellItem2 ? shellItem2 : null;
        }

        /// <summary>
        /// Retrieve <see cref="IShellItem2"/> for Apps folder (<see cref="KnownFolderId.AppsFolder"/>) containing the installed applications
        /// </summary>
        /// <returns><see cref="IShellItem2"/> for Apps folder or null when it can't be retrieved</returns>
        internal static IShellItem2? GetAppsFolder()
        {
            var appsFolder = Shell.GetKnownFolderItem(KnownFolderId.AppsFolder);
            return appsFolder;
        }

        /// <summary>
        /// Retrieves a <see cref="IShellItem2"/> for given <paramref name="path"/>
        /// </summary>
        /// <param name="path">Full path to retrieve a shell item for</param>
        /// <returns><see cref="IShellItem2"/> for given <paramref name="path"/> or null when can't retrieve a shell item</returns>
        internal static IShellItem2? GetShellItemForPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;

            var iidShellItem2 = new Guid(Win32Consts.IID_IShellItem2);
            var hr = Shell32.SHCreateItemFromParsingName(path, null, ref iidShellItem2, out var unk);

            return hr.IsSuccess && unk is IShellItem2 shellItem2 ? shellItem2 : null;
        }

        /// <summary>
        /// Gets the icon of given <paramref name="shellItem"/> in preferred <paramref name="size"/>
        /// </summary>
        /// <param name="shellItem">Shell item to retrieve the icon for</param>
        /// <param name="size">Preferred icon size</param>
        /// <param name="freeze">Flag whether to <see cref="BitmapSource.Freeze"/> the returned <see cref="BitmapSource"/>. This is needed when the bitmap source is not created in main UI thread</param>
        /// <returns>The icon of given <paramref name="shellItem"/> or null when it can't be retrieved</returns>
        internal static BitmapSource? GetShellItemBitmapSource(IShellItem2 shellItem, int size, bool freeze = true)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            var factory = (IShellItemImageFactory)shellItem;

            var s = new SIZE(size, size);
            var hr = factory.GetImage(s, SIIGBF.ResizeToFit | SIIGBF.BiggerSizeOk, out var hBitmap); //extract and resize
            if (!hr.IsSuccess) return null;

            var sourceFromHBitmap = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            if (freeze) sourceFromHBitmap.Freeze(); //needed when not being in main UI thread
            Gdi32.DeleteObject(hBitmap);

            return sourceFromHBitmap;
        }


        /// <summary>
        /// Enumerates the shell items of given <paramref name="parent"/> item (might be a directory).
        /// Given <paramref name="action"/> is called for each child shell item
        /// </summary>
        /// <param name="parent">Shell item to enumerate the containing items of</param>
        /// <param name="action">Action to be called for each child shell item</param>
        internal static void EnumShellItems(IShellItem2 parent, Action<IShellItem2> action)
        {
            // ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (parent == null || action == null) return;
            // ReSharper restore ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

            //get the enumerator
            var hr = parent.BindToHandler(IntPtr.Zero, Win32Consts.BHID_EnumItems, typeof(IEnumShellItems).GUID, out var pEnum);
            if (!hr.IsSuccess || pEnum == IntPtr.Zero || Marshal.GetObjectForIUnknown(pEnum) is not IEnumShellItems pEnumShellItems) return;

            //enumerated
            while (HRESULT.S_OK == pEnumShellItems.Next(1, out var itm, out var nFetched) && nFetched == 1)
            {
                if (itm is IShellItem2 shellItem2) action.Invoke(shellItem2);
            }
        }

        /// <summary>
        /// Creates and initializes a Shell item object from a pointer to an item identifier list (PIDL)
        /// </summary>
        /// <param name="pidl">The source PIDL.</param>
        /// <returns>Shell item object from a pointer to an item identifier list (PIDL) or null when the shell item can't be created</returns>
        internal static IShellItem2? CreateShellItemFromIdList(IntPtr pidl)
        {
            var gIShellItem2 = new Guid(Win32Consts.IID_IShellItem2);
            if (pidl != IntPtr.Zero && Shell32.SHCreateItemFromIDList(pidl, gIShellItem2, out var shellItem).IsSuccess)
                return shellItem;

            return null;
        }

        /// <summary>
        /// Displays or hides the desktop.
        /// </summary>
        internal static void ToggleDesktop()
        {
            var shell = new CShell();
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (shell is IShellDispatch6 shellDispatch)
            {
                shellDispatch.ToggleDesktop();
            }
        }
    }
}
