using System;
using System.Collections.Generic;
using System.Linq;
using ManualTimeLogger.Domain;
using ManualTimeLogger.ReportBuilder.ReportBuilders;

// ReSharper disable PossibleMultipleEnumeration

namespace ManualTimeLogger.ReportBuilder
{
    public class PerEngineerMonthReportsBuilder
    {
        private readonly Dictionary<string, IEnumerable<LogEntry>> _logEntriesPerEngineer;

        private readonly ActivityReportBuilder _activityPerEngineerReportBuilder;
        private readonly LabelReportBuilder _labelPerEngineerReportBuilder;
        private readonly IssueNumberReportBuilder _issueNumberPerEngineerReportBuilder;

        public PerEngineerMonthReportsBuilder(string reportsBasePath, DateTime firstDayOfMonth, string accountFilter,
            Dictionary<string, IEnumerable<LogEntry>> logEntriesPerEngineer)
        {
            if (firstDayOfMonth.Day != 1)
            {
                throw new ArgumentException("First day of month should be 1", nameof(firstDayOfMonth));
            }

            _logEntriesPerEngineer = logEntriesPerEngineer;

            var nrOfDaysInMonth = DateTime.DaysInMonth(firstDayOfMonth.Year, firstDayOfMonth.Month);
            _activityPerEngineerReportBuilder = new ActivityReportBuilder(new ReportCsvFileRepository(reportsBasePath, $"{accountFilter ?? "all"}_engineer_activity_month_report_{firstDayOfMonth:yyyyMMdd}.csv"), firstDayOfMonth, nrOfDaysInMonth, accountFilter);
            _labelPerEngineerReportBuilder = new LabelReportBuilder(new ReportCsvFileRepository(reportsBasePath, $"{accountFilter ?? "all"}_engineer_label_month_report_{firstDayOfMonth:yyyyMMdd}.csv"), firstDayOfMonth, nrOfDaysInMonth, accountFilter);
            _issueNumberPerEngineerReportBuilder = new IssueNumberReportBuilder(new ReportCsvFileRepository(reportsBasePath, $"{accountFilter ?? "all"}_engineer_issue_month_report_{firstDayOfMonth:yyyyMMdd}.csv"), firstDayOfMonth, nrOfDaysInMonth, accountFilter);
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
