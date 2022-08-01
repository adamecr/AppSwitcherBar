using System;
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;

/// <summary>
/// Describes how a property should be treated for display purposes.
/// </summary>
[Flags]
public enum PropertyColumnStateOptions
{
    /// <summary>
    /// Default value
    /// </summary>
    None = 0x00000000,

    /// <summary>
    /// The value is displayed as a string.
    /// </summary>
    StringType = 0x00000001,

    /// <summary>
    /// The value is displayed as an integer.
    /// </summary>
    IntegerType = 0x00000002,

    /// <summary>
    /// The value is displayed as a date/time.
    /// </summary>
    DateType = 0x00000003,

    /// <summary>
    /// A mask for display type values StringType, IntegerType, and DateType.
    /// </summary>
    TypeMask = 0x0000000f,

    /// <summary>
    /// The column should be on by default in Details view.
    /// </summary>
    OnByDefault = 0x00000010,

    /// <summary>
    /// Will be slow to compute. Perform on a background thread.
    /// </summary>
    Slow = 0x00000020,

    /// <summary>
    /// Provided by a handler, not the folder.
    /// </summary>
    Extended = 0x00000040,

    /// <summary>
    /// Not displayed in the context menu, but is listed in the More... dialog.
    /// </summary>
    SecondaryUI = 0x00000080,

    /// <summary>
    /// Not displayed in the user interface (UI).
    /// </summary>
    Hidden = 0x00000100,

    /// <summary>
    /// VarCmp produces same result as IShellFolder::CompareIDs.
    /// </summary>
    PreferVariantCompare = 0x00000200,

    /// <summary>
    /// PSFormatForDisplay produces same result as IShellFolder::CompareIDs.
    /// </summary>
    PreferFormatForDisplay = 0x00000400,

    /// <summary>
    /// Do not sort folders separately.
    /// </summary>
    NoSortByFolders = 0x00000800,

    /// <summary>
    /// Only displayed in the UI.
    /// </summary>
    ViewOnly = 0x00010000,

    /// <summary>
    /// Marks columns with values that should be read in a batch.
    /// </summary>
    BatchRead = 0x00020000,

    /// <summary>
    /// Grouping is disabled for this column.
    /// </summary>
    NoGroupBy = 0x00040000,

    /// <summary>
    /// Can't resize the column.
    /// </summary>
    FixedWidth = 0x00001000,

    /// <summary>
    /// The width is the same in all dots per inch (dpi)s.
    /// </summary>
    NoDpiScale = 0x00002000,

    /// <summary>
    /// Fixed width and height ratio.
    /// </summary>
    FixedRatio = 0x00004000,

    /// <summary>
    /// Filters out new display flags.
    /// </summary>
    DisplayMask = 0x0000F000,
}