// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;

/// <summary>
/// Provides a set of flags to be used with IConditionFactory, 
/// ICondition, and IConditionGenerator to indicate the operation.
/// </summary>
public enum PropertyConditionOperation
{
    /// <summary>
    /// The implicit comparison between the value of the property and the value of the constant.
    /// </summary>
    Implicit,

    /// <summary>
    /// The value of the property and the value of the constant must be equal.
    /// </summary>
    Equal,

    /// <summary>
    /// The value of the property and the value of the constant must not be equal.
    /// </summary>
    NotEqual,

    /// <summary>
    /// The value of the property must be less than the value of the constant.
    /// </summary>
    LessThan,

    /// <summary>
    /// The value of the property must be greater than the value of the constant.
    /// </summary>
    GreaterThan,

    /// <summary>
    /// The value of the property must be less than or equal to the value of the constant.
    /// </summary>
    LessThanOrEqual,

    /// <summary>
    /// The value of the property must be greater than or equal to the value of the constant.
    /// </summary>
    GreaterThanOrEqual,

    /// <summary>
    /// The value of the property must begin with the value of the constant.
    /// </summary>
    ValueStartsWith,

    /// <summary>
    /// The value of the property must end with the value of the constant.
    /// </summary>
    ValueEndsWith,

    /// <summary>
    /// The value of the property must contain the value of the constant.
    /// </summary>
    ValueContains,

    /// <summary>
    /// The value of the property must not contain the value of the constant.
    /// </summary>
    ValueNotContains,

    /// <summary>
    /// The value of the property must match the value of the constant, where '?' matches any single character and '*' matches any sequence of characters.
    /// </summary>
    DOSWildCards,

    /// <summary>
    /// The value of the property must contain a word that is the value of the constant.
    /// </summary>
    WordEqual,

    /// <summary>
    /// The value of the property must contain a word that begins with the value of the constant.
    /// </summary>
    WordStartsWith,

    /// <summary>
    /// The application is free to interpret this in any suitable way.
    /// </summary>
    ApplicationSpecific,
}