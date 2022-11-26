using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using net.adamec.ui.AppSwitcherBar.Config;
using net.adamec.ui.AppSwitcherBar.ViewModel;

namespace net.adamec.ui.AppSwitcherBar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow:INotifyPropertyChanged
    {
        /// <summary>
        /// Background data retrieval service
        /// </summary>
        private IBackgroundDataService BackgroundDataService { get; }


        /// <summary>
        /// Flag whether the background data are being retrieved/refreshed
        /// </summary>
        private bool isBackgroundRefreshing;

        /// <summary>
        /// Flag whether the background data are being retrieved/refreshed
        /// </summary>
        public bool IsBackgroundRefreshing
        {
            get => isBackgroundRefreshing;
            set
            {
                if (isBackgroundRefreshing != value)
                {
                    isBackgroundRefreshing = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="options">Application settings</param>
        /// <param name="logger">Logger to be used</param>
        /// <param name="backgroundDataService">Background data retrieval service</param>
        public MainWindow(IOptions<AppSettings> options, ILogger<MainWindow> logger, IBackgroundDataService backgroundDataService) : base(options, logger)
        {
            InitializeComponent();
            BackgroundDataService = backgroundDataService;
            IsBackgroundRefreshing = !BackgroundDataService.BackgroundDataRetrieved;
            backgroundDataService.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName == nameof(BackgroundDataService.BackgroundDataRetrieved))
                {
                    IsBackgroundRefreshing = !backgroundDataService.BackgroundDataRetrieved;
                }
            };
            if (DataContext is MainViewModel viewModel && !IsDesignTime)
            {
                //initialize the "active" logic of view model - retrieving the information about windows
                Loaded += (_, _) => viewModel.Init(Hwnd);
            }
            
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
