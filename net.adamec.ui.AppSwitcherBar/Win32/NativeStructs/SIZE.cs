using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;

/// <summary>
/// The SIZE structure specifies the width and height of a rectangle.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct SIZE
{
    /// <summary>
    /// Specifies the rectangle's width. The units depend on which function uses this.
    /// </summary>
    public int x;
    /// <summary>
    /// Specifies the rectangle's height. The units depend on which function uses this.
    /// </summary>
    public int y;

    /// <summary>
    /// CTOR
    /// </summary>
    /// <param name="width">Specifies the rectangle's width. The units depend on which function uses this.</param>
    /// <param name="height">Specifies the rectangle's height. The units depend on which function uses this.</param>
    public SIZE(int width, int height)
    {
        x = width;
        y = height;
    }

    /// <summary>
    /// CTOR
    /// </summary>
    /// <param name="size">Specifies the rectangle's width and height. The units depend on which function uses this.</param>
    public SIZE(int size)
    {
        x = size;
        y = size;
    }
}