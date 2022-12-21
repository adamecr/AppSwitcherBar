using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using net.adamec.ui.AppSwitcherBar.Config;
using net.adamec.ui.AppSwitcherBar.Dto;
using net.adamec.ui.AppSwitcherBar.Win32.NativeInterfaces.Extensions;
using net.adamec.ui.AppSwitcherBar.Win32.Services;
using net.adamec.ui.AppSwitcherBar.Win32.Services.Pins;
using net.adamec.ui.AppSwitcherBar.Win32.Services.Shell;
using net.adamec.ui.AppSwitcherBar.Win32.Services.Shell.Properties;

namespace net.adamec.ui.AppSwitcherBar.ViewModel
{
    /// <summary>
    /// Provides the data being retrieved on background - <see cref="InstalledApplications"/>
    /// </summary>
    public class BackgroundDataService : IBackgroundDataService
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
        // 4002 LogBackgroundDataTelemetry (Info)
        //----------------------------------------------

        /// <summary>
        /// Logger message format for LogBackgroundDataTelemetry
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private static readonly string __LogBackgroundDataTelemetryFormatString =
            "Finished retrieving {feature} background data at {timestampFinished}. Success:{isSuccess}. Duration total: {durationTotal}, feature duration: {durationFeature}, Result:{resultMsg}";

        /// <summary>
        /// Logger message definition for LogBackgroundDataInitTelemetry (Info)
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private static readonly Action<ILogger, string, DateTime, bool, int, int, string, Exception?> __LogBackgroundDataTelemetryInfoDefinition =
            LoggerMessage.Define<string, DateTime, bool, int, int, string>(
                LogLevel.Information,
                new EventId(4002, nameof(LogBackgroundDataTelemetry)),
                __LogBackgroundDataTelemetryFormatString,
                LogOptions);

        /// <summary>
        /// Logs record (Information or error) after finishing the data retrieval
        /// </summary>
        /// <param name="level">Required log record level</param>
        /// <param name="feature">Name of the feature (type of the data)</param>
        /// <param name="timestampFinished">Timestamp when the background data retrieval finished</param>
        /// <param name="isSuccess">Flag whether the background data retrieval finished successfully</param>
        /// <param name="resultMsg">Result message - contains OK or error message</param>
        /// <param name="durationTotal">Total duration of background data retrieval (in ms)</param>
        /// <param name="durationFeature">Duration of single feature data retrieval (in ms)</param>
        private void LogBackgroundDataTelemetry(LogLevel level, string feature, DateTime timestampFinished, bool isSuccess, string resultMsg, int durationTotal, int durationFeature)
        {
            if (!logger.IsEnabled(level)) return;

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (level)
            {
                case LogLevel.Information:
                    __LogBackgroundDataTelemetryInfoDefinition(logger, feature, timestampFinished, isSuccess, durationTotal, durationFeature, resultMsg, null);
                    break;
                case LogLevel.Error:
                    __LogBackgroundDataTelemetryDefinition(logger, feature, timestampFinished, isSuccess, durationTotal, durationFeature, resultMsg, null);
                    break;
            }
        }

        //----------------------------------------------
        // 4003 LogBackgroundRunnerStart
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogBackgroundRunnerStart
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private static readonly Action<ILogger, DateTime, Exception?> __LogBackgroundRunnerStartDefinition =
            LoggerMessage.Define<DateTime>(
                LogLevel.Debug,
                new EventId(4003, nameof(LogBackgroundRunnerStart)),
                "Background worker started work on {timestamp}",
                LogOptions);

        /// <summary>
        /// Logs record (Debug) about starting the background work
        /// </summary>
        private void LogBackgroundRunnerStart()
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                __LogBackgroundRunnerStartDefinition(logger, DateTime.Now, null);

            }
        }

        //----------------------------------------------
        // 4004 LogBackgroundRunnerEndOk (Debug - OK)
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogBackgroundRunnerStart
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private static readonly Action<ILogger, DateTime, string, Exception?> __LogBackgroundRunnerEndOkDefinition =
            LoggerMessage.Define<DateTime, string>(
                LogLevel.Debug,
                new EventId(4004, nameof(LogBackgroundRunnerEndOk)),
                "Background worker finished work on {timestamp}, data: {data}",
                LogOptions);

        /// <summary>
        /// Logs record (Debug) about starting the background work
        /// </summary>
        private void LogBackgroundRunnerEndOk(BackgroundData? data)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                __LogBackgroundRunnerEndOkDefinition(logger, DateTime.Now, data?.ToString()??"[NULL]",null);

            }
        }

        //----------------------------------------------
        // 4005 LogBackgroundRunnerEndCancel (Warn - Cancel)
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogBackgroundRunnerStart
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private static readonly Action<ILogger, DateTime, Exception?> __LogBackgroundRunnerEndCancelDefinition =
            LoggerMessage.Define<DateTime>(
                LogLevel.Warning,
                new EventId(4005, nameof(LogBackgroundRunnerEndCancel)),
                "Background worker work cancelled on {timestamp}",
                LogOptions);

        /// <summary>
        /// Logs record (Debug) about starting the background work
        /// </summary>
        private void LogBackgroundRunnerEndCancel()
        {
            if (logger.IsEnabled(LogLevel.Warning))
            {
                __LogBackgroundRunnerEndCancelDefinition(logger, DateTime.Now, null);

            }
        }


        //----------------------------------------------
        // 4901 LogBackgroundDataTelemetry (Error)
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogBackgroundDataTelemetry (Error)
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private static readonly Action<ILogger, string, DateTime, bool, int, int, string, Exception?> __LogBackgroundDataTelemetryDefinition =
            LoggerMessage.Define<string, DateTime, bool, int, int, string>(
                LogLevel.Error,
                new EventId(4901, nameof(LogBackgroundDataTelemetry)),
                __LogBackgroundDataTelemetryFormatString,
                LogOptions);

        //Shares LogBackgroundDataTelemetry method

        //----------------------------------------------
        // 4902 LogBackgroundRunnerEndErr (Error - Exception)
        //----------------------------------------------

        /// <summary>
        /// Logger message definition for LogBackgroundRunnerStart
        /// </summary>
        // ReSharper disable once InconsistentNaming
        private static readonly Action<ILogger, DateTime, Exception?> __LogBackgroundRunnerEndErrDefinition =
            LoggerMessage.Define<DateTime>(
                LogLevel.Error,
                new EventId(4902, nameof(LogBackgroundRunnerEndErr)),
                "Background worker ends with error on {timestamp}",
                LogOptions);

        /// <summary>
        /// Logs record (Debug) about starting the background work
        /// </summary>
        private void LogBackgroundRunnerEndErr(Exception exception)
        {
            if (logger.IsEnabled(LogLevel.Error))
            {
                __LogBackgroundRunnerEndErrDefinition(logger, DateTime.Now, exception);

            }
        }
        #endregion

        /// <summary>
        /// Application settings
        /// </summary>
        protected readonly IAppSettings Settings;

        /// <summary>
        /// Pins  service to be used
        /// </summary>
        private IPinsService PinsService { get; }

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
        /// Information about the applications pinned in the start menu
        /// </summary>
        public PinnedAppInfo[] StartPinnedApplications { get; private set; } = Array.Empty<PinnedAppInfo>();

        /// <summary>
        /// Internal CTOR
        /// Directly used by <see cref="ViewModelLocator"/> when creating a design time instance.
        /// Internally called by public "DI bound" CTOR
        /// </summary>
        /// <param name="settings">Application setting</param>
        /// <param name="logger">Logger to be used</param>
        /// <param name="pinsService">Pins service to be used</param>
        internal BackgroundDataService(IAppSettings settings, ILogger logger, IPinsService pinsService)
        {
            Settings = settings;
            PinsService = pinsService;
            this.logger = logger;
            backgroundInitWorker = new BackgroundWorker();
            backgroundInitWorker.DoWork += (_, eventArgs) => {
                LogBackgroundRunnerStart();
                eventArgs.Result = RetrieveBackgroundData(); 
                if(backgroundInitWorker.CancellationPending) eventArgs.Cancel = true;
            };
            backgroundInitWorker.RunWorkerCompleted += (_, eventArgs) => {

                if (eventArgs.Cancelled)
                {
                    // The user canceled the operation.
                    LogBackgroundRunnerEndCancel();
                    OnBackgroundDataRetrieved(null);
                    
                }
                else if (eventArgs.Error != null)
                {
                    // There was an error during the operation.
                    LogBackgroundRunnerEndErr(eventArgs.Error);
                    OnBackgroundDataRetrieved(null); 
                }
                else
                {
                    // The operation completed normally.
                    var data = (BackgroundData?)eventArgs.Result;
                    LogBackgroundRunnerEndOk(data);
                    OnBackgroundDataRetrieved(data);
                }
            };
        }


        /// <summary>
        /// CTOR used by DI
        /// </summary>
        /// <param name="options">Application settings configuration</param>
        /// <param name="logger">Logger to be used</param>
        /// <param name="pinsService">Pins service to be used</param>
        // ReSharper disable once UnusedMember.Global
        public BackgroundDataService(IOptions<AppSettings> options, ILogger<BackgroundDataService> logger, IPinsService pinsService) :
            this(options.Value, logger, pinsService)
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
            bool isSuccess = true;
            var resultMsg = "OK";
            BackgroundData? data = null;
            var timestampStart = DateTime.Now;

            //retrieve installed apps -> AUMIs, icons
            var dataInstalledApps = new List<InstalledApplication>();

            try
            {
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

                        var app = new InstalledApplication(appName, appUserModelId, lnkTarget, iconSource, shellProperties);

                        if (Settings.FeatureFlag(AppSettings.FF_EnableRunInfoFromWindowsPrefetch, false))
                        {
                            //Initialize run stats from Windows Prefetch
                            if (app.Executable != null)
                            {
                                app.RunStats.UpdateRunInfo(Prefetch.GetPrefetchInfoNoHash(app.Executable));
                            }
                            else if (app.ShellProperties.IsStoreApp && !string.IsNullOrEmpty(app.ShellProperties.PackageInstallPath) && !string.IsNullOrEmpty(app.ShellProperties.ParsingName))
                            {
                                app.RunStats.UpdateRunInfo(Prefetch.GetPrefetchInfoNoHash(app.ShellProperties.PackageInstallPath, app.ShellProperties.ParsingName));
                            }
                        }

                        dataInstalledApps.Add(app);
                        LogInstalledAppInfo(appName, appUserModelId ?? "[Unknown]", iconSource != null, lnkTarget ?? "[N/A]");
                    });
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                resultMsg = $"{ex.GetType().Name}: {ex.Message}";
            }

            var timestampEndInstalledApps = DateTime.Now;
            var durationInstalledApps = (timestampEndInstalledApps - timestampStart).TotalMilliseconds;

            var timestampEndTotal = DateTime.Now;
            var durationTotal = (timestampEndTotal - timestampStart).TotalMilliseconds;

            LogBackgroundDataTelemetry(
                isSuccess ? LogLevel.Information : LogLevel.Error,
                "Installed applications",
                DateTime.Now, isSuccess, resultMsg,
                (int)durationTotal, (int)durationInstalledApps);


            var startPins = Array.Empty<PinnedAppInfo>();
            if (isSuccess)
            {
                //got installed apps, let's continue
                var timestampStartStartPins = DateTime.Now;

                try
                {
                    startPins=PinsService.RefreshStartPins();

                    resultMsg = "OK";
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    isSuccess = false;
                    resultMsg = $"{ex.GetType().Name}: {ex.Message}";
                }

                var timestampEndStartPins = DateTime.Now;
                var durationStartPins = (timestampEndStartPins - timestampStartStartPins).TotalMilliseconds;

                timestampEndTotal = DateTime.Now;
                durationTotal = (timestampEndTotal - timestampStart).TotalMilliseconds;

                LogBackgroundDataTelemetry(
                    isSuccess ? LogLevel.Information : LogLevel.Error,
                    "Start pinned applications",
                    DateTime.Now, isSuccess, resultMsg,
                    (int)durationTotal, (int)durationStartPins);
            }

            if (isSuccess)
            {
                data = new BackgroundData(dataInstalledApps.ToArray(),startPins.ToArray());

            }
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
                        //clone the object, co it can be used in other (UI) thread!!!
                        installedApplication.IconSource = installedApplication.IconSource.Clone();
                    }
                    InstalledApplications.Add(installedApplication);
                }

                var startPins=new List<PinnedAppInfo>();
                foreach(var pin in data.StartPinnedApplications)
                {
                    if(pin.BitmapSource !=null)
                    {
                        //clone the object, co it can be used in other (UI) thread!!!
                        pin.BitmapSource=pin.BitmapSource.Clone();
                    }
                    startPins.Add(pin);
                }
                StartPinnedApplications=startPins.ToArray();
                
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
