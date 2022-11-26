using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using net.adamec.ui.AppSwitcherBar.Dto;
using net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces.Extensions;
using net.adamec.ui.AppSwitcherBar.Win32.Services.Shell;
using net.adamec.ui.AppSwitcherBar.Win32.Services.Shell.Properties;

namespace net.adamec.ui.AppSwitcherBar.ViewModel
{
    /// <summary>
    /// Provides the data being retrieved on background - <see cref="InstalledApplications"/>
    /// </summary>
    public class BackgroundDataService: IBackgroundDataService
    {
        #region Logging
        /// <summary>
        /// Logger used
        /// </summary>
        private readonly ILogger logger;


        //EventIds:
        // 1xx - Windows API "interactions"
        // 2xx - Application Window Button interactions
        // 3xx - Telemetry
        // 9xx - Errors/Exceptions
        // ----
        // 1xxx - JumpList Service (19xx Errors/Exceptions)
        // 2xxx - Startup Service (29xx Errors/Exceptions)
        // 3xxx - AppBarWindow (39xx Errors/Exceptions)
        // 4xxx - BackgroundData Service (49xx Errors/Exceptions)

        /// <summary>
        /// Log definition options
        /// </summary>
        private static readonly LogDefineOptions LogOptions = new() { SkipEnabledCheck = true };
        //----------------------------------------------
        // 4001 LogInstalledAppInfo
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogInstalledAppInfo
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private static readonly Action<ILogger, string, string, bool, string, Exception?> __LogInstalledAppInfoDefinition =
            LoggerMessage.Define<string, string, bool, string>(
                LogLevel.Debug,
                new EventId(4001, nameof(LogInstalledAppInfo)),
                "Installed application {appName}, AUMI:{appUserModelId}, Icon:{hasIcon}, Link:{link}",
                LogOptions);

        /// <summary>
        /// Logs record (Debug) about retrieved information about the installed application
        /// </summary>
        /// <param name="appName">Name of the application</param>
        /// <param name="appUserModelId">AUMI of the application</param>
        /// <param name="hasIcon">Flag whether there is an icon source available</param>
        /// <param name="link">Link to application executable</param>

        private void LogInstalledAppInfo(string appName, string appUserModelId, bool hasIcon, string link)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                __LogInstalledAppInfoDefinition(logger, appName, appUserModelId, hasIcon, link, null);

            }
        }

        //----------------------------------------------
        // 301 LogBackgroundDataTelemetry (Info)
        //----------------------------------------------

        /// <summary>
        /// Logger message format for LogBackgroundDataTelemetry
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private static readonly string __LogBackgroundDataTelemetryFormatString =
            "Finished retrieving background data at {timestampFinished}. Success:{isSuccess}. Duration {duration} (Installed Apps:{durationInstalledApps}), Result:{resultMsg}";

        /// <summary>
        /// Logger message definition for LogBackgroundDataInitTelemetry (Info)
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private static readonly Action<ILogger, DateTime, bool, int, int, string, Exception?> __LogBackgroundDataTelemetryInfoDefinition =
            LoggerMessage.Define<DateTime, bool, int, int, string>(
                LogLevel.Information,
                new EventId(301, nameof(LogBackgroundDataTelemetry)),
                __LogBackgroundDataTelemetryFormatString,
                LogOptions);

        /// <summary>
        /// Logs record (Information or error) after finishing the data retrieval
        /// </summary>
        /// <param name="level">Required log record level</param>
        /// <param name="timestampFinished">Timestamp when the background data retrieval finished</param>
        /// <param name="isSuccess">Flag whether the background data retrieval finished successfully</param>
        /// <param name="resultMsg">Result message - contains OK or error message</param>
        /// <param name="duration">Total duration of background data retrieval (in ms)</param>
        /// <param name="durationInstalledApps">Duration of installed applications data retrieval (in ms)</param>
        private void LogBackgroundDataTelemetry(LogLevel level, DateTime timestampFinished, bool isSuccess, string resultMsg, int duration, int durationInstalledApps)
        {
            if (!logger.IsEnabled(level)) return;

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (level)
            {
                case LogLevel.Information:
                    __LogBackgroundDataTelemetryInfoDefinition(logger, timestampFinished, isSuccess, duration, durationInstalledApps, resultMsg, null);
                    break;
                case LogLevel.Error:
                    __LogBackgroundDataTelemetryDefinition(logger, timestampFinished, isSuccess, duration, durationInstalledApps, resultMsg, null);
                    break;
            }
        }

        //----------------------------------------------
        // 4901 LogBackgroundDataInitTelemetry (Error)
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogBackgroundDataTelemetry (Error)
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private static readonly Action<ILogger, DateTime, bool, int, int, string, Exception?> __LogBackgroundDataTelemetryDefinition =
            LoggerMessage.Define<DateTime, bool, int, int, string>(
                LogLevel.Error,
                new EventId(4901, nameof(LogBackgroundDataTelemetry)),
                __LogBackgroundDataTelemetryFormatString,
                LogOptions);

        //Shares LogBackgroundDataTelemetry method
        #endregion
        
        /// <summary>
        /// <see cref="BackgroundWorker"/> used to retrieve helper data on background
        /// </summary>
        private readonly BackgroundWorker backgroundInitWorker;

        /// <summary>
        /// Flag whether the background data have been retrieved 
        /// </summary>
        private bool backgroundDataRetrieved;

        /// <summary>
        /// Flag whether the background data have been retrieved 
        /// </summary>
        public bool BackgroundDataRetrieved
        {
            get => backgroundDataRetrieved;
            set
            {
                if (backgroundDataRetrieved != value)
                {
                    backgroundDataRetrieved = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Information about the applications installed in system
        /// </summary>
        public InstalledApplications InstalledApplications { get; } = new();

        /// <summary>
        /// Internal CTOR
        /// Directly used by <see cref="ViewModelLocator"/> when creating a design time instance.
        /// Internally called by public "DI bound" CTOR
        /// </summary>
         /// <param name="logger">Logger to be used</param>
        internal BackgroundDataService(ILogger logger)
        {
            this.logger = logger;
            backgroundInitWorker = new BackgroundWorker();
            backgroundInitWorker.DoWork += (_, eventArgs) => { eventArgs.Result = RetrieveBackgroundData(); };
            backgroundInitWorker.RunWorkerCompleted += (_, eventArgs) => { OnBackgroundDataRetrieved((BackgroundData)eventArgs.Result!); };
        }


        /// <summary>
        /// CTOR used by DI
        /// </summary>
        /// <param name="logger">Logger to be used</param>
        // ReSharper disable once UnusedMember.Global
        public BackgroundDataService(ILogger<BackgroundDataService> logger) : this( (ILogger)logger)
        {
            //used from DI - DI populates the parameters and the internal CTOR is called then
        }

        /// <summary>
        /// Refresh (reload) the background data
        /// </summary>
        public void Refresh()
        {
            //Refresh also init data
            if (!backgroundInitWorker.IsBusy)
            {
                BackgroundDataRetrieved = false;
                backgroundInitWorker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Retrieve helper data on background
        /// </summary>
        internal BackgroundData? RetrieveBackgroundData()
        {
            var timestampStart = DateTime.Now;
            var timestampEndInstalledApps = DateTime.MinValue;

            bool isSuccess;
            string resultMsg;
            BackgroundData? data = null;

            try
            {
                //retrieve installed apps -> AUMIs, icons
                var dataInstalledApps = new List<InstalledApplication>();
                var appsFolder = Shell.GetAppsFolder();
                if (appsFolder != null)
                {
                    Shell.EnumShellItems(appsFolder, item =>
                    {
                        var appName = item.GetDisplayName();
                        var propertyStore = item.GetPropertyStore();
                        var shellProperties = propertyStore?.GetProperties() ?? new ShellPropertiesSubset(); //empty object

                        //if (!shellProperties.IsApplication) return; //not application (not runnable)

                        var appUserModelId = shellProperties.ApplicationUserModelId;
                        var iconSource = Shell.GetShellItemBitmapSource(item, 32);
                        var lnkTarget = propertyStore?.GetPropertyValue<string>(PropertyKey.PKEY_Link_TargetParsingPath);

                        dataInstalledApps.Add(new InstalledApplication(appName, appUserModelId, lnkTarget, iconSource, shellProperties));
                        LogInstalledAppInfo(appName, appUserModelId ?? "[Unknown]", iconSource != null, lnkTarget ?? "[N/A]");
                    });
                }

                timestampEndInstalledApps = DateTime.Now;

                data = new BackgroundData(dataInstalledApps.ToArray());
                resultMsg = "OK";
                isSuccess = true;
            }
            catch (Exception ex)
            {
                isSuccess = false;
                resultMsg = $"{ex.GetType().Name}: {ex.Message}";
            }

            var timestampEndTotal = DateTime.Now;
            var durationInstalledApps = (timestampEndInstalledApps - timestampStart).TotalMilliseconds;
            var durationTotal = (timestampEndTotal - timestampStart).TotalMilliseconds;

            LogBackgroundDataTelemetry(
                isSuccess ? LogLevel.Information : LogLevel.Error,
                DateTime.Now, isSuccess, resultMsg,
                (int)durationTotal, (int)durationInstalledApps);


            return data;
        }

        /// <summary>
        /// Called when the background data have been retrieved - "copy" them to view model 
        /// </summary>
        /// <param name="data">Retrieved background data or null when not available</param>
        internal void OnBackgroundDataRetrieved(BackgroundData? data)
        {
            if (data != null)
            {
                InstalledApplications.Clear();
                foreach (var installedApplication in data.InstalledApplications)
                {
                    if (installedApplication.IconSource != null)
                    {
                        //clone the object, co it can be uses in other (UI) thread!!!
                        installedApplication.IconSource = installedApplication.IconSource.Clone();
                    }
                    InstalledApplications.Add(installedApplication);
                }
            }
            BackgroundDataRetrieved = true;
        }

        /// <summary>
        /// Occurs when a property value changes
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raise <see cref="PropertyChanged"/> event for given <paramref name="propertyName"/>
        /// </summary>
        /// <param name="propertyName">Name of the property changed</param>
        // ReSharper disable once UnusedMember.Global
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
