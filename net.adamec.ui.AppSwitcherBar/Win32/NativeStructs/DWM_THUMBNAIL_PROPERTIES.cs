using net.adamec.ui.AppSwitcherBar.Win32.NativeMethods;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;

/// <summary>
/// Specifies Desktop Window Manager (DWM) thumbnail properties. Used by the <see cref="DwmApi.DwmUpdateThumbnailProperties"/> function
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct DWM_THUMBNAIL_PROPERTIES
{
    /// <summary>
    /// A bitwise combination of DWM thumbnail constant values that indicates which members of this structure are set.
    /// </summary>
    public int dwFlags;
    /// <summary>
    /// The area in the destination window where the thumbnail will be rendered.
    /// </summary>
    public RECT rcDestination;
    /// <summary>
    /// The region of the source window to use as the thumbnail. By default, the entire window is used as the thumbnail.
    /// </summary>
    public RECT rcSource;
    /// <summary>
    /// The opacity with which to render the thumbnail. 0 is fully transparent while 255 is fully opaque. The default value is 255.
    /// </summary>
    public byte opacity;
    /// <summary>
    /// TRUE to make the thumbnail visible; otherwise, FALSE. The default is FALSE.
    /// </summary>
    public bool fVisible;
    /// <summary>
    /// TRUE to use only the thumbnail source's client area; otherwise, FALSE. The default is FALSE.
    /// </summary>
    public bool fSourceClientAreaOnly;
}