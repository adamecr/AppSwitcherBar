using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using System;
using System.Runtime.InteropServices;
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;

/// <summary>
/// Contains information about a file object.
/// </summary>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
internal struct SHFILEINFO
{
    /// <summary>
    /// A handle to the icon that represents the file. You are responsible for destroying this handle with DestroyIcon when you no longer need it.
    /// </summary>
    public IntPtr hIcon;
    /// <summary>
    /// The index of the icon image within the system image list.
    /// </summary>
    public int iIcon;
    /// <summary>
    /// An array of values that indicates the attributes of the file object. For information about these values, see the IShellFolder::GetAttributesOf method.
    /// </summary>
    public uint dwAttributes;
    /// <summary>
    /// A string that contains the name of the file as it appears in the Windows Shell, or the path and file name of the file that contains the icon representing the file.
    /// </summary>
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Win32Consts.MAX_PATH)]
    public string szDisplayName;
    /// <summary>
    /// A string that describes the type of file.
    /// </summary>
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Win32Consts.MAX_TYPE)]
    public string szTypeName;

    /// <summary>
    /// CTOR
    /// </summary>
    public SHFILEINFO()
    {
        hIcon = IntPtr.Zero;
        iIcon = 0;
        dwAttributes = 0;
        szDisplayName = "";
        szTypeName = "";
    }
}