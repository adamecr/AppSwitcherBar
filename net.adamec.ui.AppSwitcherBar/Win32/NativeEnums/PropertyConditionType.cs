// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;

/// <summary>
/// Specifies the condition type to use when displaying the property in the query builder user interface (UI).
/// </summary>
public enum PropertyConditionType
{
    /// <summary>
    /// The default condition type.
    /// </summary>
    None = 0,

    /// <summary>
    /// The string type.
    /// </summary>
    String = 1,

    /// <summary>
    /// The size type.
    /// </summary>
    Size = 2,

    /// <summary>
    /// The date/time type.
    /// </summary>
    DateTime = 3,

    /// <summary>
    /// The Boolean type.
    /// </summary>
    Boolean = 4,

    /// <summary>
    /// The number type.
    /// </summary>
    Number = 5,
}