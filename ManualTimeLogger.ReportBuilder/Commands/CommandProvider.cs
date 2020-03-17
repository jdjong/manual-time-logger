using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ManualTimeLogger.ReportBuilder.Commands
{
    public class CommandProvider
    {
        public ICommand GetCommand(string[] args)
        {
            if (args == null || !args.Any())
            {
                return GetDefaultCommand();
            }

            switch (args.Length)
            {
                // ....exe -m 20200229
                case 2:
                {
                    var argumentKeyForReportingPeriod = args[0];
                    var argumentValueForDate = args[1];

                    return BuildCommandWithoutAccountFilter(argumentValueForDate, argumentKeyForReportingPeriod);
                }

                // ....exe -m 20200229 -c nwb
                // or
                // ....exe -c nwb -m 20200229
                case 4:
                {
                    var argumentKeyForReportingPeriod = args[0] != "-c" ? args[0] : args[2];
                    var argumentValueForDate = args[0] != "-c" ? args[1] : args[3];
                    var accountFilter = args[0] != "-c" ? args[3] : args[1];

                    return BuildCommandWithAccountFilter(argumentValueForDate, argumentKeyForReportingPeriod, accountFilter);
                }
                default:
                    throw new ArgumentException("Incorrect arguments. Provide period (-w or -m) to build report for with a date (yyyyMMdd). Provide customer filter optionally (-c somecustomer)", nameof(args));
            }
        }

        private ICommand BuildCommandWithAccountFilter(string argumentValueForDate, string argumentKeyForReportingPeriod, string accountFilter)
        {
            var command = BuildCommandWithoutAccountFilter(argumentValueForDate, argumentKeyForReportingPeriod);
            command.AccountFilter = accountFilter;
            return command;
        }

        private ICommand BuildCommandWithoutAccountFilter(string argumentValueForDate, string argumentKeyForReportingPeriod)
        {
            var regex = new Regex(@"^20\d{2}(0[1-9]|1[012])(0[1-9]|[12][0-9]|3[01])$");
            if (!regex.IsMatch(argumentValueForDate))
            {
                throw new ArgumentException("Invalid date, should be in format yyyyMMdd", nameof(argumentValueForDate));
            }

            var providedDate = new DateTime(Convert.ToInt32(argumentValueForDate.Substring(0, 4)),
                Convert.ToInt32(argumentValueForDate.Substring(4, 2)), Convert.ToInt32(argumentValueForDate.Substring(6, 2)));

            switch (argumentKeyForReportingPeriod)
            {
                case "-w":
                    return new BuildWeekReportsCommand(GetMondayDateFromDateInRequestedWeek(providedDate));
                case "-m":
                    return new BuildMonthReportsCommand(GetFirstDayOfMonthRequestedMonth(providedDate));
                default:
                    throw new ArgumentException($"Unknown command {argumentKeyForReportingPeriod}",
                        nameof(argumentKeyForReportingPeriod));
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
