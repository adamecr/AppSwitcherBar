using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace net.adamec.ui.AppSwitcherBar.Dto
{
    /// <summary>
    /// Information about the window (task bar application)
    /// </summary>
    public class WndInfo : INotifyPropertyChanged
    {
        /// <summary>
        /// Window handle
        /// </summary>
        public IntPtr Hwnd { get; }

        /// <summary>
        /// Window title
        /// </summary>
        public string title;
        /// <summary>
        /// Window title
        /// </summary>
        public string Title
        {
            get => title;
            set
            {
                if (title != value)
                {
                    title = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Icon bitmap source (if available)
        /// </summary>
        private BitmapSource? bitmapSource;
        /// <summary>
        /// Icon bitmap source (if available)
        /// </summary>
        public BitmapSource? BitmapSource
        {
            get => bitmapSource;
            set
            {
                if (bitmapSource != value)
                {
                    bitmapSource = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Flag whether it's current foreground window
        /// </summary>
        private bool isForeground;
        /// <summary>
        /// Flag whether it's current foreground window
        /// </summary>
        public bool IsForeground
        {
            get => isForeground;
            set
            {
                if (isForeground != value)
                {
                    isForeground = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Window's thread ID (handle)
        /// </summary>
        private IntPtr threadHandle = IntPtr.Zero;
        /// <summary>
        /// Window's thread ID (handle)
        /// </summary>
        public IntPtr ThreadHandle
        {
            get => threadHandle;
            set
            {
                if (threadHandle != value)
                {
                    threadHandle = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Window's process ID (handle)
        /// </summary>
        private IntPtr processHandle = IntPtr.Zero;
        /// <summary>
        /// Window's process ID (handle)
        /// </summary>
        public IntPtr ProcessHandle
        {
            get => processHandle;
            set
            {
                if (processHandle != value)
                {
                    processHandle = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// ApplicationUserModelId
        /// </summary>
        private string? appId;
        /// <summary>
        /// ApplicationUserModelId
        /// </summary>
        public string? AppId
        {
            get => appId;
            set
            {
                if (appId != value)
                {
                    appId = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Change status used when (re)evaluating the windows
        /// </summary>
        public ChangeStatusEnum ChangeStatus { get; private set; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="hWnd">Window handle</param>
        /// <param name="title">Window title</param>
        public WndInfo(IntPtr hWnd, string title)
        {
            ChangeStatus = ChangeStatusEnum.New;
            this.title = title;
            Hwnd = hWnd;
        }

        /// <summary>
        /// Marks the object for removal from all windows collection - window is not available anymore.
        /// Sets the <see cref="ChangeStatus"/> to <see cref="ChangeStatusEnum.ToRemove"/>
        /// </summary>
        public void MarkForRemoval()
        {
            ChangeStatus = ChangeStatusEnum.ToRemove;
        }
        /// <summary>
        /// Marks the object for keeping in all windows collection - window is still available and there might be no changes
        /// Sets the <see cref="ChangeStatus"/> to <see cref="ChangeStatusEnum.ToRemove"/>
        /// </summary>
        public void MarkToKeep()
        {
            ChangeStatus = ChangeStatusEnum.Keep;
        }

        /// <summary>
        /// Occurs when a property changes
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raise <see cref="PropertyChanged"/> event
        /// </summary>
        /// <param name="propertyName">Name of the property changed</param>
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (ChangeStatus != ChangeStatusEnum.New) ChangeStatus = ChangeStatusEnum.Changed;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Returns the string representation of the object
        /// </summary>
        /// <returns>String representation of the object</returns>
        public override string ToString()
        {
            return $"{Title} ({Hwnd}) T:{ThreadHandle}, P:{ProcessHandle}";
        }

        /// <summary>
        /// Change status type used when (re)evaluating the windows
        /// </summary>
        public enum ChangeStatusEnum
        {
            /// <summary>
            /// New window to be added to collection
            /// </summary>
            New,
            /// <summary>
            /// Existing window to be kept in collection without changes
            /// </summary>
            Keep,
            /// <summary>
            /// Existing window to be kept in collection with changes
            /// </summary>
            Changed,
            /// <summary>
            /// Window is not available anymore, remove for the collection
            /// </summary>
            ToRemove
        }
    }
}
