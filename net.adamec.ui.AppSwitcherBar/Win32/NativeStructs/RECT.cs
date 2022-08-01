using System.Runtime.InteropServices;
using System.Windows;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;

/// <summary>
/// The RECT structure defines the coordinates of the upper-left and lower-right corners of a rectangle.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct RECT
{
    /// <summary>
    /// Empty rectangle in origin (all fields are set to 0)
    /// </summary>
    public static RECT Empty = new(0, 0, 0, 0);

    /// <summary>
    /// Specifies the x-coordinate of the upper-left corner of the rectangle.
    /// </summary>
    public int left;
    /// <summary>
    /// Specifies the y-coordinate of the upper-left corner of the rectangle.
    /// </summary>
    public int top;
    /// <summary>
    /// Specifies the x-coordinate of the lower-right corner of the rectangle.
    /// </summary>
    public int right;
    /// <summary>
    /// Specifies the y-coordinate of the lower-right corner of the rectangle.
    /// </summary>
    public int bottom;

    /// <summary>
    /// CTOR
    /// </summary>
    /// <param name="left">Specifies the x-coordinate of the upper-left corner of the rectangle</param>
    /// <param name="top">Specifies the y-coordinate of the upper-left corner of the rectangle</param>
    /// <param name="right">Specifies the x-coordinate of the lower-right corner of the rectangle</param>
    /// <param name="bottom">Specifies the y-coordinate of the lower-right corner of the rectangle</param>
    public RECT(int left, int top, int right, int bottom)
    {
        this.left = left;
        this.top = top;
        this.right = right;
        this.bottom = bottom;
    }

    /// <summary>
    /// Gets the width of the rectangle
    /// </summary>
    public int Width => right - left;
    /// <summary>
    /// Gets the height of the rectangle
    /// </summary>
    public int Height => bottom - top;

    /// <summary>
    /// Gets the <see cref="Size"/> of the rectangle
    /// </summary>
    public Size Size => new(Width, Height);

    /// <summary>
    /// Flag whether the rectangle is empty (has width and/or height is zero or negative)
    /// </summary>
    public bool IsEmpty => left >= right || top >= bottom;

    /// <summary>
    /// Explicit cast of <see cref="RECT"/> to <see cref="Int32Rect"/>
    /// </summary>
    /// <param name="r">Rectangle</param>
    public static explicit operator Int32Rect(RECT r)
    {
        return new Int32Rect(r.left, r.top, r.Width, r.Height);
    }

    /// <summary>
    /// Explicit cast of <see cref="RECT"/> to <see cref="Rect"/>
    /// </summary>
    /// <param name="r">Rectangle</param>
    public static explicit operator Rect(RECT r)
    {
        return new Rect(r.left, r.top, r.Width, r.Height);
    }

    /// <summary>
    /// Explicit cast of <see cref="Rect"/> to <see cref="RECT"/>
    /// </summary>
    /// <param name="r">Rectangle</param>
    public static explicit operator RECT(Rect r)
    {
        return new RECT((int)r.Left, (int)r.Top, (int)r.Right, (int)r.Bottom);
    }

    /// <summary>
    /// Returns the string representation of the structure
    /// </summary>
    /// <returns>String representation of the structure</returns>
    public override string ToString()
    {
        return $"{left},{top},{Width},{Height}";
    }
}