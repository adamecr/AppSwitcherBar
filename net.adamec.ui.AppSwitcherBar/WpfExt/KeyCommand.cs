using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace net.adamec.ui.AppSwitcherBar.WpfExt
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
        /// Include letter keys to trigger the command
        /// </summary>
        public static readonly DependencyProperty IncludeLettersProperty = DependencyProperty.RegisterAttached(
            "IncludeLetters",
            typeof(bool),
            typeof(KeyCommand),
            new FrameworkPropertyMetadata(false, Attach));

        /// <summary>
        /// Gets the <see cref="IncludeLettersProperty"/> value
        /// </summary>
        /// <param name="dependencyObject">Dependency object to get the value from</param>
        /// <returns>The value of <see cref="IncludeLettersProperty"/></returns>
        public static bool GetIncludeLetters(DependencyObject? dependencyObject) => (bool)(dependencyObject?.GetValue(IncludeLettersProperty) ?? false);

        /// <summary>
        /// Sets the <see cref="IncludeLettersProperty"/> value
        /// </summary>
        /// <param name="dependencyObject">Dependency object to set the value to</param>
        /// <param name="includeLetter">New value</param>
        public static void SetIncludeLetters(DependencyObject? dependencyObject, bool includeLetter) => dependencyObject?.SetValue(IncludeLettersProperty, includeLetter);

        /// <summary>
        /// <see cref="ICommand"/> to be triggered when a key is pressed
        /// It's executed with <see cref="CommandParameters"/> containing the <see cref="UIElement"/> sender and <see cref="Key"/> pressed
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
        /// if yes, execute the command from <see cref="CommandProperty"/> with <see cref="CommandParameters"/> containing the <paramref name="sender"/> and <see cref="Key"/> pressed
        /// </summary>
        /// <param name="sender">UI element</param>
        /// <param name="e">Event arguments</param>
        private static void KeyUpHandler(object sender, KeyEventArgs e)
        {
            if (sender is not UIElement element) return;

            var command = GetCommand(element);
            var keys = GetKeys(element);
            var includeLetters = GetIncludeLetters(element);

            if (command is null || keys is null || keys.Count == 0) return;

            if ((keys.Contains(e.Key) || (includeLetters && e.Key is >= Key.A and <= Key.Z)) && command.CanExecute(e.Key))
            {
                command.Execute(new CommandParameters(element,e.Key));
            }
        }


        /// <summary>
        /// Command parameters provided to <see cref="KeyCommand.CommandProperty"/> defined <see cref="ICommand"/> when executed
        /// </summary>
        public class CommandParameters
        {
            /// <summary>
            /// UI element sending the key
            /// </summary>
            public UIElement Sender { get; }
            /// <summary>
            /// Key pressed
            /// </summary>
            public Key Key { get; }

            /// <summary>
            /// CTOR
            /// </summary>
            /// <param name="sender">UI element sending the key</param>
            /// <param name="key">Key pressed</param>
            public CommandParameters(UIElement sender, Key key)
            {
                Sender = sender ?? throw new ArgumentNullException(nameof(sender));
                Key = key;
            }
        }
    }

}
