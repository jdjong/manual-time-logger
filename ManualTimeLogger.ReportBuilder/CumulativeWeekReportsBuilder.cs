﻿using System;
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
        private readonly IssueNumberReportBuilder _issueNumberCumulativeReportBuilder;

        public CumulativeWeekReportsBuilder(string reportsBasePath, DateTime firstDayOfWeek, IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            if (firstDayOfWeek.DayOfWeek != DayOfWeek.Monday)
            {
                throw new ArgumentException("First day of week should be a monday", nameof(firstDayOfWeek));
            }

            _logEntriesPerDay = logEntriesPerDay;

            var nrOfDaysInWeek = 7;
            _activityCumulativeReportBuilder = new ActivityReportBuilder(new ReportCsvFileRepository(reportsBasePath, $"cumulative_activity_week_report_{firstDayOfWeek:yyyyMMdd}.csv"), firstDayOfWeek, nrOfDaysInWeek);
            _labelCumulativeReportBuilder = new LabelReportBuilder(new ReportCsvFileRepository(reportsBasePath, $"cumulative_label_week_report_{firstDayOfWeek:yyyyMMdd}.csv"), firstDayOfWeek, nrOfDaysInWeek);
            _issueNumberCumulativeReportBuilder = new IssueNumberReportBuilder(new ReportCsvFileRepository(reportsBasePath, $"cumulative_issue_week_report_{firstDayOfWeek:yyyyMMdd}.csv"), firstDayOfWeek, nrOfDaysInWeek);
        }

        public void Build()
        {
            _activityCumulativeReportBuilder.Build(_logEntriesPerDay);
            _labelCumulativeReportBuilder.Build(_logEntriesPerDay);
            _issueNumberCumulativeReportBuilder.Build(_logEntriesPerDay);
        }
    }
}
