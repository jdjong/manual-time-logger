using System;
using System.Collections.Generic;
using System.Linq;
using ManualTimeLogger.Domain;
using ManualTimeLogger.ReportBuilder.ReportBuilders;

// ReSharper disable PossibleMultipleEnumeration

namespace ManualTimeLogger.ReportBuilder.ReportsBuilders
{
    public class PerLabelMonthReportsBuilder
    {
        private readonly Dictionary<string, IEnumerable<LogEntry>> _logEntriesPerLabel;

        private readonly ActivityReportBuilder _labelPerActivityReportBuilder;

        public PerLabelMonthReportsBuilder(string reportsBasePath, DateTime firstDayOfMonth, string accountFilter,
            Dictionary<string, IEnumerable<LogEntry>> logEntriesPerLabel)
        {
            if (firstDayOfMonth.Day != 1)
            {
                throw new ArgumentException("First day of month should be 1", nameof(firstDayOfMonth));
            }

            _logEntriesPerLabel = logEntriesPerLabel;

            var nrOfDaysInMonth = DateTime.DaysInMonth(firstDayOfMonth.Year, firstDayOfMonth.Month);
            _labelPerActivityReportBuilder = new ActivityReportBuilder(new ReportCsvFileRepository(reportsBasePath, $"{accountFilter ?? "all"}_label_activity_month_report_{firstDayOfMonth:yyyyMMdd}.csv"), firstDayOfMonth, nrOfDaysInMonth, accountFilter);
        }

        public void Build()
        {
            _logEntriesPerLabel.Keys.ToList().ForEach(label =>
            {
                var logEntriesPerActivityPerDay = _logEntriesPerLabel[label].GroupBy(x => x.CreateDate);

                // TODO, needs some refactoring, because the builder parameter name is engineer, but we pass in label. However both are strings, so therefore we abuse, but needs fixing badly!!!!!
                _labelPerActivityReportBuilder.Build(label, logEntriesPerActivityPerDay);
            });
        }
    }
}
