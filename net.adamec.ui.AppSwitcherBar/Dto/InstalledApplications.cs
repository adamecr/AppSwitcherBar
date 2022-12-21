using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable CommentTypo

namespace net.adamec.ui.AppSwitcherBar.Dto;

/// <summary>
/// Container with information of installed applications ("apps")
/// and other items registered in Windows AppsFolder/Start menu ("documents")
/// </summary>
public class InstalledApplications
{
    /// <summary>
    /// Installed applications with and by AppUserModelId
    /// </summary>
    private readonly Dictionary<string, InstalledApplication> appsByAppId = new();
    /// <summary>
    /// Installed applications with and by Link (executable)
    /// </summary>
    private readonly Dictionary<string, InstalledApplication> appsByAppLink = new();

    /// <summary>
    /// Installed applications with and by Package Family name 
    /// </summary>
    private readonly Dictionary<string, InstalledApplication> appsByPackageAppId = new();

    /// <summary>
    /// All installed applications
    /// </summary>
    private readonly List<InstalledApplication> allApps = new();

    /// <summary>
    /// All items that are not applications ("documents")
    /// </summary>
    private readonly List<InstalledApplication> allDocuments = new();

    /// <summary>
    /// Add <paramref name="app"/> into the container
    /// </summary>
    /// <param name="app">Information about installed application</param>
    public void Add(InstalledApplication app)
    {
        if (app.IsApplication)
        {
            allApps.Add(app);
            if (!string.IsNullOrEmpty(app.AppUserModelId)) appsByAppId[app.AppUserModelId.ToLowerInvariant()] = app;
            if (!string.IsNullOrEmpty(app.Executable)) appsByAppLink[app.Executable.ToLowerInvariant()] = app;
            if (!string.IsNullOrEmpty(app.ShellProperties.ApplicationUserModelId))
                appsByPackageAppId[app.ShellProperties.ApplicationUserModelId] = app;
        }
        else
        {
            allDocuments.Add(app);
        }
    }

    /// <summary>
    /// Clear the container
    /// </summary>
    public void Clear()
    {
        allApps.Clear();
        appsByAppId.Clear();
        appsByAppLink.Clear();
        appsByPackageAppId.Clear();
        allDocuments.Clear();
    }


    /// <summary>
    /// Gets the AppUserModel ID (AppId) of the application.
    /// When there is an installed application with <paramref name="executableFullPath"/> as <see cref="InstalledApplication.Executable"/> and
    /// having the <see cref="InstalledApplication.AppUserModelId"/>, the AUMI is returned and <paramref name="isMatch"/> is set to true.
    /// Otherwise <paramref name="executableFullPath"/> is returned and <paramref name="isMatch"/> is set to false
    /// </summary>
    /// <param name="executableFullPath">Full path to application executable</param>
    /// <param name="isMatch">Flag whether the AppId have been found or whether the executable path is gonna be used as AppId</param>
    /// <returns>AppUserModel ID (AppId) of the application</returns>
    public string GetAppIdFromExecutable(string executableFullPath, out bool isMatch)
    {
        isMatch = false;
        if (!appsByAppLink.TryGetValue(executableFullPath.ToLowerInvariant(), out var application) ||
            string.IsNullOrWhiteSpace(application.AppUserModelId)) return executableFullPath;

        isMatch = true;
        return application.AppUserModelId;
    }

    /// <summary>
    /// Gets the <see cref="InstalledApplications"/> information.
    /// </summary>
    /// <param name="appId">Application User Model ID</param>
    /// <returns><see cref="InstalledApplications"/> information or null when unknown</returns>
    public InstalledApplication? GetInstalledApplicationFromAppId(string appId)
    {
        return appsByAppId.TryGetValue(appId.ToLowerInvariant(), out var application) ? application : null;
    }

    /// <summary>
    /// Gets the <see cref="InstalledApplications"/> information.
    /// </summary>
    /// <param name="executableFullPath">Full path to application executable</param>
    /// <returns><see cref="InstalledApplications"/> information or null when unknown</returns>
    public InstalledApplication? GetInstalledApplicationFromExecutable(string executableFullPath)
    {
        return appsByAppLink.TryGetValue(executableFullPath.ToLowerInvariant(), out var application) ? application : null;
    }

    /// <summary>
    /// Gets the package full name for installed application
    /// </summary>
    /// <param name="appId">Application User Model ID of package</param>
    /// <returns>Package full name or null</returns>
    public string? GetPackageFullName(string appId)
    {
        return appsByPackageAppId.TryGetValue(appId, out var application) ? application.ShellProperties.PackageFullName : null;
    }

    /// <summary>
    /// Returns installed applications where <see cref="InstalledApplication.Name"/> contains <see cref="text"/>
    /// </summary>
    /// <param name="text">Text to search for</param>
    /// <returns>Installed applications where <see cref="InstalledApplication.Name"/> contains <see cref="text"/> or empty array</returns>
    public InstalledApplication[] SearchByName(string? text)
    {
        return string.IsNullOrEmpty(text) ?
            Array.Empty<InstalledApplication>() :
            allApps
                .Where(a =>
                    a.IsApplication &&
                    (a.Name.Contains(text, StringComparison.InvariantCultureIgnoreCase) ||
                     a.ShellProperties.Keywords.Any(k => k.Contains(text, StringComparison.InvariantCultureIgnoreCase))))
                .ToArray();
    }

    /// <summary>
    /// Returns non-applications ("documents") where <see cref="InstalledApplication.Name"/> contains <see cref="text"/>
    /// </summary>
    /// <param name="text">Text to search for</param>
    /// <returns>Installed non-applications ("documents") where <see cref="InstalledApplication.Name"/> contains <see cref="text"/> or empty array</returns>
    public InstalledApplication[] SearchDocumentsByName(string? text)
    {
        return string.IsNullOrEmpty(text) ?
            Array.Empty<InstalledApplication>() :
            allDocuments
                .Where(a =>
                    !a.IsApplication &&
                    (a.Name.Contains(text, StringComparison.InvariantCultureIgnoreCase) || a.ShellProperties.Keywords.Any(k => k.Contains(text, StringComparison.InvariantCultureIgnoreCase))))
                .ToArray();
    }

    /// <summary>
    /// Returns all items in Windows AppFolder - both all applications and non-applications ("documents")
    /// </summary>
    /// <returns>All items in Windows AppFolder</returns>
    public InstalledApplication[] GetAllItems()
    {
        return allApps.Union(allDocuments).ToArray();
    }
}