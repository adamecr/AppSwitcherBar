using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace net.adamec.ui.AppSwitcherBar.Dto
{
    /// <summary>
    /// Time zone time information
    /// </summary>
    public class ClockTimeZone : INotifyPropertyChanged
    {
        /// <summary>
        /// Time zone
        /// </summary>
        private readonly TimeZoneInfo timeZoneInfo;
        /// <summary>
        /// Date presentation format
        /// </summary>
        private readonly string dateFormat;
        /// <summary>
        /// Time presentation format
        /// </summary>
        private readonly string timeFormat;

        /// <summary>
        /// Name of the time zone item (as defined in settings)
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Information about delta (hours:mins) comparing to the local time
        /// </summary>
        private string delta;
        /// <summary>
        /// Information about delta (hours:mins) comparing to the local time
        /// </summary>
        public string Delta
        {
            get => delta;
            set
            {
                if (value == delta) return;
                delta = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Time zone offset to local time
        /// </summary>
        private TimeSpan offset;
        /// <summary>
        /// Time zone offset to local time
        /// </summary>
        public TimeSpan Offset
        {
            get => offset;
            private set
            {
                if (value == offset) return;
                offset = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Formatted time zone date
        /// </summary>
        private string date;
        /// <summary>
        /// Formatted time zone date
        /// </summary>
        public string Date
        {
            get => date;
            private set
            {
                if (value == date) return;
                date = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Formatted time zone time
        /// </summary>
        private string time;
        /// <summary>
        /// Formatted time zone time
        /// </summary>
        public string Time
        {
            get => time;
            private set
            {
                if (value == time) return;
                time = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="timeZoneInfo">Time zone</param>
        /// <param name="name">Name of the time zone item (as defined in settings)</param>
        /// <param name="dateFormat">Date presentation format</param>
        /// <param name="timeFormat">Time presentation format</param>
#pragma warning disable CS8618
        public ClockTimeZone(TimeZoneInfo timeZoneInfo, string name, string? dateFormat, string? timeFormat)
#pragma warning restore CS8618
        {
            Name = name;
            this.timeZoneInfo = timeZoneInfo;
            this.dateFormat = !string.IsNullOrEmpty(dateFormat) ? dateFormat : "ddd d.M.";
            this.timeFormat = !string.IsNullOrEmpty(timeFormat) ? timeFormat : "HH:mm";

            Update(DateTime.Now);
        }

        /// <summary>
        /// Update the time zone date and time from (current) local time
        /// </summary>
        /// <param name="dtLocal">Local time</param>
        public void Update(DateTime dtLocal)
        {
            var utc = TimeZoneInfo.ConvertTimeToUtc(dtLocal, TimeZoneInfo.Local);
            var dtTimeZone = TimeZoneInfo.ConvertTimeFromUtc(utc, timeZoneInfo);

            Offset = dtTimeZone - dtLocal;
            Delta = $"({Offset.Hours:+0;-#}:{Offset.Minutes:0#})";

            Date = string.Format("{0:" + dateFormat + "}", dtTimeZone);
            Time = string.Format("{0:" + timeFormat + "}", dtTimeZone);
        }

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
