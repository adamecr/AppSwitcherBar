using System.Runtime.InteropServices;
using System.Windows;
// ReSharper disable InconsistentNaming

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;

/// <summary>
/// The POINT structure defines the x- and y-coordinates of a point.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct POINT
{
    /// <summary>
    /// Specifies the x-coordinate of the point.
    /// </summary>
    public int x;
    /// <summary>
    /// Specifies the y-coordinate of the point.
    /// </summary>
    public int y;

    /// <summary>
    /// Explicit cast of <see cref="Point"/> to <see cref="POINT"/>
    /// </summary>
    /// <param name="p">Point</param>
    public static explicit operator POINT(Point p)
    {
        return new POINT { x = (int)p.X, y = (int)p.Y };
    }

    /// <summary>
    /// Explicit cast of <see cref="POINT"/> to <see cref="Point"/>
    /// </summary>
    /// <param name="p">Point</param>
    public static explicit operator Point(POINT p)
    {
        return new Point(p.x, p.y);
    }
}