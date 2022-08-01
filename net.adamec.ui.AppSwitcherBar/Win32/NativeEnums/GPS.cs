using System;
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;

/// <summary>
/// Flags that modify the property store object retrieved by methods that create a property store
/// </summary>
[Flags]
internal enum GPS
{
    /// <summary>
    /// Return a read-only property store that contains all properties. Slow items (offline files) are not opened.
    /// </summary>
    DEFAULT = 0x00000000,
    /// <summary>
    /// Meaning to a calling process: Include only properties directly from the property handler, which opens the file on the disk, network, or device.
    /// Meaning to a file folder: Only include properties directly from the handler.
    /// Meaning to other folders: When delegating to a file folder, pass this flag on to the file folder; do not do any multiplexing (MUX).
    /// When not delegating to a file folder, ignore this flag instead of returning a failure code.
    /// Combination with other flags: Cannot be combined with GPS_TEMPORARY, GPS_FASTPROPERTIESONLY, or GPS_BESTEFFORT.
    /// </summary>
    HANDLERPROPERTIESONLY = 0x00000001,
    /// <summary>
    /// Meaning to a calling process: Can write properties to the item. Note: The store may contain fewer properties than a read-only store.
    /// Meaning to a file folder: ReadWrite.
    /// Meaning to other folders: ReadWrite. Note: When using default MUX, return a single unmultiplexed store because the default MUX does not support ReadWrite.
    /// Combination with other flags: Cannot be combined with GPS_TEMPORARY, GPS_FASTPROPERTIESONLY, GPS_BESTEFFORT, or GPS_DELAYCREATION.Implies GPS_HANDLERPROPERTIESONLY.
    /// </summary>
    READWRITE = 0x00000002,
    /// <summary>
    /// Meaning to a calling process: Provides a writable store, with no initial properties, that exists for the lifetime of the Shell item instance; basically, a property bag attached to the item instance.
    /// Meaning to a file folder: Not applicable. Handled by the Shell item.
    /// Meaning to other folders: Not applicable. Handled by the Shell item.
    /// Combination with other flags: Cannot be combined with any other flag. Implies GPS_READWRITE.
    /// </summary>
    TEMPORARY = 0x00000004,
    /// <summary>
    /// Meaning to a calling process: Provides a store that does not involve reading from the disk or network. Note: Some values may be different, or missing, compared to a store without this flag.
    /// Meaning to a file folder: Include the "innate" and "fallback" stores only. Do not load the handler.
    /// Meaning to other folders: Include only properties that are available in memory or can be computed very quickly (no properties from disk, network, or peripheral IO devices). This is normally only data sources from the IDLIST. When delegating to other folders, pass this flag on to them.
    /// Combination with other flags: Cannot be combined with GPS_TEMPORARY, GPS_READWRITE, GPS_HANDLERPROPERTIESONLY, or GPS_DELAYCREATION.
    /// </summary>
    FASTPROPERTIESONLY = 0x00000008,
    /// <summary>
    /// Meaning to a calling process: Open a slow item (offline file) if necessary.
    /// Meaning to a file folder: Retrieve a file from offline storage, if necessary. Note: Without this flag, the handler is not created for offline files.
    /// Meaning to other folders: Do not return any properties that are very slow.
    /// Combination with other flags: Cannot be combined with GPS_TEMPORARY or GPS_FASTPROPERTIESONLY.
    /// </summary>
    OPENSLOWITEM = 0x00000010,
    /// <summary>
    /// Meaning to a calling process: Delay memory-intensive operations, such as file access, until a property is requested that requires such access.
    /// Meaning to a file folder: Do not create the handler until needed; for example, either GetCount/GetAt or GetValue, where the innate store does not satisfy the request. Note: GetValue might fail due to file access problems.
    /// Meaning to other folders: If the folder has memory-intensive properties, such as delegating to a file folder or network access, it can optimize performance by supporting IDelayedPropertyStoreFactory and splitting up its properties into a fast and a slow store. It can then use delayed MUX to recombine them.
    /// Combination with other flags: Cannot be combined with GPS_TEMPORARY or GPS_READWRITE.
    /// </summary>
    DELAYCREATION = 0x00000020,
    /// <summary>
    /// Meaning to a calling process: Succeed at getting the store, even if some properties are not returned. Note: Some values may be different, or missing, compared to a store without this flag.
    /// Meaning to a file folder: Succeed and return a store, even if the handler or innate store has an error during creation. Only fail if substores fail.
    /// Meaning to other folders: Succeed on getting the store, even if some properties are not returned.
    /// Combination with other flags: Cannot be combined with GPS_TEMPORARY, GPS_READWRITE, or GPS_HANDLERPROPERTIESONLY.
    /// </summary>
    BESTEFFORT = 0x00000040,
    /// <summary>
    /// Windows 7 and later. Callers should use this flag only if they are already holding an opportunistic lock (oplock) on the file because without an oplock, the bind operation cannot continue.
    /// By default, the Shell requests an oplock on a file before binding to the property handler. This flag disables the default behavior.
    /// </summary>
    NO_OPLOCK = 0x00000080,
    /// <summary>
    /// Windows 8 and later. Use this flag to retrieve only properties from the indexer for WDS results.
    /// </summary>
    PREFERQUERYPROPERTIES = 0x100,
    /// <summary>
    /// Include properties from the file's secondary stream.
    /// </summary>
    EXTRINSICPROPERTIES = 0x200,
    /// <summary>
    /// Include only properties from the file's secondary stream.
    /// </summary>
    EXTRINSICPROPERTIESONLY = 0x400,
    VOLATILEPROPERTIES= 0x800,
    GPS_VOLATILEPROPERTIESONLY= 0x1000,
    
    /// <summary>
    /// Mask for valid GETPROPERTYSTOREFLAGS values.
    /// </summary>
    MASK_VALID = 0x000000FF,
}