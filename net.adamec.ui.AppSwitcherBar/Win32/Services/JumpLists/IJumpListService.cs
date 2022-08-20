using net.adamec.ui.AppSwitcherBar.Dto;
// ReSharper disable CommentTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.Services.JumpLists;

/// <summary>
/// Encapsulates the JumpList functionality
/// </summary>
public interface IJumpListService
{
    /// <summary>
    /// Checks whether there is automatic and/or custom destinations file for given <paramref name="appId"/> and
    /// if yes, parses the file(s) and provides the retrieved JumpList items
    /// </summary>
    /// <param name="appId">Application ID - either explicit AppId or full path to executable</param>
    /// <param name="installedApplications">Information about installed applications</param>
    /// <returns>Array of JumpList items retrieved for given <paramref name="appId"/></returns>
    LinkInfo[] GetJumpListItems(string appId, InstalledApplications installedApplications);


    /// <summary>
    /// Gets the information about the applications pinned to the taskbar
    /// </summary>
    /// <param name="knownFolders">Information about the known folder paths and GUIDs</param>
    /// <returns>Array of information about the applications pinned to the taskbar</returns>
    PinnedAppInfo[] GetPinnedApplications(StringGuidPair[] knownFolders);
}