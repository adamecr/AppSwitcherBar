using System;
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;

/// <summary>
/// Determines the types of items included in an enumeration. These values are used with the IShellFolder::EnumObjects method.
/// </summary>
[Flags]
internal enum SHCONTF
{
    /// <summary>
    /// The calling application is checking for the existence of child items in the folder.
    /// </summary>
    CHECKING_FOR_CHILDREN = 0x0010,
    /// <summary>
    ///  Include items that are folders in the enumeration.
    /// </summary>
    FOLDERS = 0x0020,
    /// <summary>
    /// Include items that are not folders in the enumeration
    /// </summary>
    NONFOLDERS = 0x0040,
    /// <summary>
    /// Include hidden items in the enumeration. This does not include hidden system items. (To include hidden system items, use SHCONTF_INCLUDESUPERHIDDEN.)
    /// </summary>
    INCLUDEHIDDEN = 0x0080,
    /// <summary>
    /// No longer used; always assumed.
    /// </summary>
    INIT_ON_FIRST_NEXT = 0x0100,
    /// <summary>
    /// The calling application is looking for printer objects.
    /// </summary>
    NETPRINTERSRCH = 0x0200,
    /// <summary>
    /// The calling application is looking for resources that can be shared.
    /// </summary>
    SHAREABLE = 0x0400,
    /// <summary>
    /// Include items with accessible storage and their ancestors, including hidden items.
    /// </summary>
    STORAGE = 0x0800,
    /// <summary>
    /// Child folders should provide a navigation enumeration.
    /// </summary>
    NAVIGATION_ENUM = 0x1000,
    /// <summary>
    /// The calling application is looking for resources that can be enumerated quickly.
    /// </summary>
    FASTITEMS = 0x2000,
    /// <summary>
    /// Enumerate items as a simple list even if the folder itself is not structured in that way.
    /// </summary>
    FLATLIST = 0x4000,
    /// <summary>
    /// The calling application is monitoring for change notifications. This means that the enumerator does not have to return all results. Items can be reported through change notifications.
    /// </summary>
    ENABLE_ASYNC = 0x8000,
    /// <summary>
    /// Include hidden system items in the enumeration. This value does not include hidden non-system items. (To include hidden non-system items, use SHCONTF_INCLUDEHIDDEN.)
    /// </summary>
    INCLUDESUPERHIDDEN = 0x10000
}