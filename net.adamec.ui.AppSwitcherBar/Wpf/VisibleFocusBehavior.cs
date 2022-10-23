using System.Windows;

namespace net.adamec.ui.AppSwitcherBar.Wpf
{
    /// <summary>
    /// Behavior allowing to set focus of <see cref="UIElement"/> when become visible
    /// </summary>
    public class VisibleFocusBehavior
    {
        /// <summary>
        /// When the property is set, the <see cref="UIElement"/> gets focus when become visible
        /// </summary>
        public static readonly DependencyProperty FocusOnVisibleProperty = DependencyProperty.RegisterAttached(
            "FocusOnVisible",
            typeof(bool),
            typeof(VisibleFocusBehavior),
            new UIPropertyMetadata(false, Attach));

        /// <summary>
        /// Gets the <see cref="FocusOnVisibleProperty"/> value
        /// </summary>
        /// <param name="dependencyObject">Dependency object to get the value from</param>
        /// <returns>The value of <see cref="FocusOnVisibleProperty"/></returns>
        public static bool GetFocusOnVisible(DependencyObject dependencyObject) => (bool)dependencyObject.GetValue(FocusOnVisibleProperty);

        /// <summary>
        /// Sets the <see cref="FocusOnVisibleProperty"/> value
        /// </summary>
        /// <param name="dependencyObject">Dependency object to set the value to</param>
        /// <param name="value">New value</param>
        public static void SetFocusOnVisible(DependencyObject dependencyObject, bool value) => dependencyObject.SetValue(FocusOnVisibleProperty, value);


        /// <summary>
        /// Attaches the behavior to element - sets visible changed handler to <see cref="ElementIsVisibleChanged"/> if enabled
        /// </summary>
        /// <param name="dependencyObject">Dependency object to attach the behavior to</param>
        /// <param name="e">Event arguments containing the reference to changed (set) property and old+new value</param>
        private static void Attach(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is not UIElement element) return;

            element.IsVisibleChanged -= ElementIsVisibleChanged;
            if (e.NewValue is true) element.IsVisibleChanged += ElementIsVisibleChanged;
        }

        /// <summary>
        /// Element's visibility changed handler - set focus when visible
        /// </summary>
        /// <param name="sender">UI element</param>
        /// <param name="e">Event arguments</param>
        private static void ElementIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is UIElement { IsVisible: true } element) element.Focus();
        }
    }
}
