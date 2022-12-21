using net.adamec.ui.AppSwitcherBar.Dto;

// ReSharper disable CommentTypo

namespace net.adamec.ui.AppSwitcherBar.Win32.Services.Pins
{
    /// <summary>
    /// Encapsulates the Windows task bar and start menu pinned apps functionality
    /// </summary>
    public interface IPinsService
    {
        /// <summary>
        /// Refresh (reload) the taskbar pinned applications
        /// </summary>
        PinnedAppInfo[] RefreshTaskbarPins();

        /// <summary>
        /// Refresh (reload) the Start pinned applications
        /// </summary>
        PinnedAppInfo[] RefreshStartPins();
    }
}
