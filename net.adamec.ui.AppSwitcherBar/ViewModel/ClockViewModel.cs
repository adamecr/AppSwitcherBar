using Microsoft.Extensions.Options;
using net.adamec.ui.AppSwitcherBar.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Threading;
using net.adamec.ui.AppSwitcherBar.Win32.Services;
using net.adamec.ui.AppSwitcherBar.Dto;

namespace net.adamec.ui.AppSwitcherBar.ViewModel
{
    /// <summary>
    /// View model for ClockControl
    /// </summary>
    public class ClockViewModel : DisposableWithNotification
    {
        /// <summary>
        /// Application settings
        /// </summary>
        public IAppSettings Settings { get; }

        /// <summary>
        /// <see cref="DispatcherTimer"/> used to periodically update time and date
        /// </summary>
        private readonly DispatcherTimer timer;

        /// <summary>
        /// Current time - formatted
        /// </summary>
        private string time = string.Empty;
        /// <summary>
        /// Current time - formatted
        /// </summary>
        public string Time
        {
            get => time;
            private set
            {
                if (time == value) return;
                time = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Current time - formatted as "long format"
        /// </summary>
        private string dateTimeLong = string.Empty;
        /// <summary>
        /// Current time - formatted as "long format"
        /// </summary>
        public string DateTimeLong
        {
            get => dateTimeLong;
            private set
            {
                if (dateTimeLong == value) return;
                dateTimeLong = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Current date - formatted
        /// </summary>
        private string date = string.Empty;
        /// <summary>
        /// Current date - formatted
        /// </summary>
        public string Date
        {
            get => date;
            private set
            {
                if (date == value) return;
                date = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Flag whether to present additional timezones in popup
        /// </summary>
        public bool HasTimeZones { get; }

        /// <summary>
        /// Optional time zones to be presented in popup
        /// </summary>
        public ClockTimeZone[] TimeZones { get; } = Array.Empty<ClockTimeZone>();

        /// <summary>
        /// Internal CTOR
        /// </summary>
        /// <param name="settings">Application setting</param>
        internal ClockViewModel(IAppSettings settings)
        {
            Settings = settings;

            if (settings.ClockTimeZones != null) 
            {
                var timeZones = new List<ClockTimeZone>();
                foreach (var (name, timeZoneId) in settings.ClockTimeZones)
                {
                    try
                    {
                        var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                        var timeZone = new ClockTimeZone(timeZoneInfo, name, settings.ClockTimeZoneDateFormat, settings.ClockTimeZoneTimeFormat);
                        timeZones.Add(timeZone);

                    }
                    catch (TimeZoneNotFoundException)
                    {
                        //can't get time zone, ignore it
                    }

                }

                HasTimeZones = timeZones.Count > 0;
                TimeZones = timeZones.OrderBy(t=>t.Offset.TotalMinutes).ToArray();
            }

            timer = new DispatcherTimer(DispatcherPriority.Render)
            {
                Interval = TimeSpan.FromMilliseconds(Settings.RefreshClockIntervalMs)
            };
            timer.Tick += (_, _) => { UpdateTime(); };

            if (settings.ShowClock)
            {
                //start timer
                timer.Start();
            }
        }

        /// <summary>
        /// CTOR used by DI
        /// </summary>
        /// <param name="options">Application settings configuration</param>
        // ReSharper disable once UnusedMember.Global
        public ClockViewModel(IOptions<AppSettings> options)
            : this(options.Value)
        {
            //used from DI - DI populates the parameters and the internal CTOR is called then
        }

        /// <summary>
        /// Periodically updates the formatted date and time
        /// </summary>
        [SuppressMessage("ReSharper", "FormatStringProblem")]
        private void UpdateTime()
        {
            var dateTime = DateTime.Now;
            var df = Settings.ClockDateFormat;
            var tf = Settings.ClockTimeFormat;
            var lf = Settings.ClockLongFormat;
            if (string.IsNullOrEmpty(lf)) lf = new AppSettings().ClockLongFormat; //default

            Date = string.IsNullOrEmpty(df) ? string.Empty : string.Format("{0:" + df + "}", dateTime);
            Time = string.IsNullOrEmpty(tf) ? string.Empty : string.Format("{0:" + tf + "}", dateTime);
            DateTimeLong = string.Format("{0:" + lf + "}", dateTime);
            foreach (var timeZone in TimeZones)
            {
                timeZone.Update(dateTime);
            }
        }



        /// <summary>
        /// Dispose the <see cref="ClockViewModel"/> - stop the timer
        /// </summary>
        protected override void DoDispose()
        {
            timer.Stop();
        }
    }

   
}
