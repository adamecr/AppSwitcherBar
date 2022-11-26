using System.ComponentModel;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using net.adamec.ui.AppSwitcherBar.Config;
using net.adamec.ui.AppSwitcherBar.Views;
using net.adamec.ui.AppSwitcherBar.Win32.Services.JumpLists;
using net.adamec.ui.AppSwitcherBar.Win32.Services.Startup;

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
        /// CTOR
        /// </summary>
        public ViewModelLocator()
        {
            designTimeAppSettings = AppSettings.DesignTimeAppSettings;

            //create the (dummy) design time logger
            using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder.SetMinimumLevel(LogLevel.Trace).AddDebug().AddConsole());
            designTimeLogger = loggerFactory.CreateLogger("DesignTime");
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
                    new LanguageService(null),
                    new BackgroundDataService(designTimeLogger)) :
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
                new LanguageService(null),
                new BackgroundDataService(designTimeLogger)) :
            App.ServiceProvider.GetRequiredService<MenuPopupViewModel>();
    }
}

