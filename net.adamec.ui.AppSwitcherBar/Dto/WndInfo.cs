using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
// ReSharper disable InvertIf

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
        /// Window's thread ID
        /// </summary>
        public uint ThreadId { get; }

        /// <summary>
        /// Window's process ID
        /// </summary>
        public uint ProcessId { get; }

        /// <summary>
        /// Application executable
        /// </summary>
        public string? Executable { get; }



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
        /// Some of the shell properties
        /// </summary>
        private Properties shellProperties = new();

        /// <summary>
        /// Some of the shell properties
        /// </summary>
        public Properties ShellProperties
        {
            get => shellProperties;
            set
            {
                if (shellProperties != value)
                {
                    shellProperties = value;
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
        /// Identifier of the "group" the window belongs to.
        /// It's used for grouping the window buttons of the same application together
        /// Priority: <see cref="AppId"/>, <see cref="Executable"/>, <see cref="ProcessId"/>
        /// </summary>
        public string Group =>
            !string.IsNullOrEmpty(AppId) ? AppId : //includes the "Executable option" as the Executable is the fallback source for AppId whenever the AppId is set
            !string.IsNullOrEmpty(Executable) ? Executable : //when AppId is not set yet and executable is set
            ProcessId.ToString(); //process ID is used as a fallback when neither AppId not Executable is known or not set yet

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="hWnd">Window handle</param>
        /// <param name="title">Window title</param>
        /// <param name="threadId">Window's thread ID</param>
        /// <param name="processId">Window's process ID</param>
        /// <param name="executable">Application executable</param>
        public WndInfo(IntPtr hWnd, string title, uint threadId, uint processId, string? executable)
        {
            ChangeStatus = ChangeStatusEnum.New;
            this.title = title;
            Hwnd = hWnd;
            ThreadId = threadId;
            ProcessId = processId;
            Executable = executable;
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
            return $"{Title} ({Hwnd}) T:{ThreadId}, P:{ProcessId}, A:{AppId}, E:{Executable}";
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
