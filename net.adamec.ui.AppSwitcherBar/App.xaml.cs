using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using net.adamec.ui.AppSwitcherBar.Config;
using net.adamec.ui.AppSwitcherBar.ViewModel;
using net.adamec.ui.AppSwitcherBar.Win32.Services.JumpLists;
using net.adamec.ui.AppSwitcherBar.Win32.Services.Startup;
using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.Mvvm.Services;

namespace net.adamec.ui.AppSwitcherBar
{
    /// <summary>
    /// AppSwitcherBar application
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class App : Application
    {

#pragma warning disable CS8618
        // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        /// <see cref="IServiceProvider"/> instance. Avoid using it where possible, use the "DI tree" instead
        /// </summary>
        public static IServiceProvider ServiceProvider { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        /// <summary>
        /// Application host
        /// </summary>
        private IHost? host;

        /// <summary>
        /// Application logger
        /// </summary>
        private ILogger? logger;

        /// <summary>
        /// Application startup logic - configure, build and start the host
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
          
            host = Host
                .CreateDefaultBuilder(e.Args)
                .ConfigureHostConfiguration(config =>
                {
                    //add appsettings.json to host configuration to be able to get the Language when building the app configuration
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false); 
                    
                })
                .ConfigureAppConfiguration(config =>
                {
                    config.AddJsonFile(UserSettings.UserSettingsFile, optional: true);
                })
                .ConfigureAppConfiguration((ctx, config) =>
                {
                    //check Language|AppSettings:Language|current culture and try to read language from language.{id}.json
                    //the language can be specified like "en", "en-US", or with additional extensions "lang-region-extension-...".
                    //it will try to load the config from "all parts" from less to more specific - for example for "en-US", it will try language.en.json and language.en-us.json
                    var language =
                        ctx.Configuration[$"Language"] ?? //as the host configuration have the json as most specific, don't use prefix in command line to be able to override the json appsettings
                        ctx.Configuration[$"{nameof(AppSettings)}:Language"] ??
                        AppSettings.DefaultLanguage;

                    var languageParts = language.Split('-');
                    for (var i = 0; i < languageParts.Length; i++)
                    {
                        var languageId = string.Join('-', languageParts[..(i + 1)]);
                        config.AddJsonFile($"language.{languageId}.json", optional: true);
                    }
                })
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(context.Configuration, services);
                })
                // ReSharper disable once UnusedParameter.Local
                .ConfigureLogging(logging =>
                {
                    // Add other loggers...
                })
                .Build();

            ServiceProvider = host.Services;

            await host.StartAsync();

            //Log unhandled exceptions, so the EventLog can be checked when the app crash
            logger = host.Services.GetService<ILogger<App>>();
            AppDomain.CurrentDomain.UnhandledException += (_, exceptionArgs) =>
            {
                if (exceptionArgs.ExceptionObject is Exception exception) LogUnhandledException(exception);
            };
            Dispatcher.UnhandledException +=(_, exceptionArgs) => LogUnhandledException(exceptionArgs.Exception);
            Current.DispatcherUnhandledException += (_, exceptionArgs) => LogUnhandledException(exceptionArgs.Exception);
            TaskScheduler.UnobservedTaskException += (_, exceptionArgs) => LogUnhandledException(exceptionArgs.Exception);

            var mainWindow = host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
            
        }

        /// <summary>
        /// Logs the unhandled exception before the app get's closed.
        /// With default settings, the log item should be available in EventLog
        /// </summary>
        /// <param name="exception">Exception to log</param>
        private void LogUnhandledException(Exception exception)
        {
            logger?.LogCritical(exception, "Unhandled exception: {Message}", exception.Message);
        }


        /// <summary>
        /// Application exit logic - stop the host
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override async void OnExit(ExitEventArgs e)
        {
            if (host != null)
            {
                using (host)
                {
                    await host.StopAsync(TimeSpan.FromSeconds(5));
                }
            }
            base.OnExit(e);
        }

        /// <summary>
        /// Configure the service collection
        /// </summary>
        /// <param name="configuration">Application configuration</param>
        /// <param name="services">Service collection</param>
        private void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            //Register configuration objects
            services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));
            services.Configure<Language>(configuration.GetSection(nameof(Language)));

            //Register services
            services.AddSingleton<IThemeService, ThemeService>();
            services.AddSingleton<ILanguageService, LanguageService>();
            services.AddSingleton<IBackgroundDataService, BackgroundDataService>();

            if (bool.TryParse(configuration[$"{nameof(AppSettings)}:{nameof(AppSettings.FeatureFlags)}:JumpList"], out var hasJumpList) && hasJumpList)
            {
                var jumpListSvcVersion =
                    int.TryParse(configuration[$"{nameof(AppSettings)}:{nameof(AppSettings.FeatureFlags)}:JumpListSvcVersion"], out var parsedVersion)
                        ? parsedVersion
                        : 2;

                switch (jumpListSvcVersion)
                {
                    case 1:
                        services.AddSingleton<IJumpListService, JumpListService>(); 
                        break;
                    case 2:
                        services.AddSingleton<IJumpListService, JumpListService2>();
                        break;
                    default:
                        services.AddSingleton<IJumpListService, JumpListService2>();
                        break;
                }
            }
            else
            {
                services.AddSingleton<IJumpListService, DummyJumpListService>();
            }

            if (bool.TryParse(configuration[$"{nameof(AppSettings)}:{nameof(AppSettings.FeatureFlags)}:RunOnWindowsStartup"], out var hasWinStartup) && hasWinStartup)
            {
                services.AddSingleton<IStartupService, StartupService>();
            }
            else
            {
                services.AddSingleton<IStartupService, DummyStartupService>();
            }

            // Register app ViewModels
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MenuPopupViewModel>();

            // Register app Windows
            services.AddSingleton<MainWindow>();
        }
    }
}
