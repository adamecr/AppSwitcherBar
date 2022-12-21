using System;
using System.Windows.Media.Imaging;

// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo

namespace net.adamec.ui.AppSwitcherBar.Dto
{
    /// <summary>
    /// Properties retrieved from entity PropertyStore
    /// </summary>
    public class ShellPropertiesSubset
    {
        /// <summary>
        /// Application User Model ID (AppUserModelID, AUMI)
        /// </summary>
        public string? ApplicationUserModelId { get; init; }
        /// <summary>
        /// Specifies the display name used for the shortcut created on the taskbar when the user chooses to pin an application to the taskbar or launch a new instance through its button's Jump List.
        /// </summary>
        /// <remarks>
        /// This property is used only if a window has an explicit Application User Model ID (AppUserModelID) (System.AppUserModel.ID, set through SHGetPropertyStoreForWindow).
        /// If the window does not have an explicit AppUserModelID, this property is ignored and the window is grouped and pinned as if it were part of the process that owns it.
        ///
        /// This property is ignored if System.AppUserModel.PreventPinning is set.
        /// This enables an application to control the grouping of its windows by assigning them explicit AppUserModelIDs but preventing those windows from being pinned.
        ///
        ///  The value of this property must be one of the following:
        ///   A plain-text string that does not point to a resource.
        ///   An indirect resource string such as "@%systemdir%\system32\shell32.dll,-19263".
        ///     Note that the '@' character is required to distinguish an indirect string from a plain-text string (described in the next bulleted paragraph).
        ///     This indirect string consists of a binary file and a resource ID of the string contained in that binary.
        ///     The '-' character before the resource ID is required.
        /// </remarks>
        public string? RelaunchDisplayNameResource { get; init; }
        /// <summary>
        /// Specifies a command that can be executed through ShellExecute to launch an application when it is pinned to the taskbar or when a new instance of the application is launched through the application's Jump List.
        /// </summary>
        /// <remarks>
        /// This property is used only if a window has an explicit Application User Model ID (AppUserModelID) (System.AppUserModel.ID, set through SHGetPropertyStoreForWindow).
        /// If the window does not have an explicit AppUserModelID, this property is ignored and the window is grouped and pinned as if it were part of the process that owns it
        /// Examples include the following:
        ///   shell:::{ ED228FDF - 9EA8 - 4870 - 83B1 - 96B02CFE0D52}
        ///   virtualhost.exe /virtualapp:12345
        ///   notepad.exe
        /// </remarks>
        public string? RelaunchCommand { get; init; }

        /// <summary>
        /// Specifies the icon used for the shortcut created on the taskbar when the user chooses to pin an application to the taskbar or launch a new instance through its button's Jump List. 
        /// </summary>
        /// <remarks>
        /// This property is ignored if System.AppUserModel.PreventPinning is set.
        /// This enables an application to control the grouping of its windows by assigning them explicit AppUserModelIDs but preventing those windows from being pinned.
        ///
        /// This should be specified in one of the following formats:
        ///   Standard resource format, such as "%systemdir%\system32\shell32.dll,-128".
        ///     The '-' character before the resource ID is required.Do not use the '@' character at the front of the path string.
        ///
        ///   Direct path to an icon file, such as "%programfiles%\Microsoft\Notepad\Notepad.ico,0".
        ///     Note that because.ico files can contain multiple icon resources, a resource ID is required in the string.
        ///     If the.ico file is a single image, use "0" (without the '-' character) as the resource ID.
        /// </remarks>
        public string? RelaunchIconResource { get; init; }
        /// <summary>
        /// Inserts a separator in the Tasks section of a Jump List. 
        /// </summary>
        public bool IsDestListSeparator { get; init; }
        /// <summary>
        /// Disables the ability of a shortcut or window to be pinned to the taskbar or the Start menu.
        /// Setting this property causes the following properties to be ignored:
        ///   System.AppUserModel.RelaunchCommand
        ///   System.AppUserModel.RelaunchDisplayNameResource
        ///   System.AppUserModel.RelaunchIconResource
        /// </summary>
        public bool PreventPinning { get; init; }

        /// <summary>
        /// Flag whether the AppUserModel has AppId (<see cref="ApplicationUserModelId"/>)
        /// </summary>
        public bool HasAppId => !string.IsNullOrEmpty(ApplicationUserModelId);

        /// <summary>
        /// Icon as <see cref="BitmapSource"/> retrieved from <see cref="RelaunchIconResource"/> if any
        /// </summary>
        public BitmapSource? Icon { get; init; }

        /// <summary>
        /// Relaunch display name retrieved from <see cref="RelaunchDisplayNameResource"/> if any
        /// </summary>
        public string? RelaunchDisplayName { get; init; }

        /// <summary>
        /// Host environment - 0 for Desktop App, 1+ for Store/UWP app
        /// </summary>
        public uint HostEnvironment { get; init; }

        /// <summary>
        /// Flag whether the application is Store/UWP app (<see cref="HostEnvironment"/> is >0)
        /// </summary>
        public bool IsStoreApp => HostEnvironment >0;

        /// <summary>
        /// Run flags of shell item (0 means not runnable, 1 is runnable). IMPORTANT: It's also 0 for Store Apps!!!
        /// </summary>
        public uint RunFlags;

        /// <summary>
        /// Flag whether the shell item is an application
        /// </summary>
        public bool IsApplication => IsStoreApp || RunFlags > 0;

        /// <summary>
        /// The Shell namespace name of an item relative to a parent folder.
        /// </summary>
        public string? ParsingName { get; init; }

        /// <summary>
        /// The Shell namespace path to the item.
        /// </summary>
        public string? ParsingPath { get; init; }
        
        /// <summary>
        /// The Shell namespace path to the target of the link item (if link)
        /// </summary>
        public string? LinkTargetParsingPath { get; init; }

        /// <summary>
        /// Link arguments (if link)
        /// </summary>
        public string? LinkArguments { get; init; }

        /// <summary>
        /// Package family name if package
        /// </summary>
        public string? PackageFamilyName { get; init; }

        /// <summary>
        /// Package full name if package
        /// </summary>
        public string? PackageFullName { get; init; }

        /// <summary>
        /// Package install path if package
        /// </summary>
        public string? PackageInstallPath { get; init; }

        /// <summary>
        /// Package icon resource
        /// </summary>
        public string? PackageIconResource { get; init; }

        /// <summary>
        /// Folder in Windows Start menu
        /// </summary>
        public string? TileSuiteDisplayName { get; init; }

        /// <summary>
        /// Keywords used for search besides the app/file name
        /// </summary>
        public string[] Keywords { get; init; }=Array.Empty<string>();

        /// <summary>
        /// Get the string representation of the object
        /// </summary>
        /// <returns>String representation of the object</returns>
        public override string ToString()
        {
            return $"{(HasAppId ? ApplicationUserModelId : "[UNKNOWN]")}, IsSeparator:{IsDestListSeparator}, NoPin:{PreventPinning}, Name:{RelaunchDisplayNameResource ?? "[NULL]"}, Icon:{RelaunchIconResource ?? "[NULL]"}, Cmd:{RelaunchCommand ?? "[NULL]"}, Pkg:{PackageFullName ?? "[NULL]"}";
        }
    }
}
