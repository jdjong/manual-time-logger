using System;
using System.Collections.Generic;
using System.Linq;
using ManualTimeLogger.Domain;
using ManualTimeLogger.ReportBuilder.ReportBuilders;

// ReSharper disable PossibleMultipleEnumeration

namespace ManualTimeLogger.ReportBuilder
{
    public class CumulativeWeekReportsBuilder
    {
        private readonly IEnumerable<IGrouping<DateTime, LogEntry>> _logEntriesPerDay;

        private readonly ActivityReportBuilder _activityCumulativeReportBuilder;
        private readonly LabelReportBuilder _labelCumulativeReportBuilder;

        public CumulativeWeekReportsBuilder(string reportsBasePath, DateTime firstDayOfWeek, string accountFilter,
            IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            if (firstDayOfWeek.DayOfWeek != DayOfWeek.Monday)
            {
                throw new ArgumentException("First day of week should be a monday", nameof(firstDayOfWeek));
            }

            _logEntriesPerDay = logEntriesPerDay;

            var nrOfDaysInWeek = 7;
            _activityCumulativeReportBuilder = new ActivityReportBuilder(new ReportCsvFileRepository(reportsBasePath, $"{accountFilter ?? "all"}_cumulative_activity_week_report_{firstDayOfWeek:yyyyMMdd}.csv"), firstDayOfWeek, nrOfDaysInWeek, accountFilter);
            _labelCumulativeReportBuilder = new LabelReportBuilder(new ReportCsvFileRepository(reportsBasePath, $"{accountFilter ?? "all"}_cumulative_label_week_report_{firstDayOfWeek:yyyyMMdd}.csv"), firstDayOfWeek, nrOfDaysInWeek, accountFilter);
        }

        public void Build()
        {
            _activityCumulativeReportBuilder.Build(_logEntriesPerDay);
            _labelCumulativeReportBuilder.Build(_logEntriesPerDay);
        }
    }
}
