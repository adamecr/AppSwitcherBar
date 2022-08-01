using System;
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;

/// <summary>
/// Used to determine how to compare two Shell items. IShellItem::Compare uses this enumerated type.
/// </summary>
[Flags]
internal enum SICHINT : uint
{
    /// <summary>
    /// This relates to the iOrder parameter of the IShellItem::Compare interface and indicates that the comparison is based on the display in a folder view.
    /// </summary>
    DISPLAY = 0x00000000,
    /// <summary>
    /// Exact comparison of two instances of a Shell item.
    /// </summary>
    ALLFIELDS = 0x80000000,
    /// <summary>
    /// This relates to the iOrder parameter of the IShellItem::Compare interface and indicates that the comparison is based on a canonical name.
    /// </summary>
    CANONICAL = 0x10000000,
    /// <summary>
    /// Windows 7 and later. If the Shell items are not the same, test the file system paths.
    /// </summary>
    TEST_FILESYSPATH_IF_NOT_EQUAL = 0x20000000,
};