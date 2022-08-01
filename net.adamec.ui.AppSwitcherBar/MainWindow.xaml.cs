using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using net.adamec.ui.AppSwitcherBar.Config;
using net.adamec.ui.AppSwitcherBar.ViewModel;

namespace net.adamec.ui.AppSwitcherBar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        public MainWindow(IOptions<AppSettings> options, ILogger<MainWindow> logger) : base(options, logger)
        {
            InitializeComponent();

            if (DataContext is MainViewModel viewModel && !IsDesignTime)
            {
                //initialize the "active" logic of view model - retrieving the information about windows
                Loaded += (_, _) => viewModel.Init(Hwnd);
            }
        }
    }
}
