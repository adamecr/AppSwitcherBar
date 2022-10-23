using System.Collections.Generic;
using System.Windows.Input;
using System.Windows;

namespace net.adamec.ui.AppSwitcherBar.Wpf
{
    /// <summary>
    /// Behavior allowing to execute a command (<see cref="CommandProperty"/>) when a key (<see cref="KeysProperty"/>) is pressed
    /// </summary>
    public class KeyCommand
    {
        /// <summary>
        /// List of keys triggering the command
        /// </summary>
        public static readonly DependencyProperty KeysProperty = DependencyProperty.RegisterAttached(
            "Keys",
            typeof(List<Key>),
            typeof(KeyCommand),
            new FrameworkPropertyMetadata(new List<Key>(), Attach));

        /// <summary>
        /// Gets the <see cref="KeysProperty"/> value
        /// </summary>
        /// <param name="dependencyObject">Dependency object to get the value from</param>
        /// <returns>The value of <see cref="KeysProperty"/></returns>
        public static List<Key>? GetKeys(DependencyObject? dependencyObject) => (List<Key>?)dependencyObject?.GetValue(KeysProperty);

        /// <summary>
        /// Sets the <see cref="KeysProperty"/> value
        /// </summary>
        /// <param name="dependencyObject">Dependency object to set the value to</param>
        /// <param name="keys">New value</param>
        public static void SetKeys(DependencyObject? dependencyObject, List<Key>? keys) => dependencyObject?.SetValue(KeysProperty, keys);

        /// <summary>
        /// <see cref="ICommand"/> to be triggered when a key is pressed
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
            "Command",
            typeof(ICommand),
            typeof(KeyCommand),
            new FrameworkPropertyMetadata(null, Attach));

        /// <summary>
        /// Gets the <see cref="CommandProperty"/> value
        /// </summary>
        /// <param name="dependencyObject">Dependency object to get the value from</param>
        /// <returns>The value of <see cref="CommandProperty"/></returns>
        public static ICommand? GetCommand(DependencyObject? dependencyObject) => (ICommand?)dependencyObject?.GetValue(CommandProperty);
        /// <summary>
        /// Sets the <see cref="CommandProperty"/> value
        /// </summary>
        /// <param name="dependencyObject">Dependency object to set the value to</param>
        /// <param name="command">New value</param>
        public static void SetCommand(DependencyObject? dependencyObject, ICommand? command) => dependencyObject?.SetValue(CommandProperty, command);

        /// <summary>
        /// Attaches the behavior to element - sets key handler to <see cref="KeyUpHandler"/>
        /// </summary>
        /// <param name="dependencyObject">Dependency object to attach the behavior to</param>
        /// <param name="e">Event arguments containing the reference to changed (set) property and old+new value</param>
        private static void Attach(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is not UIElement element) return;

            element.KeyUp -= KeyUpHandler;
            element.KeyUp += KeyUpHandler;
        }

        /// <summary>
        /// Element's key up handler - checks whether the pressed key is in the list of keys (<see cref="KeysProperty"/>) and
        /// if yes, execute the command from <see cref="CommandProperty"/>
        /// </summary>
        /// <param name="sender">UI element</param>
        /// <param name="e">Event arguments</param>
        private static void KeyUpHandler(object sender, KeyEventArgs e)
        {
            if (sender is not DependencyObject d) return;

            var command = GetCommand(d);
            var keys = GetKeys(d);

            if (command is null || keys is null || keys.Count == 0) return;

            if (keys.Contains(e.Key) && command.CanExecute(e.Key))
            {
                command.Execute(e.Key);
            }
        }
    }

}
