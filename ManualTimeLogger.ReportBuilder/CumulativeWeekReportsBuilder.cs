using System;
using System.Collections.Generic;
using System.Linq;
using ManualTimeLogger.Domain;
using ManualTimeLogger.ReportBuilder.ReportBuilders;

// ReSharper disable PossibleMultipleEnumeration

namespace ManualTimeLogger.ReportBuilder
{
    // TODO, refactor, rename, test, polish
    public class CumulativeWeekReportsBuilder
    {
        private readonly DateTime _firstDayOfWeek;
        private readonly IEnumerable<IGrouping<DateTime, LogEntry>> _logEntriesPerDay;

        private readonly ActivityReportBuilder _activityCumulativeReportBuilder;
        private readonly LabelReportBuilder _labelCumulativeReportBuilder;
        private readonly IssueNumberReportBuilder _issueNumberCumulativeReportBuilder;

        public CumulativeWeekReportsBuilder(string reportsBasePath, DateTime firstDayOfWeek, IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            if (firstDayOfWeek.DayOfWeek != DayOfWeek.Monday)
            {
                throw new ArgumentException("First day of week should be a monday", nameof(firstDayOfWeek));
            }

            _firstDayOfWeek = firstDayOfWeek;
            _logEntriesPerDay = logEntriesPerDay;

            _activityCumulativeReportBuilder = new ActivityReportBuilder(new WeekReportCsvFileRepository(reportsBasePath, $"cumulative_activity_week_report_{_firstDayOfWeek:yyyyMMdd}.csv", _firstDayOfWeek), _firstDayOfWeek);
            _labelCumulativeReportBuilder = new LabelReportBuilder(new WeekReportCsvFileRepository(reportsBasePath, $"cumulative_label_week_report_{_firstDayOfWeek:yyyyMMdd}.csv", _firstDayOfWeek), _firstDayOfWeek);
            _issueNumberCumulativeReportBuilder = new IssueNumberReportBuilder(new WeekReportCsvFileRepository(reportsBasePath, $"cumulative_issue_week_report_{_firstDayOfWeek:yyyyMMdd}.csv", _firstDayOfWeek), _firstDayOfWeek);
        }

        public void Build()
        {
            _activityCumulativeReportBuilder.Build(_logEntriesPerDay);
            _labelCumulativeReportBuilder.Build(_logEntriesPerDay);
            _issueNumberCumulativeReportBuilder.Build(_logEntriesPerDay);
        }
    }
}
