using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using net.adamec.ui.AppSwitcherBar.Dto;

namespace net.adamec.ui.AppSwitcherBar.Wpf
{
    /// <summary>
    /// Customized <see cref="Button"/> used to present the application window in the app bar
    /// </summary>
    internal class AppWindowButton : Button
    {

        /// <summary>
        /// Application window information
        /// </summary>
        public WndInfo WindowInfo
        {
            get => (WndInfo)GetValue(WindowInfoProperty);
            set => SetValue(WindowInfoProperty, value);
        }

        /// <summary>
        /// Application window information
        /// </summary>
        public static readonly DependencyProperty WindowInfoProperty = DependencyProperty.Register(
            "WindowInfo",
            typeof(WndInfo),
            typeof(AppWindowButton),
            new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Command to be called (from popup to model view) to show the thumbnail
        /// </summary>
        public ICommand? BuildContextMenuCommand
        {
            get => (ICommand)GetValue(BuildContextMenuCommandProperty);
            set => SetValue(BuildContextMenuCommandProperty, value);
        }

        /// <summary>
        /// Command to be called (from button to model view) to build the context menu
        /// </summary>
        public static readonly DependencyProperty BuildContextMenuCommandProperty = DependencyProperty.Register(
            "BuildContextMenuCommand",
            typeof(ICommand),
            typeof(AppWindowButton),
            new FrameworkPropertyMetadata(null));


        /// <summary>
        /// Called when the context menu is opening
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            base.OnContextMenuOpening(e);

            BuildContextMenuCommand?.Execute(new BuildContextMenuCommandParams(this, WindowInfo));
        }
    }
}
