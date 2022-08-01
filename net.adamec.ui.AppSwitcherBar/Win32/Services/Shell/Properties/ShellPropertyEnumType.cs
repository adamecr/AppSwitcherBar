using System.Diagnostics.CodeAnalysis;
using net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;
using net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces;

namespace net.adamec.ui.AppSwitcherBar.Win32.Services.Shell.Properties;

/// <summary>
/// Wrapper over native <see cref="IPropertyEnumType"/>
/// </summary>
[SuppressMessage("ReSharper", "InvertIf")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
internal class ShellPropertyEnumType
{
    /// <summary>
    /// Encapsulated native <see cref="IPropertyEnumType"/>
    /// </summary>
    private IPropertyEnumType NativePropertyEnumType { get; }
        
    /// <summary>
    /// Display text from an enumeration information structure
    /// </summary>
    private string? displayText;
    /// <summary>
    /// Gets display text from an enumeration information structure. 
    /// </summary>
    public string DisplayText
    {
        get
        {
            if (displayText == null)
            {
                NativePropertyEnumType.GetDisplayText(out displayText);
            }
            return displayText;
        }
    }

    /// <summary>
    /// Enumeration type from an enumeration information structure
    /// </summary>
    private PropEnumType? enumType;
    /// <summary>
    /// Gets an enumeration type from an enumeration information structure. 
    /// </summary>
    public PropEnumType EnumType
    {
        get
        {
            if (!enumType.HasValue)
            {
                NativePropertyEnumType.GetEnumType(out var tempEnumType);
                enumType = tempEnumType;
            }
            return enumType.Value;
        }
    }

    /// <summary>
    /// Minimum value from an enumeration information structure
    /// </summary>
    private object? minValue;
    /// <summary>
    /// Gets a minimum value from an enumeration information structure. 
    /// </summary>
    public object? RangeMinValue
    {
        get
        {
            if (minValue == null)
            {
                using var propVar = new PropVariant();
                NativePropertyEnumType.GetRangeMinValue(propVar);
                minValue = propVar.Value;
            }
            return minValue;

        }
    }

    /// <summary>
    /// Set value from an enumeration information structure
    /// </summary>
    private object? setValue;
    /// <summary>
    /// Gets a set value from an enumeration information structure. 
    /// </summary>
    public object? RangeSetValue
    {
        get
        {
            if (setValue == null)
            {
                using var propVar = new PropVariant();
                NativePropertyEnumType.GetRangeSetValue(propVar);
                setValue = propVar.Value;
            }
            return setValue;

        }
    }

    /// <summary>
    /// Value from an enumeration information structure
    /// </summary>
    private object? enumerationValue;
    /// <summary>
    /// Gets a value from an enumeration information structure. 
    /// </summary>
    public object? RangeValue
    {
        get
        {
            if (enumerationValue == null)
            {
                using var propVar = new PropVariant();
                NativePropertyEnumType.GetValue(propVar);
                enumerationValue = propVar.Value;
            }
            return enumerationValue;
        }
    }

    /// <summary>
    /// Private CTOR
    /// </summary>
    /// <param name="nativePropertyEnumType"></param>
    internal ShellPropertyEnumType(IPropertyEnumType nativePropertyEnumType)
    {
        NativePropertyEnumType = nativePropertyEnumType;
    }
}