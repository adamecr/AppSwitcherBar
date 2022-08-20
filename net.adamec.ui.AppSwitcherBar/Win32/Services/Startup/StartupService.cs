using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using net.adamec.ui.AppSwitcherBar.Config;
using net.adamec.ui.AppSwitcherBar.ViewModel;
using net.adamec.ui.AppSwitcherBar.Win32.NativeConstants;
using net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces;

namespace net.adamec.ui.AppSwitcherBar.Win32.Services.Startup
{
    /// <summary>
    /// Encapsulates the Run on Windows Startup functionality
    /// </summary>
    public class StartupService : IStartupService
    {
        #region Logging
        // ReSharper disable InconsistentNaming
        /// <summary>
        /// Logger used
        /// </summary>
        private readonly ILogger logger;
        // 2xxx - Startup Service (29xx Errors/Exceptions)
        /// <summary>
        /// Log definition options
        /// </summary>
        private static readonly LogDefineOptions LogOptions = new() { SkipEnabledCheck = true };


        //----------------------------------------------
        // 2001 Startup Link created
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogStartupLinkCreated
        /// </summary>
        private static readonly Action<ILogger, string, Exception?> __LogStartupLinkCreatedDefinition =

            LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(2001, nameof(LogStartupLinkCreated)),
                "Created link in Windows startup folder {linkPath}",
                LogOptions);

        /// <summary>
        /// Logs record (Information) when a Windows startup link is created
        /// </summary>
        /// <param name="linkPath">Full path to the link file</param>
        private void LogStartupLinkCreated(string linkPath)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                __LogStartupLinkCreatedDefinition(logger, linkPath, null);
            }
        }

        //----------------------------------------------
        // 2002 Startup Link removed
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogStartupLinkRemoved
        /// </summary>
        private static readonly Action<ILogger, string, Exception?> __LogStartupLinkRemovedDefinition =

            LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(2002, nameof(LogStartupLinkRemoved)),
                "Removed link from Windows startup folder {linkPath}",
                LogOptions);

        /// <summary>
        /// Logs record (Information) when a Windows startup link is removed
        /// </summary>
        /// <param name="linkPath">Full path to the link file</param>
        private void LogStartupLinkRemoved(string linkPath)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                __LogStartupLinkRemovedDefinition(logger, linkPath, null);
            }
        }

        //----------------------------------------------
        // 2901 Startup link action Exception
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogStartupLinkActionException
        /// </summary>
        private static readonly Action<ILogger, string, Exception?> __LogStartupLinkActionExceptionDefinition =
            LoggerMessage.Define<string>(
                LogLevel.Warning,
                new EventId(2901, nameof(LogStartupLinkActionException)),
                "Error while processing the Startup link action {action}",
                LogOptions);

        /// <summary>
        /// Logs record (Warning) of exception thrown when processing the Startup link action
        /// </summary>
        /// <param name="action">Name of the Startup link action</param>
        /// <param name="ex">Exception thrown</param>
        private void LogStartupLinkActionException(string action, Exception ex)
        {
            if (logger.IsEnabled(LogLevel.Warning))
            {
                __LogStartupLinkActionExceptionDefinition(logger, action, ex);
            }
        }

        //----------------------------------------------
        // 2902 Startup link action not finished
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogStartupLinkActionNotFinished
        /// </summary>
        private static readonly Action<ILogger, string, string, Exception?> __LogStartupLinkActionNotFinishedDefinition =
            LoggerMessage.Define<string, string>(
                LogLevel.Warning,
                new EventId(2902, nameof(LogStartupLinkActionNotFinished)),
                "Processing the Startup link action {action} not finished: {reason}",
                LogOptions);

        /// <summary>
        /// Logs record (Warning) of exception thrown when processing the Startup link action
        /// </summary>
        /// <param name="action">Name of the Startup link action</param>
        /// <param name="reason">Description why the action was not finished</param>
        private void LogStartupLinkActionNotFinished(string action, string reason)
        {
            if (logger.IsEnabled(LogLevel.Warning))
            {
                __LogStartupLinkActionNotFinishedDefinition(logger, action, reason, null);
            }
        }

        // ReSharper restore InconsistentNaming
        #endregion

        /// <summary>
        /// Application settings
        /// </summary>
        private readonly IAppSettings settings;


        /// <summary>
        /// Internal CTOR
        /// Directly used by <see cref="ViewModelLocator"/> when creating a design time instance.
        /// Internally called by public "DI bound" CTOR
        /// </summary>
        /// <param name="settings">Application setting</param>
        /// <param name="logger">Logger to be used</param>
        internal StartupService(IAppSettings settings, ILogger logger)
        {
            this.logger = logger;
            this.settings = settings;
        }

        /// <summary>
        /// CTOR used by DI
        /// </summary>
        /// <param name="options">Application settings configuration</param>
        /// <param name="logger">Logger used</param>
        // ReSharper disable once UnusedMember.Global
        public StartupService(IOptions<AppSettings> options, ILogger<StartupService> logger) : this(options.Value, logger)
        {
            //used from DI - DI populates the parameters and the internal CTOR is called then
        }


        /// <summary>
        /// Creates the link to AppSwitcherBar at Windows Startup folder
        /// </summary>
        /// <param name="description">Application description</param>
        /// <returns>True when the link was successfully created otherwise false</returns>
        public bool CreateAppStartupLink(string? description)
        {
            try
            {
                if (!settings.AllowRunOnWindowsStartup)
                {
                    LogStartupLinkActionNotFinished(nameof(CreateAppStartupLink), "AllowRunOnWindowsStartup is not enabled in settings");
                    return false;
                }

                var linkClass = Type.GetTypeFromCLSID(new Guid(Win32Consts.CLSID_ShellLink), false);
                if (linkClass == null)
                {
                    LogStartupLinkActionNotFinished(nameof(CreateAppStartupLink), "Can't create ShellLink class type");
                    return false;
                }

                if (Activator.CreateInstance(linkClass) is not IShellLinkW link)
                {
                    LogStartupLinkActionNotFinished(nameof(CreateAppStartupLink), "Can't create IShellLinkW instance");
                    return false;
                }

                var executablePath = GetAppExecutable(out var executableDir, out var executableName);
                if (string.IsNullOrEmpty(executablePath))
                {
                    LogStartupLinkActionNotFinished(nameof(CreateAppStartupLink), "Can't get the executable path");
                    return false;
                }


                link.SetPath(executablePath);
                if (executableDir != null) link.SetWorkingDirectory(executableDir);

                if (description != null) link.SetDescription(description);

                var icoFile = executableDir != null ? Path.Combine(executableDir, $"{executableName}.ico") : null;
                if (icoFile != null && File.Exists(icoFile)) link.SetIconLocation(icoFile, 0);

                // ReSharper disable once SuspiciousTypeConversion.Global
                if (link is not IPersistFile file)
                {
                    LogStartupLinkActionNotFinished(nameof(CreateAppStartupLink), "Can't get the link IPersistFile interface");
                    return false;
                }

                var linkPath = GetAppStartupLinkPath();
                if (linkPath == null)
                {
                    LogStartupLinkActionNotFinished(nameof(CreateAppStartupLink), "Can't get the link path");
                    return false;
                }

                file.Save(linkPath, false);
                LogStartupLinkCreated(linkPath);
            }
            catch (Exception ex)
            {
                LogStartupLinkActionException(nameof(CreateAppStartupLink), ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Retrieves the information whether AppSwitcherBar has link Windows Startup folder
        /// </summary>
        /// <returns>True when AppSwitcherBar has link Windows Startup folder otherwise false</returns>
        public bool HasAppStartupLink()
        {
            var linkPath = GetAppStartupLinkPath();
            return linkPath != null && File.Exists(linkPath);
        }

        /// <summary>
        /// Removes the link to AppSwitcherBar from Windows Startup folder
        /// </summary>
        /// <returns>True when the link was successfully removed otherwise false</returns>
        public bool RemoveAppStartupLink()
        {
            try
            {
                if (!settings.AllowRunOnWindowsStartup)
                {
                    LogStartupLinkActionNotFinished(nameof(RemoveAppStartupLink), "AllowRunOnWindowsStartup is not enabled in settings");
                    return false;
                }

                var linkPath = GetAppStartupLinkPath();
                if (linkPath == null)
                {
                    LogStartupLinkActionNotFinished(nameof(RemoveAppStartupLink), "Can't get the link path");
                    return false;
                }
                if (!File.Exists(linkPath))
                {
                    LogStartupLinkActionNotFinished(nameof(RemoveAppStartupLink), $"Link doesn't exist {linkPath}");
                    return false;
                }

                File.Delete(linkPath);
                LogStartupLinkRemoved(linkPath);
                return true;
            }
            catch (Exception ex)
            {
                LogStartupLinkActionException(nameof(RemoveAppStartupLink), ex);
                return false;
            }
        }

        /// <summary>
        /// Builds the path for the AppSwitcherBar link in Windows startup folder
        /// </summary>
        /// <returns>Path for the AppSwitcherBar link in Windows startup folder or null when the path can't be built</returns>
        private static string? GetAppStartupLinkPath()
        {
            GetAppExecutable(out _, out var executableNameWithoutExt);
            if (executableNameWithoutExt == null) return null;

            var linkPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), $"{executableNameWithoutExt}.lnk");
            return linkPath;

        }

        /// <summary>
        /// Gets AppSwitcherBar executable information
        /// </summary>
        /// <param name="executableDir">Directory of the AppSwitcherBar executable or null when the executable can't be retrieved</param>
        /// <param name="executableNameWithoutExt">Name of the AppSwitcherBar executable without the extension or null when the executable can't be retrieved</param>
        /// <returns>Full path to AppSwitcherBar executable or null when the executable can't be retrieved</returns>
        private static string? GetAppExecutable(out string? executableDir, out string? executableNameWithoutExt)
        {
            var executablePath = Environment.ProcessPath;
            executableDir = executablePath != null ? Path.GetDirectoryName(executablePath) : null;
            executableNameWithoutExt = executablePath != null ? Path.GetFileNameWithoutExtension(executablePath) : null;

            return executablePath;
        }
    }
}
