using System;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using net.adamec.ui.AppSwitcherBar.Config;
using net.adamec.ui.AppSwitcherBar.ViewModel;
using net.adamec.ui.AppSwitcherBar.Win32.Services.JumpLists;
using net.adamec.ui.AppSwitcherBar.Win32.Services.Startup;

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
        /// Application startup logic - configure, build and start the host
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            host = Host
                .CreateDefaultBuilder(e.Args)
                .ConfigureAppConfiguration(config =>
                {
                    config.AddJsonFile(UserSettings.UserSettingsFile, optional: true);
                })
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(context.Configuration, services);
                })
                .ConfigureLogging(logging =>
                {
                    // Add other loggers...
                })
                .Build();

            ServiceProvider = host.Services;

            await host.StartAsync();

            var mainWindow = host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();


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

            //Register services
            if (bool.TryParse(configuration["AppSettings:FeatureFlags:JumpList"], out var hasJumpList) && hasJumpList)
            {
                services.AddSingleton<IJumpListService, JumpListService>();
            }
            else
            {
                services.AddSingleton<IJumpListService, DummyJumpListService>();
            }

            if (bool.TryParse(configuration["AppSettings:FeatureFlags:RunOnWindowsStartup"], out var hasWinStartup) && hasWinStartup)
            {
                services.AddSingleton<IStartupService, StartupService>();
            }
            else
            {
                services.AddSingleton<IStartupService, DummyStartupService>();
            }

            // Register app ViewModels
            services.AddSingleton<MainViewModel>();

            // Register app Windows
            services.AddSingleton<MainWindow>();
        }
    }
}
