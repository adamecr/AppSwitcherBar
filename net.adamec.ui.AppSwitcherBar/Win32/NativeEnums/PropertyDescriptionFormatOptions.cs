using System;
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;

/// <summary>
/// Delineates the format of a property string.
/// </summary>
/// <remarks>
/// Typically use one, or a bitwise combination of 
/// these flags, to specify the format. Some flags are mutually exclusive, 
/// so combinations like <c>ShortTime | LongTime | HideTime</c> are not allowed.
/// </remarks>
[Flags]
public enum PropertyDescriptionFormatOptions
{
    /// <summary>
    /// The format settings specified in the property's .propdesc file.
    /// </summary>
    None = 0,

    /// <summary>
    /// The value preceded with the property's display name.
    /// </summary>
    /// <remarks>
    /// This flag is ignored when the <c>hideLabelPrefix</c> attribute of the <c>labelInfo</c> element 
    /// in the property's .propinfo file is set to true.
    /// </remarks>
    PrefixName = 0x1,

    /// <summary>
    /// The string treated as a file name.
    /// </summary>
    FileName = 0x2,

    /// <summary>
    /// The sizes displayed in kilobytes (KB), regardless of size. 
    /// </summary>
    /// <remarks>
    /// This flag applies to properties of <c>Integer</c> types and aligns the values in the column. 
    /// </remarks>
    AlwaysKB = 0x4,

    /// <summary>
    /// Reserved.
    /// </summary>
    RightToLeft = 0x8,

    /// <summary>
    /// The time displayed as 'hh:mm am/pm'.
    /// </summary>
    ShortTime = 0x10,

    /// <summary>
    /// The time displayed as 'hh:mm:ss am/pm'.
    /// </summary>
    LongTime = 0x20,

    /// <summary>
    /// The time portion of date/time hidden.
    /// </summary>
    HideTime = 64,

    /// <summary>
    /// The date displayed as 'MM/DD/YY'. For example, '3/21/04'.
    /// </summary>
    ShortDate = 0x80,

    /// <summary>
    /// The date displayed as 'DayOfWeek Month day, year'. 
    /// For example, 'Monday, March 21, 2004'.
    /// </summary>
    LongDate = 0x100,

    /// <summary>
    /// The date portion of date/time hidden.
    /// </summary>
    HideDate = 0x200,

    /// <summary>
    /// The friendly date descriptions, such as "Yesterday".
    /// </summary>
    RelativeDate = 0x400,

    /// <summary>
    /// The text displayed in a text box as a cue for the user, such as 'Enter your name'.
    /// </summary>
    /// <remarks>
    /// The invitation text is returned if formatting failed or the value was empty. 
    /// Invitation text is text displayed in a text box as a cue for the user, 
    /// Formatting can fail if the data entered 
    /// is not of an expected type, such as putting alpha characters in 
    /// a phone number field.
    /// </remarks>
    UseEditInvitation = 0x800,

    /// <summary>
    /// This flag requires UseEditInvitation to also be specified. When the 
    /// formatting flags are ReadOnly | UseEditInvitation and the algorithm 
    /// would have shown invitation text, a string is returned that indicates 
    /// the value is "Unknown" instead of the invitation text.
    /// </summary>
    ReadOnly = 0x1000,

    /// <summary>
    /// The detection of the reading order is not automatic. Useful when converting 
    /// to ANSI to omit the Unicode reading order characters.
    /// </summary>
    NoAutoReadingOrder = 0x2000,

    /// <summary>
    /// Smart display of DateTime values
    /// </summary>
    SmartDateTime = 0x4000
}