using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using net.adamec.ui.AppSwitcherBar.Win32.NativeStructs;
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace net.adamec.ui.AppSwitcherBar.Wpf
{
    /// <summary>
    /// Customized <see cref="Popup"/> used to show the window thumbnails (provided by DWM API)
    /// </summary>
    internal class ThumbnailPopup : Popup
    {

        /// <summary>
        /// Source window HWND
        /// </summary>
        public IntPtr SourceHwnd
        {
            get => (IntPtr)GetValue(SourceHwndProperty);
            set => SetValue(SourceHwndProperty, value);
        }

        /// <summary>
        /// Source window HWND
        /// </summary>
        public static readonly DependencyProperty SourceHwndProperty = DependencyProperty.Register(
            nameof(SourceHwnd),
            typeof(IntPtr),
            typeof(ThumbnailPopup),
            new FrameworkPropertyMetadata(IntPtr.Zero));

        /// <summary>
        /// Command to be called (from popup to model view) to show the thumbnail
        /// </summary>
        public ICommand? ThumbnailShowCommand
        {
            get => (ICommand)GetValue(ThumbnailShowCommandProperty);
            set => SetValue(ThumbnailShowCommandProperty, value);
        }

        /// <summary>
        /// Command to be called (from popup to model view) to show the thumbnail
        /// </summary>
        public static readonly DependencyProperty ThumbnailShowCommandProperty = DependencyProperty.Register(
            nameof(ThumbnailShowCommand),
            typeof(ICommand),
            typeof(ThumbnailPopup),
            new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Command to be called (from popup to model view) to hide the thumbnail
        /// </summary>
        public ICommand? ThumbnailHideCommand
        {
            get => (ICommand)GetValue(ThumbnailHideCommandProperty);
            set => SetValue(ThumbnailHideCommandProperty, value);
        }
        /// <summary>
        /// Command to be called (from popup to model view) to hide the thumbnail
        /// </summary>
        public static readonly DependencyProperty ThumbnailHideCommandProperty = DependencyProperty.Register(
            nameof(ThumbnailHideCommand),
            typeof(ICommand),
            typeof(ThumbnailPopup),
            new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Called when the popup is shown (<see cref="Popup.IsOpen"/> changes from false to true)
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                //send the show thumbnail command
                ThumbnailShowCommand?.Execute(new ThumbnailPopupCommandParams(GetPopupHwnd(), GetThumbnailTargetRectWithinPopupContent(6), SourceHwnd));
            }), DispatcherPriority.ContextIdle, null);
        }

        /// <summary>
        /// Called when the popup is closed (<see cref="Popup.IsOpen"/> changes from true to false)
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            //send the hide thumbnail command
            ThumbnailHideCommand?.Execute(null);
        }

        /// <summary>
        /// Retrieves the HWND of the popup
        /// </summary>
        /// <remarks>Popup is "special" within WPF as it's rendered in own window (besides the WPF Application window). It also have a dedicated visual tree for the content.
        /// So it's necessary to get the visual tree of popup through <see cref="Popup.Child"/> property that acts as an "interface" between the logical and visual tree.</remarks>
        /// <returns>HWND of the popup</returns>
        private IntPtr GetPopupHwnd()
        {
            var popupContent = Child; //Accessor to the popup content visual tree
            var source = (HwndSource?)PresentationSource.FromVisual(popupContent); //get the HWND source of dedicated popup window
            var handle = source?.Handle ?? IntPtr.Zero; //get the HWND
            return handle;
        }

        /// <summary>
        /// Gets the target rectangle that can be used to render the thumbnail within the popup content
        /// </summary>
        /// <param name="padding">Thumbnail rectangle padding</param>
        /// <returns>Thumbnail target rectangle</returns>
        /// <exception cref="InvalidOperationException">Thrown when can't get the popup content <see cref="FrameworkElement"/></exception>
        private RECT GetThumbnailTargetRectWithinPopupContent(int padding)
        {
            return GetThumbnailTargetRectWithinPopupContent(new Thickness(padding));
        }


        /// <summary>
        /// Gets the target rectangle that can be used to render the thumbnail within the popup content
        /// </summary>
        /// <param name="padding">Thumbnail rectangle padding</param>
        /// <returns>Thumbnail target rectangle</returns>
        /// <exception cref="InvalidOperationException">Thrown when can't get the popup content <see cref="FrameworkElement"/></exception>
        private RECT GetThumbnailTargetRectWithinPopupContent(Thickness padding)
        {
            var popupContent = Child;
            if (WpfTools.FindVisualRoot(popupContent) is not FrameworkElement popupRoot) throw new InvalidOperationException("Can't find the popup root");

            //popup bounds
            var popupContentRect = popupContent.TransformToVisual(popupRoot).TransformBounds(new Rect(popupRoot.RenderSize));

            //apply padding
            var popupContentRectWithPadding = new Rect(popupContentRect.Left + padding.Left, popupContentRect.Top + padding.Top, popupContentRect.Width - padding.Left - padding.Right, popupContentRect.Height - padding.Top - padding.Bottom);

            //apply DPI
            var popupRect = new RECT(
                this.WpfDimensionToScreen(popupContentRectWithPadding.Left),
                this.WpfDimensionToScreen(popupContentRectWithPadding.Top),
                this.WpfDimensionToScreen(popupContentRectWithPadding.Right),
                this.WpfDimensionToScreen(popupContentRectWithPadding.Bottom));
            return popupRect;
        }
    }
}
