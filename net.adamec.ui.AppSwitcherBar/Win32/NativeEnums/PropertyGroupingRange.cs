// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;

/// <summary>
/// Specifies the property description grouping ranges.
/// </summary>
public enum PropertyGroupingRange
{
    /// <summary>
    /// The individual values.
    /// </summary>
    Discrete = 0,

    /// <summary>
    /// The static alphanumeric ranges.
    /// </summary>
    Alphanumeric = 1,

    /// <summary>
    /// The static size ranges.
    /// </summary>
    Size = 2,

    /// <summary>
    /// The dynamically-created ranges.
    /// </summary>
    Dynamic = 3,

    /// <summary>
    /// The month and year groups.
    /// </summary>
    Date = 4,

    /// <summary>
    /// The percent groups.
    /// </summary>
    Percent = 5,

    /// <summary>
    /// The enumerated groups.
    /// </summary>
    Enumerated = 6,
}