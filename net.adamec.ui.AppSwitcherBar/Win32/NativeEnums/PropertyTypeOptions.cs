using System;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;

/// <summary>
/// Describes the attributes of the <c>typeInfo</c> element in the property's <c>.propdesc</c> file.
/// </summary>
[Flags]
public enum PropertyTypeOptions
{
    /// <summary>
    /// The property uses the default values for all attributes.
    /// </summary>
    None = 0x00000000,

    /// <summary>
    /// The property can have multiple values.   
    /// </summary>
    /// <remarks>
    /// These values are stored as a VT_VECTOR in the PROPVARIANT structure.
    /// This value is set by the multipleValues attribute of the typeInfo element in the property's .propdesc file.
    /// </remarks>
    MultipleValues = 0x00000001,

    /// <summary>
    /// This property cannot be written to. 
    /// </summary>
    /// <remarks>
    /// This value is set by the isInnate attribute of the typeInfo element in the property's .propdesc file.
    /// </remarks>
    IsInnate = 0x00000002,

    /// <summary>
    /// The property is a group heading. 
    /// </summary>
    /// <remarks>
    /// This value is set by the isGroup attribute of the typeInfo element in the property's .propdesc file.
    /// </remarks>
    IsGroup = 0x00000004,

    /// <summary>
    /// The user can group by this property. 
    /// </summary>
    /// <remarks>
    /// This value is set by the canGroupBy attribute of the typeInfo element in the property's .propdesc file.
    /// </remarks>
    CanGroupBy = 0x00000008,

    /// <summary>
    /// The user can stack by this property. 
    /// </summary>
    /// <remarks>
    /// This value is set by the canStackBy attribute of the typeInfo element in the property's .propdesc file.
    /// </remarks>
    CanStackBy = 0x00000010,

    /// <summary>
    /// This property contains a hierarchy. 
    /// </summary>
    /// <remarks>
    /// This value is set by the isTreeProperty attribute of the typeInfo element in the property's .propdesc file.
    /// </remarks>
    IsTreeProperty = 0x00000020,

    /// <summary>
    /// Include this property in any full text query that is performed. 
    /// </summary>
    /// <remarks>
    /// This value is set by the includeInFullTextQuery attribute of the typeInfo element in the property's .propdesc file.
    /// </remarks>
    IncludeInFullTextQuery = 0x00000040,

    /// <summary>
    /// This property is meant to be viewed by the user.  
    /// </summary>
    /// <remarks>
    /// This influences whether the property shows up in the "Choose Columns" dialog, for example.
    /// This value is set by the isViewable attribute of the typeInfo element in the property's .propdesc file.
    /// </remarks>
    IsViewable = 0x00000080,

    /// <summary>
    /// This property is included in the list of properties that can be queried.   
    /// </summary>
    /// <remarks>
    /// A queryable property must also be viewable.
    /// This influences whether the property shows up in the query builder UI.
    /// This value is set by the isQueryable attribute of the typeInfo element in the property's .propdesc file.
    /// </remarks>
    IsQueryable = 0x00000100,

    /// <summary>
    /// Used with an innate property (that is, a value calculated from other property values) to indicate that it can be deleted.  
    /// </summary>
    /// <remarks>
    /// Windows Vista with Service Pack 1 (SP1) and later.
    /// This value is used by the Remove Properties user interface (UI) to determine whether to display a check box next to an property that allows that property to be selected for removal.
    /// Note that a property that is not innate can always be purged regardless of the presence or absence of this flag.
    /// </remarks>
    CanBePurged = 0x00000200,

    /// <summary>
    /// This property is owned by the system.
    /// </summary>
    IsSystemProperty = unchecked((int)0x80000000),

    /// <summary>
    /// A mask used to retrieve all flags.
    /// </summary>
    MaskAll = unchecked((int)0x800001FF),
}