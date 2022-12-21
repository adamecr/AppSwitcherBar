using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

// ReSharper disable once IdentifierTypo
namespace net.adamec.ui.AppSwitcherBar.Win32.Services
{
    /// <summary>
    /// Base for classes that need to be <see cref="IDisposable"/> and <see cref="INotifyPropertyChanged"/>
    /// </summary>
    public abstract class DisposableWithNotification : Disposable, INotifyPropertyChanged
    {
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
