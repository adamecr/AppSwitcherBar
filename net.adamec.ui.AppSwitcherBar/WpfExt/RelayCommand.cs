using System;
using System.Windows.Input;

namespace net.adamec.ui.AppSwitcherBar.WpfExt
{
    /// <summary>
    /// <see cref="ICommand"/> implementation used for mapping the UI Elements to view model Action 
    /// </summary>
    internal class RelayCommand : ICommand
    {
        /// <summary>
        /// Command action to be executed with optional command parameter 
        /// </summary>
        readonly Action<object?>? executeAction;
        /// <summary>
        /// Function evaluating whether the command can be executed with given (optional) command parameter
        /// </summary>
        readonly Func<object?, bool>? canExecuteFunction;

        /// <summary>
        /// CTOR for commands with parameter
        /// </summary>
        /// <param name="execute">Command action to be executed with command parameter </param>
        /// <param name="canExecute">Optional function evaluating whether the command can be executed with given command parameter.
        /// When not provided, the command is always executed</param>
        /// <exception cref="ArgumentNullException">Throws <see cref="ArgumentNullException"/> when the command <paramref name="execute"/> action is null</exception>
        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            executeAction = execute ?? throw new ArgumentNullException(nameof(execute));
            canExecuteFunction = canExecute;
        }

        /// <summary>
        /// CTOR for commands without parameter
        /// </summary>
        /// <param name="execute">Command action to be executed</param>
        /// <param name="canExecute">Optional function evaluating whether the command can be executed. When not provided, the command is always executed</param>
        /// <exception cref="ArgumentNullException">Throws <see cref="ArgumentNullException"/> when the command <paramref name="execute"/> action is null</exception>
        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            if (execute is null) throw new ArgumentNullException(nameof(execute));

            executeAction = _ => execute();
            if (canExecute != null) canExecuteFunction = _ => canExecute();
        }

        /// <summary>
        /// Occurs when a change impacting the ability to execute (CanExecute) changes
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// Raise the <see cref="CanExecuteChanged"/> event
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Evaluates whether the command can be executed
        /// </summary>
        /// <param name="parameter">Optional command parameter</param>
        /// <returns>True when the command can be executed, otherwise false</returns>
        public bool CanExecute(object? parameter) => canExecuteFunction?.Invoke(parameter) ?? true;

        /// <summary>
        /// Execute the command
        /// </summary>
        /// <param name="parameter">Optional command parameter</param>
        public void Execute(object? parameter) => executeAction?.Invoke(parameter);
    }
}
