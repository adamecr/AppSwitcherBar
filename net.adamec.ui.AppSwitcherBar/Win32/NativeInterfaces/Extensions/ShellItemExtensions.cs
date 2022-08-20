using net.adamec.ui.AppSwitcherBar.Dto;
using net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;
using net.adamec.ui.AppSwitcherBar.Win32.Services;
using net.adamec.ui.AppSwitcherBar.Win32.Services.Shell.Properties;

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces.Extensions;

internal static class ShellItemExtensions
{
    /// <summary>
    /// Gets the typed value of the property identified by <paramref name="key"/>
    /// </summary>
    /// <typeparam name="TValue">Type of the value to retrieve</typeparam>
    /// <param name="shellItem">Shell item to retrieve the property from</param>
    /// <param name="key">Identifier of the property</param>
    /// <returns>Typed value of the property identified by <paramref name="key"/> or the default value of <typeparamref name="TValue"/></returns>
    public static TValue? GetPropertyValue<TValue>(this IShellItem2 shellItem, PropertyKey key)
    {
        return shellItem.GetPropertyValue<TValue>(key, default);
    }

    /// <summary>
    /// Gets the typed value of the property identified by <paramref name="key"/>
    /// </summary>
    /// <typeparam name="TValue">Type of the value to retrieve</typeparam>
    /// <param name="shellItem">Shell item to retrieve the property from</param>
    /// <param name="key">Identifier of the property</param>
    /// <param name="valueDefault">Default value to be returned when it can't be retrieved from property store</param>
    /// <returns>Typed value of the property identified by <paramref name="key"/> or the <paramref name="valueDefault"/></returns>
    public static TValue? GetPropertyValue<TValue>(this IShellItem2 shellItem, PropertyKey key, TValue? valueDefault)
    {
        var prop = ShellProperty.Get(shellItem, key);
        if (prop.IsValueRetrieved && prop.Value is TValue tValue) return tValue;

        return valueDefault;
    }

    /// <summary>
    /// Creates the <see cref="ShellPropertiesSubset"/> from <paramref name="shellItem"/>
    /// </summary>
    /// <param name="shellItem"><see cref="IShellItem2"/> to build the <see cref="ShellPropertiesSubset"/> from</param>
    /// <returns><see cref="ShellPropertiesSubset"/> created from <paramref name="shellItem"/> or empty <see cref="ShellPropertiesSubset"/> when <paramref name="shellItem"/> is null</returns>
    public static ShellPropertiesSubset GetProperties(this IShellItem2? shellItem)
    {
        if (shellItem is null) return new ShellPropertiesSubset(); //empty object

        var relaunchDisplayNameResource = shellItem.GetPropertyValue<string>(PropertyKey.PKEY_AppUserModel_RelaunchDisplayNameResource);
        var relaunchDisplayName =
            relaunchDisplayNameResource == null ?
                null :
                relaunchDisplayNameResource.StartsWith("@") ?
                    Resource.GetResourceString(relaunchDisplayNameResource) :
                    relaunchDisplayNameResource;

        var relaunchIconResource = shellItem.GetPropertyValue<string>(PropertyKey.PKEY_AppUserModel_RelaunchIconResource);
        var icon = relaunchIconResource == null ? null : Resource.GetResourceIcon(relaunchIconResource, IconSizeEnum.Large);

        var retVal = new ShellPropertiesSubset
        {
            ApplicationUserModelId = shellItem.GetPropertyValue<string>(PropertyKey.PKEY_AppUserModel_ID),
            RelaunchDisplayNameResource = relaunchDisplayNameResource,
            RelaunchDisplayName = relaunchDisplayName,
            RelaunchCommand = shellItem.GetPropertyValue<string>(PropertyKey.PKEY_AppUserModel_RelaunchCommand),
            RelaunchIconResource = relaunchIconResource,
            Icon = icon,
            IsDestListSeparator = shellItem.GetPropertyValue(PropertyKey.PKEY_AppUserModel_IsDestListSeparator, false),
            PreventPinning = shellItem.GetPropertyValue(PropertyKey.PKEY_AppUserModel_PreventPinning, false),
            PackageFamilyName = shellItem.GetPropertyValue<string>(PropertyKey.PKEY_AppUserModel_PackageFamilyName),
            PackageFullName = shellItem.GetPropertyValue<string>(PropertyKey.PKEY_AppUserModel_PackageFullName),
            PackageInstallPath = shellItem.GetPropertyValue<string>(PropertyKey.PKEY_AppUserModel_PackageInstallPath),
            // ReSharper disable once RedundantTypeArgumentsOfMethod
            HostEnvironment = shellItem.GetPropertyValue<uint>(PropertyKey.PKEY_AppUserModel_HostEnvironment, 0),
            ParsingName = shellItem.GetPropertyValue<string>(PropertyKey.PKEY_ParsingName),
            ParsingPath = shellItem.GetPropertyValue<string>(PropertyKey.PKEY_ParsingPath),
            LinkTargetParsingPath = shellItem.GetPropertyValue<string>(PropertyKey.PKEY_Link_TargetParsingPath),
            PackageIconResource = shellItem.GetPropertyValue<string>(PropertyKey.PKEY_Tile_SmallLogoPath)
        };

        return retVal;
    }

    /// <summary>
    /// Gets the display name of the shell item
    /// </summary>
    /// <param name="shellItem">Shell item to get the display name for</param>
    /// <returns>Display name of the shell item</returns>
    public static string GetDisplayName(this IShellItem shellItem)
    {
        return shellItem.GetDisplayName(SIGDN.NORMALDISPLAY);
    }

    /// <summary>
    /// Gets the <see cref="IPropertyStore"/> of the shell item
    /// </summary>
    /// <param name="shellItem">Shell item to get the property store for</param>
    /// <returns>Property store of the shell item or null if can't get the store</returns>
    public static IPropertyStore? GetPropertyStore(this IShellItem2 shellItem)
    {
        var hr = shellItem.GetPropertyStore(GPS.DEFAULT, typeof(IPropertyStore).GUID, out var ps);
        return hr.IsSuccess && ps is { } propertyStore ? propertyStore : null;
    }
}