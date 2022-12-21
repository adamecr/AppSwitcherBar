using System;
using net.adamec.ui.AppSwitcherBar.AppBar;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace net.adamec.ui.AppSwitcherBar.Views
{
    /// <summary>
    /// Interaction logic for ClockControl.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class ClockControl : UserControl
    {
        /// <summary>
        /// Appbar dock mode
        /// </summary>
        public AppBarDockMode DockMode
        {
            get => (AppBarDockMode)GetValue(DockModeProperty);
            set => SetValue(DockModeProperty, value);
        }

        /// <summary>
        /// Appbar dock mode
        /// </summary>
        public static readonly DependencyProperty DockModeProperty = DependencyProperty.Register(
            nameof(DockMode),
            typeof(AppBarDockMode),
            typeof(ClockControl),
            new FrameworkPropertyMetadata(AppBarDockMode.Bottom));


        /// <summary>
        /// Flag whether to show time zones popup
        /// </summary>
        public bool IsTimeZonesPopupOpen
        {
            get => (bool)GetValue(IsTimeZonesPopupOpenPropertyKey.DependencyProperty);
            private set => SetValue(IsTimeZonesPopupOpenPropertyKey, value);
        }

        /// <summary>
        /// Flag whether to show time zones popup
        /// </summary>
        private static readonly DependencyPropertyKey IsTimeZonesPopupOpenPropertyKey = DependencyProperty.RegisterReadOnly(
            nameof(IsTimeZonesPopupOpen),
            typeof(bool),
            typeof(ClockControl),
            new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Delay (in milliseconds) before the time zones popup is shown
        /// </summary>
        public int TimeZonesPopupDelay
        {
            get => (int)GetValue(TimeZonesPopupDelayProperty);
            set => SetValue(TimeZonesPopupDelayProperty, value);
        }

        /// <summary>
        /// Delay (in milliseconds) before the time zones popup is shown
        /// </summary>
        public static readonly DependencyProperty TimeZonesPopupDelayProperty = DependencyProperty.Register(
            nameof(TimeZonesPopupDelay),
            typeof(int),
            typeof(ClockControl),
            new FrameworkPropertyMetadata(400));

        /// <summary>
        /// Timer used to delay time zones popup show
        /// </summary>
        private DispatcherTimer? timeZonesPopupTimer;
        /// <summary>
        /// Timer used to delay popup show.
        /// Existing timer is reset when a value is set
        /// </summary>
        private DispatcherTimer? TimeZonesPopupTimer
        {
            get => timeZonesPopupTimer;
            set
            {
                ResetTimeZonesPopupTimer();
                timeZonesPopupTimer = value;
            }
        }

        public ClockControl()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// Hides the time zones popup if open
        /// </summary>
        public void HideTimeZonesPopup()
        {
            ResetTimeZonesPopupTimer();
            IsTimeZonesPopupOpen = false;
        }

        /// <summary>
        /// Show time zones popup on mouse enter
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if (TimeZonesPopupDelay > 0)
            {
                TimeZonesPopupTimer = new DispatcherTimer(DispatcherPriority.Normal)
                {
                    Interval = TimeSpan.FromMilliseconds(TimeZonesPopupDelay)
                };
                TimeZonesPopupTimer.Tick += (_, _) => IsTimeZonesPopupOpen = true;
                TimeZonesPopupTimer.Start();
            }
            else
            {
                IsTimeZonesPopupOpen = true;
            }
        }

        /// <summary>
        /// Resets (stops) the <see cref="timeZonesPopupTimer"/> if exist
        /// </summary>
        private void ResetTimeZonesPopupTimer()
        {
            timeZonesPopupTimer?.Stop();
            timeZonesPopupTimer = null;
        }

        /// <summary>
        /// Hide time zones popup on mouse leave
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            HideTimeZonesPopup();
        }
    }
}
