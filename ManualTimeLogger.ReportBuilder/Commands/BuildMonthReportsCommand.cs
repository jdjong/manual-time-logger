using System;

namespace ManualTimeLogger.ReportBuilder.Commands
{
    public class BuildMonthReportsCommand : ICommand
    {
        /// <summary>
        /// When period is month, then the first day is the first of the month.
        /// </summary>
        public DateTime FromDay { get; }

        public BuildMonthReportsCommand(DateTime firstDayOfPeriod)
        {
            if (firstDayOfPeriod.Day != 1)
            {
                throw new ArgumentException("Period is month, so first date of period should be on the first of the month", nameof(firstDayOfPeriod));
            }

            FromDay = firstDayOfPeriod;
        }
    }
}
