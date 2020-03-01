using System;
using System.Collections.Generic;
using System.Linq;
using ManualTimeLogger.Domain;
using ManualTimeLogger.ReportBuilder.ReportBuilders;

// ReSharper disable PossibleMultipleEnumeration

namespace ManualTimeLogger.ReportBuilder
{
    public class PerEngineerWeekReportsBuilder
    {
        private readonly Dictionary<string, IEnumerable<LogEntry>> _logEntriesPerEngineer;

        private readonly ActivityReportBuilder _activityPerEngineerReportBuilder;
        private readonly LabelReportBuilder _labelPerEngineerReportBuilder;
        private readonly IssueNumberReportBuilder _issueNumberPerEngineerReportBuilder;

        public PerEngineerWeekReportsBuilder(string reportsBasePath, DateTime firstDayOfWeek, Dictionary<string, IEnumerable<LogEntry>> logEntriesPerEngineer)
        {
            if (firstDayOfWeek.DayOfWeek != DayOfWeek.Monday)
            {
                throw new ArgumentException("First day of week should be a monday", nameof(firstDayOfWeek));
            }

            _logEntriesPerEngineer = logEntriesPerEngineer;

            _activityPerEngineerReportBuilder = new ActivityReportBuilder(new WeekReportCsvFileRepository(reportsBasePath, $"engineer_activity_week_report_{firstDayOfWeek:yyyyMMdd}.csv", firstDayOfWeek), firstDayOfWeek);
            _labelPerEngineerReportBuilder = new LabelReportBuilder(new WeekReportCsvFileRepository(reportsBasePath, $"engineer_label_week_report_{firstDayOfWeek:yyyyMMdd}.csv", firstDayOfWeek), firstDayOfWeek);
            _issueNumberPerEngineerReportBuilder = new IssueNumberReportBuilder(new WeekReportCsvFileRepository(reportsBasePath, $"engineer_issue_week_report_{firstDayOfWeek:yyyyMMdd}.csv", firstDayOfWeek), firstDayOfWeek);
        }

        public void Build()
        {
            _logEntriesPerEngineer.Keys.ToList().ForEach(engineer =>
            {
                var logEntriesPerEngineerPerDay = _logEntriesPerEngineer[engineer].GroupBy(x => x.CreateDate);

                _activityPerEngineerReportBuilder.Build(engineer, logEntriesPerEngineerPerDay);
                _labelPerEngineerReportBuilder.Build(engineer, logEntriesPerEngineerPerDay);
                _issueNumberPerEngineerReportBuilder.Build(engineer, logEntriesPerEngineerPerDay);
            });
        }
    }
}
