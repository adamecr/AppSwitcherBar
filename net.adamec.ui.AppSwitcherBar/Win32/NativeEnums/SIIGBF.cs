using System;
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;

[Flags]
internal enum SIIGBF
{
    /// <summary>
    /// SIIGBF_RESIZETOFIT (0x00000000)
    /// Shrink the bitmap as necessary to fit, preserving its aspect ratio.
    /// </summary>
    ResizeToFit = 0,
    /// <summary>
    /// SIIGBF_BIGGERSIZEOK (0x00000001)
    /// Passed by callers if they want to stretch the returned image themselves. For example, if the caller passes an icon size of 80x80, a 96x96 thumbnail could be returned. This action can be used as a performance optimization if the caller expects that they will need to stretch the image. Note that the Shell implementation of IShellItemImageFactory performs a GDI stretch blit. If the caller wants a higher quality image stretch than provided through that mechanism, they should pass this flag and perform the stretch themselves.
    /// </summary>
    BiggerSizeOk = 1,
    /// <summary>
    /// SIIGBF_MEMORYONLY (0x00000002)
    /// Return the item only if it is already in memory. Do not access the disk even if the item is cached. Note that this only returns an already-cached icon and can fall back to a per-class icon if an item has a per-instance icon that has not been cached.Retrieving a thumbnail, even if it is cached, always requires the disk to be accessed, so GetImage should not be called from the UI thread without passing SIIGBF_MEMORYONLY.
    /// </summary>
    MemoryOnly = 2,
    /// <summary>
    /// SIIGBF_ICONONLY (0x00000004)
    /// Return only the icon, never the thumbnail.
    /// </summary>
    IconOnly = 4,
    /// <summary>
    /// SIIGBF_THUMBNAILONLY (0x00000008)
    /// Return only the thumbnail, never the icon. Note that not all items have thumbnails, so SIIGBF_THUMBNAILONLY will cause the method to fail in these cases.
    /// </summary>
    ThumbnailOnly = 8,
    /// <summary>
    /// SIIGBF_INCACHEONLY (0x00000010)
    /// Allows access to the disk, but only to retrieve a cached item. This returns a cached thumbnail if it is available. If no cached thumbnail is available, it returns a cached per-instance icon but does not extract a thumbnail or icon.
    /// </summary>
    InCacheOnly = 16,
    /// <summary>
    /// SIIGBF_CROPTOSQUARE (0x00000020)
    /// Introduced in Windows 8. If necessary, crop the bitmap to a square.
    /// </summary>
    CropToSquare = 32,
    /// <summary>
    /// SIIGBF_WIDETHUMBNAILS (0x00000040)
    /// Introduced in Windows 8. Stretch and crop the bitmap to a 0.7 aspect ratio.
    /// </summary>
    WideThumbnails = 64,
    /// <summary>
    ///  SIIGBF_ICONBACKGROUND (0x00000080)
    /// Introduced in Windows 8. If returning an icon, paint a background using the associated app's registered background color.
    /// </summary>
    IconBackground = 128,
    /// <summary>
    ///  SIIGBF_SCALEUP(0x00000100)
    /// Introduced in Windows 8. If necessary, stretch the bitmap so that the height and width fit the given size.
    /// </summary>
    ScaleUp = 256

}