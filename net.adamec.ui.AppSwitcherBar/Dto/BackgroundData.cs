
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
        /// CTOR
        /// </summary>
        /// <param name="installedApplications">Array of installed applications</param>
        public BackgroundData(InstalledApplication[] installedApplications)
        {
            InstalledApplications = installedApplications;
        }
    }
}
