using net.adamec.ui.AppSwitcherBar.Dto;
using net.adamec.ui.AppSwitcherBar.Win32.Services;
using net.adamec.ui.AppSwitcherBar.Win32.Services.Shell.Properties;

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces.Extensions;

/// <summary>
/// Extension methods for <see cref="IPropertyStore"/>
/// </summary>
internal static class PropertyStoreExtensions
{
    /// <summary>
    /// Gets the typed value of the property identified by <paramref name="key"/>
    /// </summary>
    /// <typeparam name="TValue">Type of the value to retrieve</typeparam>
    /// <param name="propertyStore">Property store to retrieve the property from</param>
    /// <param name="key">Identifier of the property</param>
    /// <returns>Typed value of the property identified by <paramref name="key"/> or the default value of <typeparamref name="TValue"/></returns>
    public static TValue? GetPropertyValue<TValue>(this IPropertyStore propertyStore, PropertyKey key)
    {
        return propertyStore.GetPropertyValue<TValue>(key, default);
    }

    /// <summary>
    /// Gets the typed value of the property identified by <paramref name="key"/>
    /// </summary>
    /// <typeparam name="TValue">Type of the value to retrieve</typeparam>
    /// <param name="propertyStore">Property store to retrieve the property from</param>
    /// <param name="key">Identifier of the property</param>
    /// <param name="valueDefault">Default value to be returned when it can't be retrieved from property store</param>
    /// <returns>Typed value of the property identified by <paramref name="key"/> or the <paramref name="valueDefault"/></returns>
    public static TValue? GetPropertyValue<TValue>(this IPropertyStore propertyStore, PropertyKey key, TValue? valueDefault)
    {
        var prop = ShellProperty.Get(propertyStore, key);
        if (prop.IsValueRetrieved && prop.Value is TValue tValue) return tValue;

        return valueDefault;
    }

    /// <summary>
    /// Creates the <see cref="ShellPropertiesSubset"/> from <paramref name="propertyStore"/>
    /// </summary>
    /// <param name="propertyStore"><see cref="IPropertyStore"/> to build the <see cref="ShellPropertiesSubset"/> from</param>
    /// <returns><see cref="ShellPropertiesSubset"/> created from <paramref name="propertyStore"/> or empty <see cref="ShellPropertiesSubset"/> when <paramref name="propertyStore"/> is null</returns>
    public static ShellPropertiesSubset GetProperties(this IPropertyStore? propertyStore)
    {
        if (propertyStore is null) return new ShellPropertiesSubset(); //empty object

        var relaunchDisplayNameResource = propertyStore.GetPropertyValue<string>(PropertyKey.PKEY_AppUserModel_RelaunchDisplayNameResource);
        var relaunchDisplayName =
            relaunchDisplayNameResource == null ?
                null :
                relaunchDisplayNameResource.StartsWith("@") ?
                    Resource.GetResourceString(relaunchDisplayNameResource) :
                    relaunchDisplayNameResource;

        var relaunchIconResource = propertyStore.GetPropertyValue<string>(PropertyKey.PKEY_AppUserModel_RelaunchIconResource);
        var icon = relaunchIconResource == null ? null : Resource.GetResourceIcon(relaunchIconResource, IconSizeEnum.Large);

        var retVal = new ShellPropertiesSubset
        {
            ApplicationUserModelId = propertyStore.GetPropertyValue<string>(PropertyKey.PKEY_AppUserModel_ID),
            RelaunchDisplayNameResource = relaunchDisplayNameResource,
            RelaunchDisplayName = relaunchDisplayName,
            RelaunchCommand = propertyStore.GetPropertyValue<string>(PropertyKey.PKEY_AppUserModel_RelaunchCommand),
            RelaunchIconResource = relaunchIconResource,
            Icon = icon,
            IsDestListSeparator = propertyStore.GetPropertyValue(PropertyKey.PKEY_AppUserModel_IsDestListSeparator, false),
            PreventPinning = propertyStore.GetPropertyValue(PropertyKey.PKEY_AppUserModel_PreventPinning, false),
            PackageFamilyName = propertyStore.GetPropertyValue<string>(PropertyKey.PKEY_AppUserModel_PackageFamilyName),
            PackageFullName = propertyStore.GetPropertyValue<string>(PropertyKey.PKEY_AppUserModel_PackageFullName),
            PackageInstallPath = propertyStore.GetPropertyValue<string>(PropertyKey.PKEY_AppUserModel_PackageInstallPath),
            // ReSharper disable once RedundantTypeArgumentsOfMethod
            HostEnvironment = propertyStore.GetPropertyValue<uint>(PropertyKey.PKEY_AppUserModel_HostEnvironment, 0),
            ParsingName = propertyStore.GetPropertyValue<string>(PropertyKey.PKEY_ParsingName),
            ParsingPath = propertyStore.GetPropertyValue<string>(PropertyKey.PKEY_ParsingPath),
            LinkTargetParsingPath = propertyStore.GetPropertyValue<string>(PropertyKey.PKEY_Link_TargetParsingPath),
            LinkArguments = propertyStore.GetPropertyValue<string>(PropertyKey.PKEY_Link_Arguments),
            PackageIconResource = propertyStore.GetPropertyValue<string>(PropertyKey.PKEY_Tile_SmallLogoPath),
            RunFlags = propertyStore.GetPropertyValue<uint>(PropertyKey.PKEY_AppUserModel_RunFlags)
        };

        return retVal;
    }
}