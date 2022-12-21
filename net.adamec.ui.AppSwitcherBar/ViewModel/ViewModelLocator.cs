using System.ComponentModel;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using net.adamec.ui.AppSwitcherBar.Config;
using net.adamec.ui.AppSwitcherBar.Views;
using net.adamec.ui.AppSwitcherBar.Win32.Services.Audio;
using net.adamec.ui.AppSwitcherBar.Win32.Services.JumpLists;
using net.adamec.ui.AppSwitcherBar.Win32.Services.Pins;
using net.adamec.ui.AppSwitcherBar.Win32.Services.Startup;
using Wpf.Ui.Mvvm.Services;

namespace net.adamec.ui.AppSwitcherBar.ViewModel
{
    /// <summary>
    /// View Model locator used to provide the required view model
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Dummy dependency object used to get the designer mode information
        /// </summary>
        private readonly DependencyObject dummyDependencyObject = new();
        /// <summary>
        /// Flag whether running in designer mode (XAML designer)
        /// </summary>
        private bool IsDesignTime => DesignerProperties.GetIsInDesignMode(dummyDependencyObject);
        /// <summary>
        /// Dummy logger used in designer mode
        /// </summary>
        private readonly ILogger designTimeLogger;

        /// <summary>
        /// Default AppSettings used in designer mode
        /// </summary>
        private readonly AppSettings designTimeAppSettings;

        /// <summary>
        /// Background data service used in design time
        /// </summary>
        private readonly IBackgroundDataService designTimeBackgroundDataService;

        /// <summary>
        /// Language service used in design time
        /// </summary>
        private readonly ILanguageService designTimeLanguageService;

        /// <summary>
        /// Pins service used in design time
        /// </summary>
        private readonly IPinsService designTimePinsService;

        /// <summary>
        /// CTOR
        /// </summary>
        public ViewModelLocator()
        {
            designTimeAppSettings = AppSettings.DesignTimeAppSettings;

            //create the (dummy) design time logger
            using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder.SetMinimumLevel(LogLevel.Trace).AddDebug().AddConsole());
            designTimeLogger = loggerFactory.CreateLogger("DesignTime");
            designTimeLanguageService = new LanguageService(null);
            designTimePinsService = new PinsService(designTimeAppSettings, designTimeLogger, new ThemeService());
            designTimeBackgroundDataService = new BackgroundDataService(designTimeAppSettings, designTimeLogger, designTimePinsService);
        }

        /// <summary>
        /// Gets the <see cref="MainViewModel"/> for <see cref="MainWindow"/>
        /// When running in runtime, it's retrieved from DI.
        /// When running in design time a new instance of <see cref="MainViewModel"/> is used with default <see cref="AppSettings"/> and dummy logger and dummy services 
        /// </summary>
        /// <remarks>No need to mock whole model/class for design time, just use the default settings and design time logger</remarks>
        public MainViewModel MainViewModel => IsDesignTime ?
                new MainViewModel(
                    designTimeAppSettings,
                    designTimeLogger,
                    new DummyJumpListService(),
                    designTimeLanguageService,
                    designTimeBackgroundDataService,
                    designTimePinsService) :
                App.ServiceProvider.GetRequiredService<MainViewModel>();

        /// <summary>
        /// Gets the <see cref="MenuPopupViewModel"/> for <see cref="MenuPopup"/>
        /// When running in runtime, it's retrieved from DI.
        /// When running in design time a new instance of <see cref="MenuPopupViewModel"/> is used with default <see cref="AppSettings"/>  and dummy services
        /// </summary>
        /// <remarks>No need to mock whole model/class for design time, just use the default settings and design time logger</remarks>
        public MenuPopupViewModel MenuPopupViewModel => IsDesignTime ?
            new MenuPopupViewModel(
                MainViewModel,
                designTimeAppSettings,
                designTimeLogger,
                new DummyStartupService(),
                designTimeLanguageService,
                designTimeBackgroundDataService,
                designTimePinsService) :
            App.ServiceProvider.GetRequiredService<MenuPopupViewModel>();

        /// <summary>
        /// Gets the <see cref="AudioViewModel"/> for <see cref="MenuPopup"/>
        /// When running in runtime, it's retrieved from DI.
        /// When running in design time a new instance of <see cref="AudioViewModel"/> is used with default <see cref="AppSettings"/>  and dummy services
        /// </summary>
        /// <remarks>No need to mock whole model/class for design time, just use the default settings and design time logger</remarks>
        public AudioViewModel AudioViewModel => IsDesignTime ?
            new AudioViewModel(
                designTimeAppSettings,
                designTimeLogger,
               new DummyAudioService()) :
            App.ServiceProvider.GetRequiredService<AudioViewModel>();

        /// <summary>
        /// Gets the <see cref="ClockViewModel"/> for <see cref="ClockControl"/>
        /// When running in runtime, it's retrieved from DI.
        /// When running in design time a new instance of <see cref="ClockViewModel"/> is used with default <see cref="AppSettings"/> 
        /// </summary>
        /// <remarks>No need to mock whole model/class for design time, just use the default settings and design time logger</remarks>
        public ClockViewModel ClockViewModel => IsDesignTime ?
            new ClockViewModel(
                designTimeAppSettings) :
            App.ServiceProvider.GetRequiredService<ClockViewModel>();
    }
}

