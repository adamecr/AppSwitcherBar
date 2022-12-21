using System;
using System.Runtime.CompilerServices;

// ReSharper disable InvertIf
// ReSharper disable IdentifierTypo

namespace net.adamec.ui.AppSwitcherBar.Dto
{
    /// <summary>
    /// Information about the window (task bar application)
    /// </summary>
    public class WndInfo : ButtonInfo
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
                    if (isForeground)
                    {
                        RunStats.UpdateForeground();
                        InstalledApplication?.RunStats.UpdateForeground();
                        PinnedApplication?.RunStats.UpdateForeground();
                    }
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Reference to installed application if any
        /// </summary>
        public InstalledApplication? InstalledApplication { get; set; }

        /// <summary>
        /// Reference to pinned application if any
        /// </summary>
        public PinnedAppInfo? PinnedApplication { get; set; }

        /// <summary>
        /// Change status used when (re)evaluating the windows
        /// </summary>
        public ChangeStatusEnum ChangeStatus { get; private set; }

        /// <summary>
        /// <see cref="ProcessId"/> is used as a fallback when neither AppId not Executable is known or not set yet
        /// </summary>
        protected override string GroupFallback => ProcessId.ToString();

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="hWnd">Window handle</param>
        /// <param name="title">Window title</param>
        /// <param name="threadId">Window's thread ID</param>
        /// <param name="processId">Window's process ID</param>
        /// <param name="executable">Application executable</param>
        public WndInfo(IntPtr hWnd, string title, uint threadId, uint processId, string? executable)
        : base(title, executable, null)
        {
            ChangeStatus = ChangeStatusEnum.New;
            Hwnd = hWnd;
            ThreadId = threadId;
            ProcessId = processId;
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
        /// Sets <see cref="ButtonInfo.GroupIndex"/> and <see cref="ButtonInfo.WindowIndex"/>
        /// </summary>
        /// <param name="groupIndex">New <see cref="ButtonInfo.GroupIndex"/></param>
        /// <param name="windowIndex">New <see cref="ButtonInfo.WindowIndex"/></param>
        /// <returns>This object</returns>
        public override WndInfo SetIndicies(int groupIndex, int windowIndex)
        {
            base.SetIndicies(groupIndex, windowIndex);
            return this;
        }


        /// <summary>
        /// Raise <see cref="ButtonInfo.PropertyChanged"/> event
        /// </summary>
        /// <param name="propertyName">Name of the property changed</param>
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (ChangeStatus != ChangeStatusEnum.New) ChangeStatus = ChangeStatusEnum.Changed;
            base.OnPropertyChanged(propertyName);
        }

        /// <summary>
        /// Returns the string representation of the object
        /// </summary>
        /// <returns>String representation of the object</returns>
        public override string ToString()
        {
            return $"{Title} ({Hwnd}) Idx:{GroupIndex}/{WindowIndex} T:{ThreadId}, P:{ProcessId}, A:{AppId}, E:{Executable}";
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
