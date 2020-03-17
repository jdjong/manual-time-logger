using System;

namespace ManualTimeLogger.ReportBuilder.Commands
{
    public class BuildWeekReportsCommand : ICommand
    {
        /// <summary>
        /// When period is week, then the first day is the monday of the period.
        /// </summary>
        public DateTime FromDay { get; }

        public string AccountFilter { get; set; }

        public BuildWeekReportsCommand(DateTime fromDay)
        {
            if (fromDay.DayOfWeek != DayOfWeek.Monday)
            {
                throw new ArgumentException("Period is week, so first date of period should be a monday", nameof(fromDay));
            }

            FromDay = fromDay;
        }
    }
}
