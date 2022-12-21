using net.adamec.ui.AppSwitcherBar.AppBar;
using System.Windows;
using System.Windows.Controls;

namespace net.adamec.ui.AppSwitcherBar.Views
{
    /// <summary>
    /// Interaction logic for AudioControl.xaml
    /// </summary>
    public partial class AudioControl : UserControl
    {
        /// <summary>
        /// Appbar dock mode
        /// </summary>
        public AppBarDockMode DockMode
        {
            get => (AppBarDockMode)GetValue(DockModeProperty);
            set => SetValue(DockModeProperty, value);
        }

        /// <summary>
        /// Appbar dock mode
        /// </summary>
        public static readonly DependencyProperty DockModeProperty = DependencyProperty.Register(
            nameof(DockMode),
            typeof(AppBarDockMode),
            typeof(AudioControl),
            new FrameworkPropertyMetadata(AppBarDockMode.Bottom));
       
        public AudioControl()
        {
            InitializeComponent();
        }
    }
}
