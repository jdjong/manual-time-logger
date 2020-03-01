using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ManualTimeLogger.ReportBuilder
{
    public class CommandProvider
    {
        public ICommand GetCommand(string[] args)
        {
            if (args == null || !args.Any())
            {
                return GetDefaultCommand();
            }

            if (args.Length != 2)
            {
                throw new ArgumentException("Incorrect arguments. Provide period (-w or -m) to build report for with a date (yyyyMMdd).", nameof(args));
            }

            var argumentKeyForReportingPeriod = args[0];
            var argumentValueForDate = args[1];

            var regex = new Regex(@"^20\d{2}(0[1-9]|1[012])(0[1-9]|[12][0-9]|3[01])$");
            if (!regex.IsMatch(argumentValueForDate))
            {
                throw new ArgumentException("Invalid date, should be in format yyyyMMdd", nameof(args));
            }

            var providedDate = new DateTime(Convert.ToInt32(argumentValueForDate.Substring(0, 4)), Convert.ToInt32(argumentValueForDate.Substring(4, 2)), Convert.ToInt32(argumentValueForDate.Substring(6, 2)));

            switch (argumentKeyForReportingPeriod)
            {
                case "-w":
                    return new BuildWeekReportsCommand(GetMondayDateFromDateInRequestedWeek(providedDate));
                case "-m":
                    return new BuildMonthReportsCommand(GetFirstDayOfMonthRequestedMonth(providedDate));
                default:
                    throw new ArgumentException($"Unknown command {argumentKeyForReportingPeriod}", nameof(args));
            }
        }

        private BuildWeekReportsCommand GetDefaultCommand()
        {
            return new BuildWeekReportsCommand(GetMondayDateFromDateInRequestedWeek(DateTime.Today));
        }

        private DateTime GetMondayDateFromDateInRequestedWeek(DateTime aDateForTheRequestedWeek)
        {
            var dateIterator = aDateForTheRequestedWeek;
            while (dateIterator.DayOfWeek != DayOfWeek.Monday)
            {
                dateIterator = dateIterator.AddDays(-1);
            }

            return dateIterator;
        }

        private DateTime GetFirstDayOfMonthRequestedMonth(DateTime aDateForTheRequestedMonth)
        {
            return new DateTime(aDateForTheRequestedMonth.Year, aDateForTheRequestedMonth.Month, 1);
        }
    }
}
