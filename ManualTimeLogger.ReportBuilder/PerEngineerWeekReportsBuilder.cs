using System;
using System.Collections.Generic;
using System.Linq;
using ManualTimeLogger.Domain;
using ManualTimeLogger.ReportBuilder.ReportBuilders;

// ReSharper disable PossibleMultipleEnumeration

namespace ManualTimeLogger.ReportBuilder
{
    // TODO, refactor, rename, test, polish
    public class PerEngineerWeekReportsBuilder
    {
        private readonly DateTime _firstDayOfWeek;
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

            _firstDayOfWeek = firstDayOfWeek;
            _logEntriesPerEngineer = logEntriesPerEngineer;

            _activityPerEngineerReportBuilder = new ActivityReportBuilder(new WeekReportCsvFileRepository(reportsBasePath, $"engineer_activity_week_report_{_firstDayOfWeek:yyyyMMdd}.csv", _firstDayOfWeek), _firstDayOfWeek);
            _labelPerEngineerReportBuilder = new LabelReportBuilder(new WeekReportCsvFileRepository(reportsBasePath, $"engineer_label_week_report_{_firstDayOfWeek:yyyyMMdd}.csv", _firstDayOfWeek), _firstDayOfWeek);
            _issueNumberPerEngineerReportBuilder = new IssueNumberReportBuilder(new WeekReportCsvFileRepository(reportsBasePath, $"engineer_issue_week_report_{_firstDayOfWeek:yyyyMMdd}.csv", _firstDayOfWeek), _firstDayOfWeek);
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
