using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using net.adamec.ui.AppSwitcherBar.Dto;
using net.adamec.ui.AppSwitcherBar.ViewModel;

namespace net.adamec.ui.AppSwitcherBar.WpfExt
{
    /// <summary>
    /// Customized <see cref="Button"/> used to present the application window or pinned application in the app bar
    /// </summary>
    public class AppButton : Wpf.Ui.Controls.Button
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
        /// Delay (in milliseconds) before the thumbnail is shown
        /// </summary>
        public int ThumbnailDelay
        {
            get => (int)GetValue(ThumbnailDelayProperty);
            set => SetValue(ThumbnailDelayProperty, value);
        }

        /// <summary>
        /// Delay (in milliseconds) before the thumbnail is shown
        /// </summary>
        public static readonly DependencyProperty ThumbnailDelayProperty = DependencyProperty.Register(
            nameof(ThumbnailDelay),
            typeof(int),
            typeof(AppButton),
            new FrameworkPropertyMetadata(400));

        /// <summary>
        /// Timer used to delay thumbnail show
        /// </summary>
        private DispatcherTimer? thumbnailTimer;
        /// <summary>
        /// Timer used to delay thumbnail show.
        /// Existing timer is reset when a value is set
        /// </summary>
        private DispatcherTimer? ThumbnailTimer
        {
            get => thumbnailTimer;
            set
            {
                ResetThumbnailTimer();
                thumbnailTimer = value;
            }
        }

        /// <summary>
        /// CTOR - allow drop (be a drop target for file)
        /// </summary>
        public AppButton() 
        {
            AllowDrop = true;
        }

        /// <summary>
        /// Invoked when the "object" (file) is dragged into the app button
        /// Just show the application window to allow to drag and drop file in there
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                //if it's file, show the application window to allow to drag and drop file in there
                var bar = WpfTools.FindVisualAncestor<MainWindow>(this);
                if (bar is { DataContext: MainViewModel main } && DataContext is WndInfo wndInfo)
                {
                    main.ToggleApplicationWindow(wndInfo.Hwnd, true);
                }
            }

            //we don't want to drop it to app button
            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        /// <summary>
        /// Invoked when the "object" (file) is dragged into the app button
        /// We don't want to drop it to app button - show the <see cref="DragDropEffects.None"/>
        /// (it's a visual hint, even if the file is dropped here, nothing happen as the Drop handler is not implemented)
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);

            //we don't want to drop it to app button
            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }


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
            ResetThumbnailTimer();
            CanShowThumbnail = false;
        }

        /// <summary>
        /// Show thumbnail on mouse enter is <see cref="IsThumbnailAllowed"/>
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if (IsThumbnailAllowed)
            {
                if (ThumbnailDelay > 0)
                {
                    ThumbnailTimer = new DispatcherTimer(DispatcherPriority.Normal)
                    {
                        Interval = TimeSpan.FromMilliseconds(ThumbnailDelay)
                    };
                    ThumbnailTimer.Tick += (_, _) => CanShowThumbnail = true;
                    ThumbnailTimer.Start();
                }
                else
                {
                    CanShowThumbnail = true;
                }
            }
            else
            {
                CanShowThumbnail = false;
            }
        }

        /// <summary>
        /// Resets (stops) the <see cref="thumbnailTimer"/> if exist
        /// </summary>
        private void ResetThumbnailTimer()
        {
            thumbnailTimer?.Stop();
            thumbnailTimer = null;
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
