using System;
using System.Collections.Generic;
using System.Linq;
using ManualTimeLogger.Domain;
using ManualTimeLogger.ReportBuilder.ReportBuilders;

// ReSharper disable PossibleMultipleEnumeration

namespace ManualTimeLogger.ReportBuilder
{
    public class PerLabelWeekReportsBuilder
    {
        private readonly Dictionary<string, IEnumerable<LogEntry>> _logEntriesPerLabel;

        private readonly ActivityReportBuilder _labelPerActivityReportBuilder;

        public PerLabelWeekReportsBuilder(string reportsBasePath, DateTime firstDayOfWeek, string accountFilter, Dictionary<string, IEnumerable<LogEntry>> logEntriesPerLabel)
        {
            if (firstDayOfWeek.DayOfWeek != DayOfWeek.Monday)
            {
                throw new ArgumentException("First day of week should be a monday", nameof(firstDayOfWeek));
            }

            _logEntriesPerLabel = logEntriesPerLabel;

            var nrOfDaysInWeek = 7;
            _labelPerActivityReportBuilder = new ActivityReportBuilder(new ReportCsvFileRepository(reportsBasePath, $"{accountFilter ?? "all"}_label_activity_week_report_{firstDayOfWeek:yyyyMMdd}.csv"), firstDayOfWeek, nrOfDaysInWeek, accountFilter);
        }

        public void Build()
        {
            _logEntriesPerLabel.Keys.ToList().ForEach(label =>
            {
                var logEntriesPerLabelPerDay = _logEntriesPerLabel[label].GroupBy(x => x.CreateDate);

                _labelPerActivityReportBuilder.Build(label, logEntriesPerLabelPerDay);
            });
        }
    }
}
