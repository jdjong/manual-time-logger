using System;

namespace ManualTimeLogger.ReportBuilder
{
    public class ReportBuilderCommand
    {
        public ReportPeriod Period { get; }
        /// <summary>
        /// When period is week, then the first day is the monday of the period.
        /// When period is month, then the first day is the first of the month.
        /// </summary>
        public DateTime FirstDayOfPeriod { get; }

        public ReportBuilderCommand(ReportPeriod period, DateTime firstDayOfPeriod)
        {
            // TODO, add validation and test for period in combination with first day.
            Period = period;
            FirstDayOfPeriod = firstDayOfPeriod;
        }
    }
}
