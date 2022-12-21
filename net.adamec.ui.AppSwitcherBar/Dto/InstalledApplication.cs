using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;
using net.adamec.ui.AppSwitcherBar.Dto.Search;
using net.adamec.ui.AppSwitcherBar.Win32.Services;

// ReSharper disable StringLiteralTypo

namespace net.adamec.ui.AppSwitcherBar.Dto
{
    /// <summary>
    /// Information about installed application
    /// </summary>
    public class InstalledApplication:ISearchable
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
        /// Flag whether the item is runnable application
        /// </summary>
        public bool IsApplication { get; }

        /// <summary>
        /// Flag whether the item is virtual application - shell target
        /// like my computer, this PC, file explorer - shell:::{guid}
        /// the <see cref="Executable"/> should be ::{guid}
        /// </summary>
        private bool IsShellTarget { get; }

        /// <summary>
        /// Folder in windows Start menu (if applicable)
        /// </summary>
        public string? StartMenuFolder { get; }

        /// <summary>
        /// Category in the application list - first char from Folder or name if not in folder, "#" for numbers, "~" for non digit/letter
        /// </summary>
        public string AppListCategory { get; }

        /// <summary>
        /// Folder in the application list - empty string when not in folder
        /// </summary>
        public string AppListFolder { get; }

        /// <summary>
        /// Application list sort key - category+folder+name
        /// </summary>
        public string AppListSortKey { get; }

        /// <summary>
        /// Application run statistics
        /// </summary>
        public RunStats RunStats { get; } = new();

        /// <summary>
        /// Search sort key
        /// </summary>
        public string SearchSortKey
        {
            get
            {
                var modificationTime = DateTime.MinValue;

                if (!string.IsNullOrEmpty(Executable) && File.Exists(Executable))
                {
                    modificationTime = File.GetLastWriteTime(Executable);
                }
                if (ShellProperties.IsStoreApp && !string.IsNullOrEmpty(ShellProperties.PackageInstallPath) && Directory.Exists(ShellProperties.PackageInstallPath))
                {
                    modificationTime = Directory.GetLastWriteTime(ShellProperties.PackageInstallPath);
                }

                return RunStats.BuildStandardSearchSortKey(modificationTime);
            }
        }


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

            IsShellTarget = executable?.StartsWith("::{") ?? false;
            IsApplication = shellProperties.IsApplication || IsShellTarget;
            StartMenuFolder = shellProperties.TileSuiteDisplayName;

            AppListFolder = StartMenuFolder ?? string.Empty;

            var appListCategoryChar = StartMenuFolder == null ? name[..1][0] : AppListFolder[..1][0];

            if (char.IsLetter(appListCategoryChar))
            {
                AppListCategory = appListCategoryChar.ToString().ToUpper();
            }
            else if (char.IsDigit(appListCategoryChar))
            {
                AppListCategory = "#";
            }
            else
            {
                AppListCategory = "~";
            }


            AppListSortKey = $"{AppListCategory}{AppListFolder}{name}";

        }

        /// <summary>
        /// Launches the installed application
        /// </summary>
        public void LaunchInstalledApp(Action<Exception>? errorAction)
        {
            try
            {
                if (ShellProperties.IsStoreApp)
                {
                    Package.ActivateApplication(AppUserModelId, null, out _);
                    RunStats.UpdateLaunched();
                }
                else if (IsShellTarget)
                {
                    if (Executable == null)
                    {
                        errorAction?.Invoke(new FileNotFoundException("Executable shell target is null"));
                        return; //can't do anything
                    }
                    //launch 
                    var startInfo = new ProcessStartInfo($"shell:{Executable}")
                    {
                        Arguments = ShellProperties.LinkArguments,
                        UseShellExecute = true
                    };
                    Process.Start(startInfo);
                    RunStats.UpdateLaunched();
                }
                else
                {
                    if (Executable == null || !File.Exists(Executable))
                    {
                        errorAction?.Invoke(new FileNotFoundException($"Executable file doesn't exist: {Executable ?? "[NULL] Executable"}"));
                        return; //can't do anything
                    }
                    //launch link
                    var startInfo = new ProcessStartInfo(Executable)
                    {
                        Arguments = ShellProperties.LinkArguments,
                        WorkingDirectory = Path.GetDirectoryName(Executable),
                        UseShellExecute = true
                    };
                    Process.Start(startInfo);
                    RunStats.UpdateLaunched();
                }
            }
            catch (Exception exception)
            {
                errorAction?.Invoke(exception);
            }
        }

        /// <summary>
        /// Returns the string representation of the object
        /// </summary>
        /// <returns>String representation of the object</returns>
        public override string ToString()
        {
            return $"Installed application {Name}, AUMI:{AppUserModelId ?? "[Unknown]"}, IsApp:{IsApplication}, Icon:{(IconSource != null ? "Yes" : "No")}, Link:{Executable ?? "[N/A]"}";
        }
    }
}
