using System.Windows;

namespace net.adamec.ui.AppSwitcherBar.WpfExt
{
    /// <summary>
    /// Behavior allowing to set IsActive property of <see cref="UIElement"/>
    /// </summary>
    public class IsActiveBehavior
    {
        /// <summary>
        /// IsActive value holder
        /// </summary>
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.RegisterAttached(
            "IsActive",
            typeof(bool),
            typeof(IsActiveBehavior),
            new UIPropertyMetadata(false, Attach));

        /// <summary>
        /// Gets the <see cref="IsActiveProperty"/> value
        /// </summary>
        /// <param name="dependencyObject">Dependency object to get the value from</param>
        /// <returns>The value of <see cref="IsActiveProperty"/></returns>
        public static bool GetIsActive(DependencyObject dependencyObject) => (bool)dependencyObject.GetValue(IsActiveProperty);

        /// <summary>
        /// Sets the <see cref="IsActiveProperty"/> value
        /// </summary>
        /// <param name="dependencyObject">Dependency object to set the value to</param>
        /// <param name="value">New value</param>
        public static void SetIsActive(DependencyObject dependencyObject, bool value) => dependencyObject.SetValue(IsActiveProperty, value);


        /// <summary>
        /// ShowIndicator value holder
        /// </summary>
        public static readonly DependencyProperty ShowIndicatorProperty = DependencyProperty.RegisterAttached(
            "ShowIndicator",
            typeof(bool),
            typeof(IsActiveBehavior),
            new UIPropertyMetadata(true, Attach));

        /// <summary>
        /// Gets the <see cref="ShowIndicatorProperty"/> value
        /// </summary>
        /// <param name="dependencyObject">Dependency object to get the value from</param>
        /// <returns>The value of <see cref="ShowIndicatorProperty"/></returns>
        public static bool GetShowIndicator(DependencyObject dependencyObject) => (bool)dependencyObject.GetValue(ShowIndicatorProperty);

        /// <summary>
        /// Sets the <see cref="ShowIndicatorProperty"/> value
        /// </summary>
        /// <param name="dependencyObject">Dependency object to set the value to</param>
        /// <param name="value">New value</param>
        public static void SetShowIndicator(DependencyObject dependencyObject, bool value) => dependencyObject.SetValue(ShowIndicatorProperty, value);

        /// <summary>
        /// Attaches the behavior to element
        /// </summary>
        /// <param name="dependencyObject">Dependency object to attach the behavior to</param>
        /// <param name="e">Event arguments containing the reference to changed (set) property and old+new value</param>
        private static void Attach(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            //nothing special
        }


    }
}
