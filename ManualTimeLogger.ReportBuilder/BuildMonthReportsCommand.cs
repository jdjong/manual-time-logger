using System;

namespace ManualTimeLogger.ReportBuilder
{
    public class BuildMonthReportsCommand : ICommand
    {
        /// <summary>
        /// When period is month, then the first day is the first of the month.
        /// </summary>
        public DateTime FirstDayOfPeriod { get; }

        public BuildMonthReportsCommand(DateTime firstDayOfPeriod)
        {
            if (firstDayOfPeriod.Day != 1)
            {
                throw new ArgumentException("Period is month, so first date of period should be on the first of the month", nameof(firstDayOfPeriod));
            }

            FirstDayOfPeriod = firstDayOfPeriod;
        }
    }
}
