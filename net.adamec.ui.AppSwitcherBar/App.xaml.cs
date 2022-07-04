using System;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using net.adamec.ui.AppSwitcherBar.Config;
using net.adamec.ui.AppSwitcherBar.ViewModel;

namespace net.adamec.ui.AppSwitcherBar
{
    public partial class App : Application
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        /// <summary>
        /// <see cref="IServiceProvider"/> instance. Avoid using it where possible, use the "DI tree" instead
        /// </summary>
        public static IServiceProvider ServiceProvider { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        private IHost? host;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            host = Host
                .CreateDefaultBuilder()
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

        private void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            //Register configuration objects
            services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));

            // Register app ViewModels
            services.AddSingleton<MainViewModel>();

            // Register app Windows
            services.AddSingleton<MainWindow>();
        }
    }
}
