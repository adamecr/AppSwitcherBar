
namespace net.adamec.ui.AppSwitcherBar.Dto
{
    /// <summary>
    /// Container for data retrieved at background process
    /// </summary>
    internal class BackgroundData
    {
        /// <summary>
        /// Array of installed applications
        /// </summary>
        public InstalledApplication[] InstalledApplications { get; }

        /// <summary>
        /// Information about the applications pinned in the startmenu
        /// </summary>
        public PinnedAppInfo[] StartPinnedApplications { get; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="installedApplications">Array of installed applications</param>
        /// <param name="startPinnedApplications">Information about the applications pinned in the startmenu</param>
        public BackgroundData(InstalledApplication[] installedApplications, PinnedAppInfo[] startPinnedApplications)
        {
            InstalledApplications = installedApplications;
            StartPinnedApplications = startPinnedApplications;
        }

        /// <summary>
        /// Returns string representation of the object
        /// </summary>
        /// <returns>String representation of the object</returns>
        public override string ToString()
        {
            return $"{InstalledApplications?.Length ?? 0} installed applications, {StartPinnedApplications?.Length ?? 0} Start pinned applications";
        }
    }
}
