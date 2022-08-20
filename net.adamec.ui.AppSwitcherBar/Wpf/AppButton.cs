using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using net.adamec.ui.AppSwitcherBar.Dto;

namespace net.adamec.ui.AppSwitcherBar.Wpf
{
    /// <summary>
    /// Customized <see cref="Button"/> used to present the application window or pinned application in the app bar
    /// </summary>
    internal class AppButton : Button
    {
        /// <summary>
        /// Flag whether the button supports thumbnails, allowed for window buttons only
        /// </summary>
        private bool IsThumbnailAllowed => ButtonInfo is WndInfo;

        /// <summary>
        /// Application button information
        /// </summary>
        public ButtonInfo ButtonInfo
        {
            get => (ButtonInfo)GetValue(ButtonInfoProperty);
            set => SetValue(ButtonInfoProperty, value);
        }

        /// <summary>
        /// Application button information
        /// </summary>
        public static readonly DependencyProperty ButtonInfoProperty = DependencyProperty.Register(
            nameof(ButtonInfo),
            typeof(ButtonInfo),
            typeof(AppButton),
            new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Command to be called (from popup to model view) to build the context menu
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
            nameof(BuildContextMenuCommand),
            typeof(ICommand),
            typeof(AppButton),
            new FrameworkPropertyMetadata(null));
     
        /// <summary>
        /// Command to be called (from popup to model view) to launch the pinned application
        /// </summary>
        public ICommand? LaunchPinnedAppCommand
        {
            get => (ICommand)GetValue(LaunchPinnedAppCommandProperty);
            set => SetValue(LaunchPinnedAppCommandProperty, value);
        }

        /// <summary>
        /// Command to be called (from button to model view) to launch the pinned application
        /// </summary>
        public static readonly DependencyProperty LaunchPinnedAppCommandProperty = DependencyProperty.Register(
            nameof(LaunchPinnedAppCommand),
            typeof(ICommand),
            typeof(AppButton),
            new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Flag whether the button can show thumbnail
        /// </summary>
        public bool CanShowThumbnail
        {
            get => (bool)GetValue(CanShowThumbnailPropertyKey.DependencyProperty);
            private set => SetValue(CanShowThumbnailPropertyKey, value);
        }

        /// <summary>
        /// Flag whether the button can show thumbnail
        /// </summary>
        private static readonly DependencyPropertyKey CanShowThumbnailPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(CanShowThumbnail),
            typeof(bool),
            typeof(AppButton),
            new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Flag whether the button is source for drag and drop while reordering
        /// </summary>
        public bool IsDragAndDropSource
        {
            get => (bool)GetValue(IsDragAndDropSourcePropertyKey.DependencyProperty);
            private set => SetValue(IsDragAndDropSourcePropertyKey, value);
        }

        /// <summary>
        /// Flag whether the button is source for drag and drop while reordering
        /// </summary>
        private static readonly DependencyPropertyKey IsDragAndDropSourcePropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(IsDragAndDropSource),
            typeof(bool),
            typeof(AppButton),
            new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Flag whether the button is source for drag and drop while reordering
        /// </summary>
        /// <remarks>This is needed for proper behavior of XAML Designer</remarks>
        // ReSharper disable once UnusedMember.Global
        public static readonly DependencyProperty IsDragAndDropSourceProperty = IsDragAndDropSourcePropertyKey.DependencyProperty;

        /// <summary>
        /// Flag whether the button is target for drag and drop while reordering
        /// </summary>
        public bool IsDragAndDropTarget
        {
            get => (bool)GetValue(IsDragAndDropTargetPropertyKey.DependencyProperty);
            private set => SetValue(IsDragAndDropTargetPropertyKey, value);
        }

        /// <summary>
        /// Flag whether the button is target for drag and drop while reordering
        /// </summary>
        private static readonly DependencyPropertyKey IsDragAndDropTargetPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(IsDragAndDropTarget),
            typeof(bool),
            typeof(AppButton),
            new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Flag whether the button is target for drag and drop while reordering
        /// </summary>
        /// <remarks>This is needed for proper behavior of XAML Designer</remarks>
        // ReSharper disable once UnusedMember.Global
        public static readonly DependencyProperty IsDragAndDropTargetProperty = IsDragAndDropTargetPropertyKey.DependencyProperty;

        /// <summary>
        /// Called when the context menu is opening
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            base.OnContextMenuOpening(e);

            BuildContextMenuCommand?.Execute(new BuildContextMenuCommandParams(this, ButtonInfo));
        }

        /// <summary>
        /// Sets the <see cref="IsDragAndDropSource"/> property to given <paramref name="value"/>
        /// </summary>
        /// <param name="value">Flag whether the button is to be marked as drag and drop source</param>
        public void SetIsDragAndDropSource(bool value)
        {
            IsDragAndDropSource = value;
#if DEBUG
            Debug.WriteLine($"SetIsDragAndDropSource {value} {ButtonInfo.Title}");
#endif
        }

        /// <summary>
        /// Sets the <see cref="IsDragAndDropTarget"/> property to given <paramref name="value"/>
        /// </summary>
        /// <param name="value">Flag whether the button is to be marked as drag and drop target</param>
        public void SetIsDragAndDropTarget(bool value)
        {
            IsDragAndDropTarget = value;
#if DEBUG
            Debug.WriteLine($"SetIsDragAndDropTarget {value} {ButtonInfo.Title}");
#endif
        }

        /// <summary>
        /// Hides the thumbnail if open
        /// </summary>
        public void HideThumbnail()
        {
            CanShowThumbnail = false;
        }

        /// <summary>
        /// Show thumbnail on mouse enter is <see cref="IsThumbnailAllowed"/>
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            CanShowThumbnail = IsThumbnailAllowed;
        }

        /// <summary>
        /// Hide thumbnail on mouse leave
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            HideThumbnail();
        }

        /// <summary>
        /// Hide thumbnail when button is clicked
        /// </summary>
        protected override void OnClick()
        {
            HideThumbnail();
            base.OnClick();
        }
    }
}
