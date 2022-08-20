using System.Windows.Media.Imaging;
// ReSharper disable StringLiteralTypo

namespace net.adamec.ui.AppSwitcherBar.Dto
{
    /// <summary>
    /// Information about installed application
    /// </summary>
    public class InstalledApplication
    {
        /// <summary>
        /// Name of the installed application
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// AppUserModel ID (AppId) of the application if available
        /// </summary>
        public string? AppUserModelId { get; }
        /// <summary>
        /// Link to the application (executable) if available
        /// </summary>
        public string? Executable { get; }
        
        /// <summary>
        /// Icon of the application if available
        /// </summary>
        public BitmapSource? IconSource { get; set; }

        /// <summary>
        /// Some of the shell properties
        /// </summary>
        public ShellPropertiesSubset ShellProperties { get; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="name"> Name of the installed application</param>
        /// <param name="appUserModelId">AppUserModel ID (AppId) of the application if available</param>
        /// <param name="executable">Link to the application (executable) if available</param>
        /// <param name="iconSource">Icon of the application if available</param>
        /// <param name="shellProperties">Some of the shell properties</param>
        public InstalledApplication(string name, string? appUserModelId, string? executable, BitmapSource? iconSource, ShellPropertiesSubset shellProperties)
        {
            Name = name;
            AppUserModelId = appUserModelId;
            Executable = executable;
            IconSource = iconSource;
            ShellProperties = shellProperties;
        }

        /// <summary>
        /// Returns the string representation of the object
        /// </summary>
        /// <returns>String representation of the object</returns>
        public override string ToString()
        {
            return $"Installed application {Name}, AUMI:{AppUserModelId ?? "[Unknown]"}, Icon:{(IconSource != null ? "Yes" : "No")}, Link:{Executable ?? "[N/A]"}";
        }
    }
}
