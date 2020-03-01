using System;

namespace ManualTimeLogger.ReportBuilder
{
    public interface ICommand
    {
        /// <summary>
        /// When period is week, then the first day is the monday of the period.
        /// </summary>
        DateTime FirstDayOfPeriod { get; }
    }
}