using System.Windows;
using System.Windows.Controls;
using net.adamec.ui.AppSwitcherBar.ViewModel;
using net.adamec.ui.AppSwitcherBar.WpfExt;

namespace net.adamec.ui.AppSwitcherBar.Views
{
    /// <summary>
    /// Interaction logic for MenuPopup.xaml
    /// </summary>
    public partial class MenuPopup : UserControl
    {
        
        public MenuPopup()
        {
            InitializeComponent();

            IsVisibleChanged += (_, _) =>
            {
                if (DataContext is MenuPopupViewModel viewModel)
                {
                    viewModel.IsInMenuPopup = IsVisible;
                    if (IsVisible) viewModel.FillThemeResourcesColl();
                }
            };
        }


        private void HamburgerClick(object sender, RoutedEventArgs e)
        {
            if (sender is DependencyObject d)
            {
                IsActiveBehavior.SetIsActive(d, !IsActiveBehavior.GetIsActive(d));
            }
        }

        
    }
}
