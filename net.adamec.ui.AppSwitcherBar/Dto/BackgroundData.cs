
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
        /// Sorted array of known folders and their GUIDs
        /// </summary>
        public StringGuidPair[] KnownFolders { get; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="installedApplications">Array of installed applications</param>
        /// <param name="knownFolders">Sorted array of known folders and their GUIDs</param>
        public BackgroundData(InstalledApplication[] installedApplications, StringGuidPair[] knownFolders)
        {
            InstalledApplications = installedApplications;
            KnownFolders = knownFolders;
        }
    }
}
