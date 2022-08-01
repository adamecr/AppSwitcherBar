// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable UnusedMember.Global

namespace net.adamec.ui.AppSwitcherBar.Win32.NativeEnums;

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