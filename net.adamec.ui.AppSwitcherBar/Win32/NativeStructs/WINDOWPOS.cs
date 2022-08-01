using System;
using System.Runtime.InteropServices;
using System.Windows;
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable UnusedMember.Global

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;

/// <summary>
/// Contains information about the size and position of a window.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct WINDOWPOS
{
    /// <summary>
    /// A handle to the window.
    /// </summary>
    public IntPtr hwnd;
    /// <summary>
    /// The position of the window in Z order (front-to-back position). 
    /// </summary>
    public IntPtr hwndInsertAfter;
    /// <summary>
    /// The position of the left edge of the window.
    /// </summary>
    public int x;
    /// <summary>
    /// The position of the top edge of the window.
    /// </summary>
    public int y;
    /// <summary>
    /// The window width, in pixels.
    /// </summary>
    public int cx;
    /// <summary>
    /// The window height, in pixels.
    /// </summary>
    public int cy;
    /// <summary>
    /// The window position flags
    /// </summary>
    public int flags;

    /// <summary>
    /// Get or set bounding box within this structure
    /// </summary>
    public Rect Bounds
    {
        get => new(x, y, cx, cy);
        set
        {
            x = (int)value.X;
            y = (int)value.Y;
            cx = (int)value.Width;
            cy = (int)value.Height;
        }
    }
}