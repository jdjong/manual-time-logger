using System;
using System.Linq;
using System.Text.RegularExpressions;

// TODO, unit test
namespace ManualTimeLogger.ReportBuilder
{
    public class CommandProvider
    {
        public ReportBuilderCommand GetCommand(string[] args)
        {
            if (!args.Any())
            {
                return new ReportBuilderCommand(ReportPeriod.Week, GetMondayDateFromDateInRequestedWeek(DateTime.Today));
            }

            if (args.Length != 2)
            {
                throw new ArgumentException("Expecting 2 arguments", nameof(args));
            }

            var argumentKeyForReportingPeriod = args[0];
            var argumentValueForDate = args[1];

            if (argumentKeyForReportingPeriod != "-w" && argumentKeyForReportingPeriod != "-m")
            {
                throw new ArgumentException("Incorrect argument key", nameof(args));
            }

            var regex = new Regex(@"^20\d{2}(0[1-9]|1[012])(0[1-9]|[12][0-9]|3[01])$");
            if (!regex.IsMatch(argumentValueForDate))
            {
                throw new ArgumentException("Invalid date, should be in format yyyyMMdd", nameof(args));
            }

            var providedDate = new DateTime(Convert.ToInt32(argumentValueForDate.Substring(0, 4)), Convert.ToInt32(argumentValueForDate.Substring(4, 2)), Convert.ToInt32(argumentValueForDate.Substring(6, 2)));
            return new ReportBuilderCommand(argumentKeyForReportingPeriod == "-m" ? ReportPeriod.Month : ReportPeriod.Week, GetMondayDateFromDateInRequestedWeek(providedDate));

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
    }
}
