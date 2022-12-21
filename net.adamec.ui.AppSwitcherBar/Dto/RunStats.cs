using System;

namespace net.adamec.ui.AppSwitcherBar.Dto
{
    /// <summary>
    /// Application run statistics 
    /// </summary>
    public class RunStats
    {

        /// <summary>
        /// Info when the application window was set to foreground the last time
        /// </summary>
        public DateTime LastForegroundDateTime { get; private set; } = DateTime.MinValue;

        /// <summary>
        /// Last run time or <see cref="DateTime.MinValue"/> if not available
        /// </summary>
        public DateTime LastRunDateTime { get; private set; } = DateTime.MinValue;

        /// <summary>
        /// Number of runs or 0 if not available
        /// </summary>
        public int RunCount { get; private set; }

        /// <summary>
        /// CTOR
        /// </summary>
        public RunStats()
        {
            
        }

        /// <summary>
        /// Updates statistics run info from persistent store
        /// </summary>
        /// <param name="lastRunDateTime">Last run time or <see cref="DateTime.MinValue"/> if not available</param>
        /// <param name="runCount">Number of runs or 0 if not available</param>
        public RunStats UpdateRunInfo(DateTime lastRunDateTime, int runCount)
        {
            LastRunDateTime = lastRunDateTime;
            RunCount = runCount;

            return this;
        }

        /// <summary>
        /// Updates statistics run info from persistent store
        /// </summary>
        /// <param name="source">Source information</param>
        public RunStats UpdateRunInfo(RunStats? source)
        {
            if (source == null) return this;

            LastRunDateTime = source.LastRunDateTime;
            RunCount = source.RunCount;

            return this;
        }

        /// <summary>
        /// Updates statistics after the application has been launched
        /// </summary>
        public RunStats UpdateLaunched()
        {
            RunCount++;

            var lastRunDateTime = DateTime.Now;
            if (lastRunDateTime > LastRunDateTime) LastRunDateTime = lastRunDateTime;

            return this;
        }

        /// <summary>
        /// Updates statistics after the application/window has been set as foreground window
        /// </summary>
        public RunStats UpdateForeground()
        {
            var lastForegroundDateTime = DateTime.Now;
            if (lastForegroundDateTime > LastForegroundDateTime) LastForegroundDateTime = lastForegroundDateTime;

            return this;
        }

        /// <summary>
        /// Builds the standard search sort key for windows and applications from run statistics and given <paramref name="lastModifiedDateTime"/>
        /// </summary>
        /// <param name="lastModifiedDateTime">Last modification date if available otherwise <see cref="DateTime.MinValue"/> (always for windows)</param>
        /// <returns>The search sort key for window or application keeping the run statistics</returns>
        public string BuildStandardSearchSortKey(DateTime lastModifiedDateTime)
        {
            var runs = RunCount;
            if (runs < 0) runs = 0;
            if (runs > 999) runs = 999;

            var sortKey = $"{LastForegroundDateTime:yyyyMMddHHmmss}{runs:###}{LastRunDateTime:yyyyMMddHHmm}{lastModifiedDateTime:yyyyMMddHH}";
            return sortKey;
        }
    }
}
