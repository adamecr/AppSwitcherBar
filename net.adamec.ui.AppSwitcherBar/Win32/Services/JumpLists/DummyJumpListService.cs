using System;
using net.adamec.ui.AppSwitcherBar.Dto;
// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.Services.JumpLists;

/// <summary>
/// Dummy implementation of <see cref="IJumpListService"/>
/// </summary>
public class DummyJumpListService : IJumpListService
{
    /// <summary>
    /// Dummy implementation if <see cref="IJumpListService.GetJumpListItems"/> - does nothing, just returns an empty array
    /// </summary>
    /// <param name="appId">Ignored</param>
    /// <param name="installedApplications">Ignored</param>
    /// <returns>Empty Array of JumpList items</returns>
    public LinkInfo[] GetJumpListItems(string appId, InstalledApplications installedApplications)
    {
        return Array.Empty<LinkInfo>();
    }

    /// <summary>
    /// Gets the information about the applications pinned to the taskbar - does nothing, just returns an empty array
    /// </summary>
    /// <param name="knownFolders">Information about the known folder paths and GUIDs</param>
    /// <returns>Empty Array of information about applications pinned to the taskbar</returns>
    public PinnedAppInfo[] GetPinnedApplications(StringGuidPair[] knownFolders)
    {
        return Array.Empty<PinnedAppInfo>();
    }
}