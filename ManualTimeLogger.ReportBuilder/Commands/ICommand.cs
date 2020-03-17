using System;

namespace ManualTimeLogger.ReportBuilder.Commands
{
    public interface ICommand
    {
        /// <summary>
        /// When period is week, then the first day is the monday of the period.
        /// </summary>
        DateTime FromDay { get; }

        string AccountFilter { get; set; }
    }
}