// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;

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