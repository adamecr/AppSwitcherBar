using System;
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;

/// <summary>
/// Associates property names with property description list strings.
/// </summary>
[Flags]
public enum PropertyViewOptions
{
    /// <summary>
    /// The property is shown by default.
    /// </summary>
    None = 0x00000000,

    /// <summary>
    /// The property is centered.
    /// </summary>
    CenterAlign = 0x00000001,

    /// <summary>
    /// The property is right aligned.
    /// </summary>
    RightAlign = 0x00000002,

    /// <summary>
    /// The property is shown as the beginning of the next collection of properties in the view.
    /// </summary>
    BeginNewGroup = 0x00000004,

    /// <summary>
    /// The remainder of the view area is filled with the content of this property.
    /// </summary>
    FillArea = 0x00000008,

    /// <summary>
    /// The property is reverse sorted if it is a property in a list of sorted properties.
    /// </summary>
    SortDescending = 0x00000010,

    /// <summary>
    /// The property is only shown if it is present.
    /// </summary>
    ShowOnlyIfPresent = 0x00000020,

    /// <summary>
    /// The property is shown by default in a view (where applicable).
    /// </summary>
    ShowByDefault = 0x00000040,

    /// <summary>
    /// The property is shown by default in primary column selection user interface (UI).
    /// </summary>
    ShowInPrimaryList = 0x00000080,

    /// <summary>
    /// The property is shown by default in secondary column selection UI.
    /// </summary>
    ShowInSecondaryList = 0x00000100,

    /// <summary>
    /// The label is hidden if the view is normally inclined to show the label.
    /// </summary>
    HideLabel = 0x00000200,

    /// <summary>
    /// The property is not displayed as a column in the UI.
    /// </summary>
    Hidden = 0x00000800,

    /// <summary>
    /// The property is wrapped to the next row.
    /// </summary>
    CanWrap = 0x00001000,

    /// <summary>
    /// A mask used to retrieve all flags.
    /// </summary>
    MaskAll = 0x000003ff,
}