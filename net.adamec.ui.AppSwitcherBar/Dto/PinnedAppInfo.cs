using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;
using net.adamec.ui.AppSwitcherBar.Win32.Services;
using net.adamec.ui.AppSwitcherBar.Win32.Services.Shell;

namespace net.adamec.ui.AppSwitcherBar.Dto;

/// <summary>
/// Information about the window (task bar application)
/// </summary>
public class PinnedAppInfo : ButtonInfo
{
    /// <summary>
    /// Type of the pinned app (link/package)
    /// </summary>
    public PinnedAppTypeEnum PinnedAppType { get; }

    /// <summary>
    /// Index (order) of the pinned app
    /// </summary>
    public int PinnedAppIndex { get; }

    /// <summary>
    /// Path to the link file
    /// </summary>
    public string? LinkFile { get; }

    /// <summary>
    /// <see cref="ButtonInfo.Group"/> fallback when neither AppId not Executable is known or not set yet
    /// </summary>
    protected override string GroupFallback => (PinnedAppType == PinnedAppTypeEnum.Link ? ShellProperties.LinkTargetParsingPath : ShellProperties.ApplicationUserModelId) ?? "unknown";

    /// <summary>
    /// CTOR
    /// </summary>
    /// <param name="title">Window title</param>
    /// <param name="order">Index (order) of the pinned app</param>
    /// <param name="pinnedAppType">Type of the pinned app (link/package)</param>
    /// <param name="shellProperties">Subset of shell properties</param>
    /// <param name="appId">Application ID</param>
    /// <param name="executable">Application executable</param>
    public PinnedAppInfo(string title, int order, PinnedAppTypeEnum pinnedAppType, ShellPropertiesSubset shellProperties, string? appId,string? executable)
        : base(title, executable, null)
    {
        PinnedAppIndex = order;
        PinnedAppType = pinnedAppType;
        ShellProperties = shellProperties;
        AppId = appId;
        LinkFile = shellProperties.ParsingPath;

        if (pinnedAppType == PinnedAppTypeEnum.Link)
        {
            if (shellProperties.LinkTargetParsingPath == null) return;

            var shellItem = Shell.GetShellItemForPath(shellProperties.LinkTargetParsingPath);
            
            if (shellItem == null && shellProperties.ParsingPath != null)
                shellItem = Shell.GetShellItemForPath(shellProperties.ParsingPath);
            
            if (shellItem != null)
            {
                BitmapSource = Shell.GetShellItemBitmapSource(shellItem, 32);
            }
        }
        else
        {
            if (shellProperties.PackageFullName == null || shellProperties.PackageIconResource == null) return;
            
            var assetPath = Package.GetPackageImageAsset(shellProperties.PackageFullName, shellProperties.PackageIconResource, 32);
            if (assetPath != null && File.Exists(assetPath))
            {
                BitmapSource = new BitmapImage(new Uri(assetPath));
            }
        }
    }

    /// <summary>
    /// Sets <see cref="ButtonInfo.GroupIndex"/> and <see cref="ButtonInfo.WindowIndex"/>
    /// </summary>
    /// <param name="groupIndex">New <see cref="ButtonInfo.GroupIndex"/></param>
    /// <param name="windowIndex">New <see cref="ButtonInfo.WindowIndex"/></param>
    /// <returns>This object</returns>
    // ReSharper disable once IdentifierTypo
    public override PinnedAppInfo SetIndicies(int groupIndex, int windowIndex)
    {
        base.SetIndicies(groupIndex, windowIndex);
        return this;
    }

    /// <summary>
    /// Launches the pinned application
    /// </summary>
    public void LaunchPinnedApp(Action<Exception>? errorAction)
    {
        try
        {
            if (PinnedAppType == PinnedAppTypeEnum.Package)
            {
                Package.ActivateApplication(AppId, null, out _);
            }
            else
            {
                if (LinkFile == null || !File.Exists(LinkFile)) return; //can't do anything
                //launch link
                var startInfo = new ProcessStartInfo(LinkFile)
                {
                    UseShellExecute = true
                };
                Process.Start(startInfo);
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
        return $"{Title} #{PinnedAppIndex}({PinnedAppType}) Idx:{GroupIndex}/{WindowIndex}, A:{AppId}, E:{Executable}, L:{LinkFile}";
    }

    /// <summary>
    /// Type of the pinned application (link to desktop app or Store/UWP package)
    /// </summary>
    public enum PinnedAppTypeEnum
    {
        /// <summary>
        /// Link to desktop app
        /// </summary>
        Link,
        /// <summary>
        /// Store/UWP app package
        /// </summary>
        Package
    }
}