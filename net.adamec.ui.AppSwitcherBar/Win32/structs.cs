using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace net.adamec.ui.AppSwitcherBar.Win32
{
    /// <summary>
    /// Specifies Desktop Window Manager (DWM) thumbnail properties. Used by the <see cref="DwmApi.DwmUpdateThumbnailProperties(IntPtr, ref DWM_THUMBNAIL_PROPERTIES)"/> function
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
    }

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

    /// <summary>
    /// Contains information about a system appbar message.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct APPBARDATA
    {
        /// <summary>
        /// The size of the structure, in bytes.
        /// </summary>
        public int cbSize;
        /// <summary>
        /// The handle to the appbar window. Not all messages use this member.
        /// </summary>
        public IntPtr hWnd;
        /// <summary>
        /// An application-defined message identifier. The application uses the specified identifier for notification messages that it sends to the appbar identified by the hWnd member. This member is used when sending the <see cref="ABMsg.NEW"/> message.
        /// </summary>
        public int uCallbackMessage;
        /// <summary>
        /// A value that specifies an edge of the screen.
        /// </summary>
        public ABEdge uEdge;
        /// <summary>
        /// The bounding rectangle, in screen coordinates, of an appbar or the Windows taskbar
        /// </summary>
        public RECT rc;
        /// <summary>
        /// A message-dependent value
        /// </summary>
        public IntPtr lParam;
    }

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

    /// <summary>
    /// The MONITORINFOEX structure contains information about a display monitor.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct MONITORINFOEX
    {
        /// <summary>
        /// The size of the structure, in bytes.
        /// </summary>
        public int cbSize;
        /// <summary>
        /// A <see cref="RECT"/> structure that specifies the display monitor rectangle, expressed in virtual-screen coordinates. Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
        /// </summary>
        public RECT rcMonitor;
        /// <summary>
        /// A <see cref="RECT"/> structure that specifies the work area rectangle of the display monitor, expressed in virtual-screen coordinates. Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
        /// </summary>
        public RECT rcWork;
        /// <summary>
        /// A set of flags that represent attributes of the display monitor.
        /// </summary>
        public MONITORINFOF dwFlags;
        /// <summary>
        /// A string that specifies the device name of the monitor being used. 
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Win32Consts.CCHDEVICENAME)]
        public string szDevice;
    }
}
