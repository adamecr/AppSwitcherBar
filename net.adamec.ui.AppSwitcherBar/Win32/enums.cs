using System;

namespace net.adamec.ui.AppSwitcherBar.Win32
{

    /// <summary>
    /// HRESULT Wrapper    
    /// </summary>    
    internal enum HResult
    {
        /// <summary>     
        /// S_OK          
        /// </summary>    
        Ok = 0x0000,

        /// <summary>
        /// S_FALSE
        /// </summary>        
        False = 0x0001,

        /// <summary>
        /// E_INVALIDARG
        /// </summary>
        InvalidArguments = unchecked((int)0x80070057),

        /// <summary>
        /// E_OUTOFMEMORY
        /// </summary>
        OutOfMemory = unchecked((int)0x8007000E),

        /// <summary>
        /// E_NOINTERFACE
        /// </summary>
        NoInterface = unchecked((int)0x80004002),

        /// <summary>
        /// E_FAIL
        /// </summary>
        Fail = unchecked((int)0x80004005),

        /// <summary>
        /// E_ELEMENTNOTFOUND
        /// </summary>
        ElementNotFound = unchecked((int)0x80070490),

        /// <summary>
        /// TYPE_E_ELEMENTNOTFOUND
        /// </summary>
        TypeElementNotFound = unchecked((int)0x8002802B),

        /// <summary>
        /// NO_OBJECT
        /// </summary>
        NoObject = unchecked((int)0x800401E5),

        /// <summary>
        /// Win32 Error code: ERROR_CANCELLED
        /// </summary>
        Win32ErrorCanceled = 1223,

        /// <summary>
        /// ERROR_CANCELLED
        /// </summary>
        Canceled = unchecked((int)0x800704C7),

        /// <summary>
        /// The requested resource is in use
        /// </summary>
        ResourceInUse = unchecked((int)0x800700AA),

        /// <summary>
        /// The requested resources is read-only.
        /// </summary>
        AccessDenied = unchecked((int)0x80030005)
    }

    /// <summary>
    /// Edge of the screen
    /// </summary>
    internal enum ABEdge : int
    {
        /// <summary>
        /// Left edge.
        /// </summary>
        ABE_LEFT = 0,
        /// <summary>
        /// Top edge.
        /// </summary>
        ABE_TOP = 1,
        /// <summary>
        /// Right edge.
        /// </summary>
        ABE_RIGHT = 2,
        /// <summary>
        /// Bottom edge.
        /// </summary>
        ABE_BOTTOM = 3
    }

    /// <summary>
    /// APPBAR API messages
    /// </summary>
    internal enum ABMsg
    {
        /// <summary>
        /// Registers a new appbar and specifies the message identifier that the system should use to send it notification messages. An appbar should send this message before sending any other appbar messages.
        /// </summary>
        NEW = 0,
        /// <summary>
        /// Unregisters an appbar by removing it from the system's internal list. The system no longer sends notification messages to the appbar or prevents other applications from using the screen area used by the appbar.
        /// </summary>
        REMOVE = 1,
        /// <summary>
        /// Requests a size and screen position for an appbar. When the request is made, the message proposes a screen edge and a bounding rectangle for the appbar. The system adjusts the bounding rectangle so that the appbar does not interfere with the Windows taskbar or any other appbars.
        /// </summary>
        QUERYPOS = 2,
        /// <summary>
        /// Sets the size and screen position of an appbar. The message specifies a screen edge and the bounding rectangle for the appbar. The system may adjust the bounding rectangle so that the appbar does not interfere with the Windows taskbar or any other appbars.
        /// </summary>
        SETPOS = 3,
        /// <summary>
        /// Retrieves the autohide and always-on-top states of the Windows taskbar.
        /// </summary>
        GETSTATE = 4,
        /// <summary>
        /// Retrieves the bounding rectangle of the Windows taskbar.
        /// </summary>
        GETTASKBARPOS = 5,
        /// <summary>
        /// Notifies the system that an appbar has been activated. An appbar should call this message in response to the WM_ACTIVATE message.
        /// </summary>
        ACTIVATE = 6,
        /// <summary>
        /// Retrieves the handle to the autohide appbar associated with an edge of the screen. If the system has multiple monitors, the monitor that contains the primary taskbar is used.
        /// </summary>
        GETAUTOHIDEBAR = 7,
        /// <summary>
        /// Registers or unregisters an autohide appbar for a given edge of the screen. If the system has multiple monitors, the monitor that contains the primary taskbar is used.
        /// </summary>
        SETAUTOHIDEBAR = 8,
        /// <summary>
        /// Notifies the system when an appbar's position has changed. An appbar should call this message in response to the WM_WINDOWPOSCHANGED message.
        /// </summary>
        WINDOWPOSCHANGED = 9,
        /// <summary>
        /// Sets the autohide and always-on-top states of the Windows taskbar.
        /// </summary>
        SETSTATE = 10
    }

    /// <summary>
    /// APPBAR API notifications (events)
    /// </summary>
    internal enum ABNotify
    {
        /// <summary>
        /// Notifies an appbar that the taskbar's autohide or always-on-top state has changed that is, the user has selected or cleared the "Always on top" or "Auto hide" check box on the taskbar's property sheet.
        /// </summary>
        STATECHANGE = 0,
        /// <summary>
        /// Notifies an appbar when an event has occurred that may affect the appbar's size and position. Events include changes in the taskbar's size, position, and visibility state, as well as the addition, removal, or resizing of another appbar on the same side of the screen.
        /// </summary>
        POSCHANGED = 1,
        /// <summary>
        /// Notifies an appbar when a full-screen application is opening or closing. This notification is sent in the form of an application-defined message that is set by the <see cref="ABMsg.NEW"/> message.
        /// </summary>
        FULLSCREENAPP = 2,
        /// <summary>
        /// Notifies an appbar that the user has selected the Cascade, Tile Horizontally, or Tile Vertically command from the taskbar's shortcut menu.
        /// </summary>
        WINDOWARRANGE = 3
    }

    /// <summary>
    /// A set of flags that represent attributes of the display monitor.
    /// </summary>
    [Flags]
    internal enum MONITORINFOF
    {
        /// <summary>
        /// This is the primary display monitor.
        /// </summary>
        PRIMARY = 0x1
    }

    /// <summary>
    /// Identifies the dots per inch (dpi) setting for a monitor.
    /// </summary>
    [Flags]
    internal enum MONITOR_DPI_TYPE
    {
        /// <summary>
        /// The effective DPI. This value should be used when determining the correct scale factor for scaling UI elements. This incorporates the scale factor set by the user for this specific display.
        /// </summary>
        MDT_EFFECTIVE_DPI = 0,
        /// <summary>
        /// The angular DPI. This DPI ensures rendering at a compliant angular resolution on the screen. This does not include the scale factor set by the user for this specific display.
        /// </summary>
        MDT_ANGULAR_DPI = 1,
        /// <summary>
        /// The raw DPI. This value is the linear DPI of the screen as measured on the screen itself. Use this value when you want to read the pixel density and not the recommended scaling setting. This does not include the scale factor set by the user for this specific display and is not guaranteed to be a supported DPI value.
        /// </summary>
        MDT_RAW_DPI = 2,
        /// <summary>
        /// The default DPI setting for a monitor is MDT_EFFECTIVE_DPI.
        /// </summary>
        MDT_DEFAULT = MDT_EFFECTIVE_DPI
    }

    /// <summary>
    /// Window sizing and positioning flags
    /// </summary>
    [Flags]
    internal enum SetWindowPosFlags : uint
    {
        /// <summary>If the calling thread and the thread that owns the window are attached to different input queues,
        /// the system posts the request to the thread that owns the window. This prevents the calling thread from
        /// blocking its execution while other threads process the request.</summary>
        /// <remarks>SWP_ASYNCWINDOWPOS</remarks>
        AsynchronousWindowPosition = 0x4000,
        /// <summary>Prevents generation of the WM_SYNCPAINT message.</summary>
        /// <remarks>SWP_DEFERERASE</remarks>
        DeferErase = 0x2000,
        /// <summary>Draws a frame (defined in the window's class description) around the window.</summary>
        /// <remarks>SWP_DRAWFRAME</remarks>
        DrawFrame = 0x0020,
        /// <summary>Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to
        /// the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE
        /// is sent only when the window's size is being changed.</summary>
        /// <remarks>SWP_FRAMECHANGED</remarks>
        FrameChanged = 0x0020,
        /// <summary>Hides the window.</summary>
        /// <remarks>SWP_HIDEWINDOW</remarks>
        HideWindow = 0x0080,
        /// <summary>Does not activate the window. If this flag is not set, the window is activated and moved to the
        /// top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter
        /// parameter).</summary>
        /// <remarks>SWP_NOACTIVATE</remarks>
        DoNotActivate = 0x0010,
        /// <summary>Discards the entire contents of the client area. If this flag is not specified, the valid
        /// contents of the client area are saved and copied back into the client area after the window is sized or
        /// repositioned.</summary>
        /// <remarks>SWP_NOCOPYBITS</remarks>
        DoNotCopyBits = 0x0100,
        /// <summary>Retains the current position (ignores X and Y parameters).</summary>
        /// <remarks>SWP_NOMOVE</remarks>
        IgnoreMove = 0x0002,
        /// <summary>Does not change the owner window's position in the Z order.</summary>
        /// <remarks>SWP_NOOWNERZORDER</remarks>
        DoNotChangeOwnerZOrder = 0x0200,
        /// <summary>Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to
        /// the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent
        /// window uncovered as a result of the window being moved. When this flag is set, the application must
        /// explicitly invalidate or redraw any parts of the window and parent window that need redrawing.</summary>
        /// <remarks>SWP_NOREDRAW</remarks>
        DoNotRedraw = 0x0008,
        /// <summary>Same as the SWP_NOOWNERZORDER flag.</summary>
        /// <remarks>SWP_NOREPOSITION</remarks>
        DoNotReposition = 0x0200,
        /// <summary>Prevents the window from receiving the WM_WINDOWPOSCHANGING message.</summary>
        /// <remarks>SWP_NOSENDCHANGING</remarks>
        DoNotSendChangingEvent = 0x0400,
        /// <summary>Retains the current size (ignores the cx and cy parameters).</summary>
        /// <remarks>SWP_NOSIZE</remarks>
        IgnoreResize = 0x0001,
        /// <summary>Retains the current Z order (ignores the hWndInsertAfter parameter).</summary>
        /// <remarks>SWP_NOZORDER</remarks>
        IgnoreZOrder = 0x0004,
        /// <summary>Displays the window.</summary>
        /// <remarks>SWP_SHOWWINDOW</remarks>
        ShowWindow = 0x0040,
    }
}
