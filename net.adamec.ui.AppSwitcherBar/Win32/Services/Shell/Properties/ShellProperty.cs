using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;
using net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;

namespace net.adamec.ui.AppSwitcherBar.Win32.Services.Shell.Properties;

/// <summary>
/// Wrapper of Property system property value
/// </summary>
internal class ShellProperty
{
    /// <summary>
    /// Property key
    /// </summary>
    public PropertyKey Key { get; init; }

    /// <summary>
    /// Name of the <see cref="Key"/> if known
    /// </summary>
    public string? KeyName { get; init; }

    /// <summary>
    /// Variant type of property value
    /// </summary>
    public VarEnum VarType { get; init; }

    /// <summary>
    /// Checks value is empty (<see cref="VarEnum.VT_EMPTY"/> or null (<see cref="VarEnum.VT_NULL"/>
    /// </summary>
    /// <returns>True when the value is empty or null</returns>
    public bool IsNullOrEmpty => VarType == VarEnum.VT_EMPTY | VarType == VarEnum.VT_NULL;

    /// <summary>
    /// Retrieved property value (can be also null) or null when not retrieved
    /// </summary>
    public object? Value { get; init; }

    /// <summary>
    /// Flag whether the property value has been retrieved
    /// </summary>
    public bool IsValueRetrieved { get; init; }

    /// <summary>
    /// Typed string value when the <see cref="Value"/> was retrieved and is string
    /// </summary>
    public string? ValueAsString => IsValueRetrieved && Value is string propStr ? propStr : null;

    /// <summary>
    /// Private CTOR
    /// </summary>
    private ShellProperty()
    {

    }

    /// <summary>
    /// Gets the string representation of an object
    /// </summary>
    /// <returns>The string representation of an object</returns>
    public override string ToString()
    {
        return $"{KeyName} {Key} {VarEnumToString(VarType)} {Value}";
    }

    /// <summary>
    /// Gets the property value wrapper
    /// </summary>
    /// <param name="shellItem"><see cref="IShellItem2"/> owning the property store</param>
    /// <param name="formatId">Property key format ID</param>
    /// <param name="propertyId">Property key property ID</param>
    /// <returns>Property value wrapper</returns>
    public static ShellProperty Get(IShellItem2 shellItem, string formatId, int propertyId)
    {
        return Get(shellItem, new PropertyKey(formatId, propertyId));
    }

    /// <summary>
    /// Gets the property value wrapper
    /// </summary>
    /// <param name="shellItem"><see cref="IShellItem2"/> owning the property store</param>
    /// <param name="formatId">Property key format ID</param>
    /// <param name="propertyId">Property key property ID</param>
    /// <returns>Property value wrapper</returns>
    public static ShellProperty Get(IShellItem2 shellItem, Guid formatId, int propertyId)
    {
        return Get(shellItem, new PropertyKey(formatId, propertyId));
    }

    /// <summary>
    /// Gets the property value wrapper
    /// </summary>
    /// <param name="shellItem"><see cref="IShellItem2"/> owning the property store containing the property</param>
    /// <param name="propertyKey">Property key</param>
    /// <returns>Property value wrapper</returns>
    public static ShellProperty Get(IShellItem2 shellItem, PropertyKey propertyKey)
    {
        var hr = shellItem.GetPropertyStore(GPS.DEFAULT, typeof(IPropertyStore).GUID, out var ps);
        if (hr.IsSuccess)
        {
            return Get(ps, propertyKey);
        }

        return new ShellProperty()
        {
            Key = propertyKey,
            VarType = VarEnum.VT_NULL,
            IsValueRetrieved = false,
            KeyName = PropertyKey.GetPropertyKeyName(propertyKey)
        };
    }

    /// <summary>
    /// Gets the property value wrapper
    /// </summary>
    /// <param name="propertyStore"><see cref="IPropertyStore"/> containing the property</param>
    /// <param name="formatId">Property key format ID</param>
    /// <param name="propertyId">Property key property ID</param>
    /// <returns>Property value wrapper</returns>
    public static ShellProperty Get(IPropertyStore propertyStore, string formatId, int propertyId)
    {
        return Get(propertyStore, new PropertyKey(formatId, propertyId));
    }

    /// <summary>
    /// Gets the property value wrapper
    /// </summary>
    /// <param name="propertyStore"><see cref="IPropertyStore"/> containing the property</param>
    /// <param name="formatId">Property key format ID</param>
    /// <param name="propertyId">Property key property ID</param>
    /// <returns>Property value wrapper</returns>
    public static ShellProperty Get(IPropertyStore propertyStore, Guid formatId, int propertyId)
    {
        return Get(propertyStore, new PropertyKey(formatId, propertyId));
    }

    /// <summary>
    /// Gets the property value wrapper
    /// </summary>
    /// <param name="propertyStore"><see cref="IPropertyStore"/> containing the property</param>
    /// <param name="propertyKey">Property key</param>
    /// <returns>Property value wrapper</returns>
    public static ShellProperty Get(IPropertyStore propertyStore, PropertyKey propertyKey)
    {
        object? value = null;
        var varType = VarEnum.VT_NULL;
        var isValueRetrieved = false;

        using (var pv = new PropVariant())
        {
            var hr = propertyStore.GetValue(ref propertyKey, pv);
            // ReSharper disable once InvertIf
            if (hr.IsSuccess)
            {
                value = pv.Value;
                varType = pv.VarType;
                isValueRetrieved = true;
            }
        }

        return new ShellProperty()
        {
            Key = propertyKey,
            VarType = varType,
            IsValueRetrieved = isValueRetrieved,
            Value = value,
            KeyName = PropertyKey.GetPropertyKeyName(propertyKey)
        };
    }


    /// <summary>
    /// Gets the property value wrapper for all available properties
    /// </summary>
    /// <param name="shellItem"><see cref="IShellItem2"/> to get all properties from</param>
    /// <returns>Property value wrapper for all available properties</returns>
    [SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "<Pending>")]
    public static ShellProperty[] GetAll(IShellItem2 shellItem)
    {
        IPropertyStore? ps = null;
        try
        {
            var iidIPropertyStore = new Guid(Win32Consts.IID_IPropertyStore);
            shellItem.GetPropertyStore(GPS.BESTEFFORT, ref iidIPropertyStore, out ps);
            var all = GetAll(ps);
            return all;
        }
        catch
        {
            return Array.Empty<ShellProperty>();
        }
        finally
        {
            if (ps != null)
            {
                Marshal.ReleaseComObject(ps);
                // ReSharper disable once RedundantAssignment
                ps = null;
            }
        }


    }

    /// <summary>
    /// Gets the property value wrapper for all available properties
    /// </summary>
    /// <param name="propertyStore"><see cref="IPropertyStore"/> to get all properties from</param>
    /// <returns>Property value wrapper for all available properties</returns>
    public static ShellProperty[] GetAll(IPropertyStore propertyStore)
    {
        var l = new List<ShellProperty>();

        // Populate the property collection
        var hr = propertyStore.GetCount(out var propertyCount);
        if (!hr.IsSuccess) return Array.Empty<ShellProperty>();
        for (uint i = 0; i < propertyCount; i++)
        {
            try
            {
                propertyStore.GetAt(i, out var propKey);
                var p = Get(propertyStore, propKey);
                l.Add(p);
            }
            catch
            {
                //ignore and try next one
            }
        }
        return l.ToArray();
    }

    /// <summary>
    /// Gets description of all properties provided by <paramref name="propertyStore"/>
    /// </summary>
    /// <param name="propertyStore"><see cref="IPropertyStore"/> to get all property descriptions from</param>
    /// <returns>Description of all properties provided by <paramref name="propertyStore"/></returns>
    public static ShellPropertyDescription[] GetDescriptions(IPropertyStore propertyStore)
    {
        try
        {
            var all = GetPropertyDescriptions(propertyStore);
            return all;
        }
        catch
        {
            return Array.Empty<ShellPropertyDescription>();
        }
    }


    /// <summary>
    /// Gets description of all properties provided by <paramref name="shellItem"/>
    /// </summary>
    /// <param name="shellItem"><see cref="IShellItem2"/> to get all property descriptions from</param>
    /// <returns>Description of all properties provided by <paramref name="shellItem"/></returns>
    [SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "<Pending>")]
    public static ShellPropertyDescription[] GetDescriptions(IShellItem2 shellItem)
    {
        IPropertyStore? ps = null;
        try
        {
            var iidIPropertyStore = new Guid(Win32Consts.IID_IPropertyStore);
            shellItem.GetPropertyStore(GPS.DEFAULT, ref iidIPropertyStore, out ps);
            var all = GetPropertyDescriptions(ps);
            return all;
        }
        catch
        {
            return Array.Empty<ShellPropertyDescription>();
        }
        finally
        {
            if (ps != null)
            {
                Marshal.ReleaseComObject(ps);
                // ReSharper disable once RedundantAssignment
                ps = null;
            }
        }
    }

    /// <summary>
    /// Gets description of all properties provided by <paramref name="propertyStore"/>
    /// </summary>
    /// <param name="propertyStore"><see cref="IPropertyStore"/> to get all property descriptions from</param>
    /// <returns>Description of all properties provided by <paramref name="propertyStore"/></returns>
    private static ShellPropertyDescription[] GetPropertyDescriptions(IPropertyStore propertyStore)
    {
        var l = new List<ShellPropertyDescription>();

        var hr = propertyStore.GetCount(out var propertyCount);
        if (!hr.IsSuccess) return Array.Empty<ShellPropertyDescription>();
        for (uint i = 0; i < propertyCount; i++)
        {
            propertyStore.GetAt(i, out var propKey);
            var d = new ShellPropertyDescription(propKey);
            l.Add(d);
        }
        return l.ToArray();
    }

    /// <summary>
    /// Provides the .NET <see cref="Type"/> equivalent to given <see cref="VarEnum"/> type
    /// </summary>
    /// <param name="varEnumType">VarEnum type</param>
    /// <returns>.NET Type equivalent to <paramref name="varEnumType"/></returns>
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    public static Type VarEnumToSystemType(VarEnum varEnumType)
    {
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        return varEnumType switch
        {
            VarEnum.VT_EMPTY or VarEnum.VT_NULL => typeof(object),
            VarEnum.VT_UI1 => typeof(byte?),
            VarEnum.VT_I2 => typeof(short?),
            VarEnum.VT_UI2 => typeof(ushort?),
            VarEnum.VT_I4 => typeof(int?),
            VarEnum.VT_UI4 => typeof(uint?),
            VarEnum.VT_I8 => typeof(long?),
            VarEnum.VT_UI8 => typeof(ulong?),
            VarEnum.VT_R8 => typeof(double?),
            VarEnum.VT_BOOL => typeof(bool?),
            VarEnum.VT_FILETIME => typeof(DateTime?),
            VarEnum.VT_CLSID => typeof(IntPtr?),
            VarEnum.VT_CF => typeof(IntPtr?),
            VarEnum.VT_BLOB => typeof(byte[]),
            VarEnum.VT_LPWSTR => typeof(string),
            VarEnum.VT_UNKNOWN => typeof(IntPtr?),
            VarEnum.VT_STREAM => typeof(IStream),
            VarEnum.VT_R4 => typeof(float?),
            VarEnum.VT_DATE => typeof(DateTime?),
            VarEnum.VT_BSTR => typeof(string),
            VarEnum.VT_DECIMAL => typeof(decimal?),
            VarEnum.VT_I1 => typeof(char?),
            VarEnum.VT_INT => typeof(int?),
            VarEnum.VT_UINT => typeof(uint?),
            VarEnum.VT_HRESULT => typeof(HRESULT?),
            VarEnum.VT_PTR => typeof(IntPtr?),
            VarEnum.VT_LPSTR => typeof(string),
            // ReSharper disable BitwiseOperatorOnEnumWithoutFlags
            VarEnum.VT_VECTOR | VarEnum.VT_UI1 => typeof(byte[]),
            VarEnum.VT_VECTOR | VarEnum.VT_I2 => typeof(short[]),
            VarEnum.VT_VECTOR | VarEnum.VT_UI2 => typeof(ushort[]),
            VarEnum.VT_VECTOR | VarEnum.VT_I4 => typeof(int[]),
            VarEnum.VT_VECTOR | VarEnum.VT_UI4 => typeof(uint[]),
            VarEnum.VT_VECTOR | VarEnum.VT_I8 => typeof(long[]),
            VarEnum.VT_VECTOR | VarEnum.VT_UI8 => typeof(ulong[]),
            VarEnum.VT_VECTOR | VarEnum.VT_R8 => typeof(double[]),
            VarEnum.VT_VECTOR | VarEnum.VT_BOOL => typeof(bool[]),
            VarEnum.VT_VECTOR | VarEnum.VT_FILETIME => typeof(DateTime[]),
            VarEnum.VT_VECTOR | VarEnum.VT_CLSID => typeof(IntPtr[]),
            VarEnum.VT_VECTOR | VarEnum.VT_CF => typeof(IntPtr[]),
            VarEnum.VT_VECTOR | VarEnum.VT_LPWSTR => typeof(string[]),
            VarEnum.VT_VECTOR | VarEnum.VT_R4 => typeof(float[]),
            VarEnum.VT_VECTOR | VarEnum.VT_DATE => typeof(DateTime[]),
            VarEnum.VT_VECTOR | VarEnum.VT_BSTR => typeof(string[]),
            VarEnum.VT_VECTOR | VarEnum.VT_DECIMAL => typeof(decimal[]),
            VarEnum.VT_VECTOR | VarEnum.VT_I1 => typeof(char[]),
            VarEnum.VT_VECTOR | VarEnum.VT_INT => typeof(int[]),
            VarEnum.VT_VECTOR | VarEnum.VT_UINT => typeof(uint[]),
            VarEnum.VT_VECTOR | VarEnum.VT_HRESULT => typeof(HRESULT[]),
            VarEnum.VT_VECTOR | VarEnum.VT_PTR => typeof(IntPtr[]),
            VarEnum.VT_VECTOR | VarEnum.VT_LPSTR => typeof(string[]),
            // ReSharper restore BitwiseOperatorOnEnumWithoutFlags
            _ => typeof(object),
        };
    }

    /// <summary>
    /// Gets the string representing the <see cref="VarEnum"/> value including the "additive" flags VT_VECTOR, VT_ARRAY and VT_BYREF
    /// </summary>
    /// <param name="varEnum"><see cref="VarEnum"/> value</param>
    /// <returns>The string representing the <see cref="VarEnum"/> value, including the "additive" flags VT_VECTOR, VT_ARRAY and VT_BYREF</returns>
    public static string VarEnumToString(VarEnum varEnum)
    {
        var varEnumBaseType = (VarEnum)((int)varEnum & 0x0fff);

        // ReSharper disable BitwiseOperatorOnEnumWithoutFlags
        var isVector = (varEnum & VarEnum.VT_VECTOR) == VarEnum.VT_VECTOR;
        var isArray = (varEnum & VarEnum.VT_ARRAY) == VarEnum.VT_ARRAY;
        var isByRef = (varEnum & VarEnum.VT_BYREF) == VarEnum.VT_BYREF;
        // ReSharper restore BitwiseOperatorOnEnumWithoutFlags

        return $"{(isByRef ? "VT_BYREF | " : "")}{(isArray ? "VT_ARRAY | " : "")}{(isVector ? "VT_VECTOR | " : "")}{varEnumBaseType}";
    }


}

