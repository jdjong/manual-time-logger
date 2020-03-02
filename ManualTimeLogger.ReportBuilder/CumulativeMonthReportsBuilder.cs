using System;
using System.Collections.Generic;
using System.Linq;
using ManualTimeLogger.Domain;
using ManualTimeLogger.ReportBuilder.ReportBuilders;

// ReSharper disable PossibleMultipleEnumeration

namespace ManualTimeLogger.ReportBuilder
{
    public class CumulativeMonthReportsBuilder
    {
        private readonly IEnumerable<IGrouping<DateTime, LogEntry>> _logEntriesPerDay;

        private readonly ActivityReportBuilder _activityCumulativeReportBuilder;
        private readonly LabelReportBuilder _labelCumulativeReportBuilder;
        private readonly IssueNumberReportBuilder _issueNumberCumulativeReportBuilder;

        public CumulativeMonthReportsBuilder(string reportsBasePath, DateTime firstDayOfMonth, IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            if (firstDayOfMonth.Day != 1)
            {
                throw new ArgumentException("First day of month should be 1", nameof(firstDayOfMonth));
            }

            _logEntriesPerDay = logEntriesPerDay;

            var nrOfDaysInMonth = DateTime.DaysInMonth(firstDayOfMonth.Year, firstDayOfMonth.Month);
            _activityCumulativeReportBuilder = new ActivityReportBuilder(new ReportCsvFileRepository(reportsBasePath, $"cumulative_activity_month_report_{firstDayOfMonth:yyyyMMdd}.csv"), firstDayOfMonth, nrOfDaysInMonth);
            _labelCumulativeReportBuilder = new LabelReportBuilder(new ReportCsvFileRepository(reportsBasePath, $"cumulative_label_month_report_{firstDayOfMonth:yyyyMMdd}.csv"), firstDayOfMonth, nrOfDaysInMonth);
            _issueNumberCumulativeReportBuilder = new IssueNumberReportBuilder(new ReportCsvFileRepository(reportsBasePath, $"cumulative_issue_month_report_{firstDayOfMonth:yyyyMMdd}.csv"), firstDayOfMonth, nrOfDaysInMonth);
        }

        public void Build()
        {
            _activityCumulativeReportBuilder.Build(_logEntriesPerDay);
            _labelCumulativeReportBuilder.Build(_logEntriesPerDay);
            _issueNumberCumulativeReportBuilder.Build(_logEntriesPerDay);
        }
    }
}
