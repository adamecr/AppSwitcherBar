// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;

/// <summary>
/// Describes the particular wordings of sort offerings.
/// </summary>
/// <remarks>
/// Note that the strings shown are English versions only; 
/// localized strings are used for other locales.
/// </remarks>
public enum PropertySortDescription
{
    /// <summary>
    /// The default ascending or descending property sort, "Sort going up", "Sort going down".
    /// </summary>
    General,

    /// <summary>
    /// The alphabetical sort, "A on top", "Z on top".
    /// </summary>
    AToZ,

    /// <summary>
    /// The numerical sort, "Lowest on top", "Highest on top".
    /// </summary>
    LowestToHighest,

    /// <summary>
    /// The size sort, "Smallest on top", "Largest on top".
    /// </summary>
    SmallestToBiggest,

    /// <summary>
    /// The chronological sort, "Oldest on top", "Newest on top".
    /// </summary>
    OldestToNewest,
}