using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ManualTimeLogger.Domain;
using ManualTimeLogger.Persistence;
// ReSharper disable PossibleMultipleEnumeration

namespace ManualTimeLogger.ReportBuilder
{
    // TODO, refactor, rename, test, polish
    public class WeekReportsBuilder
    {
        private readonly string _timeLogsBasePath;
        private readonly DateTime _firstDayOfWeek;
        private readonly WeekReportCsvFileRepository _activityPerEngineerReportRepository;
        private readonly WeekReportCsvFileRepository _labelPerEngineerReportRepository;
        private readonly WeekReportCsvFileRepository _issueNumberPerEngineerReportRepository;
        private readonly WeekReportCsvFileRepository _activityCumulativeReportRepository;
        private readonly WeekReportCsvFileRepository _labelCumulativeReportRepository;
        private readonly WeekReportCsvFileRepository _issueNumberCumulativeReportRepository;

        public WeekReportsBuilder(string timeLogsBasePath, string reportsBasePath, DateTime firstDayOfWeek)
        {
            if (firstDayOfWeek.DayOfWeek != DayOfWeek.Monday)
            {
                throw new ArgumentException("First day of week should be a monday", nameof(firstDayOfWeek));
            }

            _timeLogsBasePath = timeLogsBasePath;
            _firstDayOfWeek = firstDayOfWeek;

            _activityPerEngineerReportRepository = new WeekReportCsvFileRepository(reportsBasePath, $"engineer_activity_week_report_{_firstDayOfWeek:yyyyMMdd}.csv", _firstDayOfWeek);
            _labelPerEngineerReportRepository = new WeekReportCsvFileRepository(reportsBasePath, $"engineer_label_week_report_{_firstDayOfWeek:yyyyMMdd}.csv", _firstDayOfWeek);
            _issueNumberPerEngineerReportRepository = new WeekReportCsvFileRepository(reportsBasePath, $"engineer_issue_week_report_{_firstDayOfWeek:yyyyMMdd}.csv", _firstDayOfWeek);

            _activityCumulativeReportRepository = new WeekReportCsvFileRepository(reportsBasePath, $"cumulative_activity_week_report_{_firstDayOfWeek:yyyyMMdd}.csv", _firstDayOfWeek);
            _labelCumulativeReportRepository = new WeekReportCsvFileRepository(reportsBasePath, $"cumulative_label_week_report_{_firstDayOfWeek:yyyyMMdd}.csv", _firstDayOfWeek);
            _issueNumberCumulativeReportRepository = new WeekReportCsvFileRepository(reportsBasePath, $"cumulative_issue_week_report_{_firstDayOfWeek:yyyyMMdd}.csv", _firstDayOfWeek);
        }

        public void Build()
        {
            var allTimeLogFileNames = Directory.EnumerateFiles(_timeLogsBasePath);
            var allTimeLogRepositories = allTimeLogFileNames.Select(filePaths => new CsvFileRepository(_timeLogsBasePath, Path.GetFileName(filePaths))).ToList();

            var logEntriesPerEngineer = allTimeLogRepositories.ToDictionary(repository => repository.GetEngineerName(), repository => repository.GetAllLogEntries());
            var logEntriesPerDay = logEntriesPerEngineer.SelectMany(x => x.Value).GroupBy(x => x.CreateDate);

            GenerateCumulativePerEngineerReport(logEntriesPerEngineer);
            GenerateCumulativeOverallReport(null, logEntriesPerDay);
        }

        private void GenerateCumulativePerEngineerReport(Dictionary<string, IEnumerable<LogEntry>> logEntriesPerEngineer)
        {
            logEntriesPerEngineer.Keys.ToList().ForEach(engineer =>
            {
                var logEntriesPerEngineerPerDay = logEntriesPerEngineer[engineer].GroupBy(x => x.CreateDate);
                GenerateEngineerReport(engineer, logEntriesPerEngineerPerDay);
            });
        }

        private void GenerateEngineerReport(string engineer, IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            var differentActivities = logEntriesPerDay.SelectMany(x => x).Select(x => x.Activity).Distinct();
            var differentLabels = logEntriesPerDay.SelectMany(x => x).Select(x => x.Label).Distinct();
            var differentIssueNumbers = logEntriesPerDay.SelectMany(x => x).Select(x => x.IssueNumber).Distinct();

            differentActivities.ToList().ForEach(activity =>
            {
                var timeForActivityPerDay = logEntriesPerDay.ToDictionary(x => x.Key, x => x.Where(y => y.Activity == activity).Sum(y => y.Duration));
                _activityPerEngineerReportRepository.SaveReportEntry(new WeekReportEntry(engineer, activity, _firstDayOfWeek, timeForActivityPerDay));
            });

            differentLabels.ToList().ForEach(label =>
            {
                var timeForLabelPerDay = logEntriesPerDay.ToDictionary(x => x.Key, x => x.Where(y => y.Label == label).Sum(y => y.Duration));
                _labelPerEngineerReportRepository.SaveReportEntry(new WeekReportEntry(engineer, label, _firstDayOfWeek, timeForLabelPerDay));
            });

            differentIssueNumbers.ToList().ForEach(issueNumber =>
            {
                var timeForIssueNumberPerDay = logEntriesPerDay.ToDictionary(x => x.Key, x => x.Where(y => y.IssueNumber == issueNumber).Sum(y => y.Duration));
                _issueNumberPerEngineerReportRepository.SaveReportEntry(new WeekReportEntry(engineer, issueNumber.ToString(), _firstDayOfWeek, timeForIssueNumberPerDay));
            });
        }

        private void GenerateCumulativeOverallReport(string engineer, IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            GenerateCumulativeReport(engineer, logEntriesPerDay);
        }

        private void GenerateCumulativeReport(string engineer, IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            var differentActivities = logEntriesPerDay.SelectMany(x => x).Select(x => x.Activity).Distinct();
            var differentLabels = logEntriesPerDay.SelectMany(x => x).Select(x => x.Label).Distinct();
            var differentIssueNumbers = logEntriesPerDay.SelectMany(x => x).Select(x => x.IssueNumber).Distinct();

            differentActivities.ToList().ForEach(activity =>
            {
                var timeForActivityPerDay = logEntriesPerDay.ToDictionary(x => x.Key, x => x.Where(y => y.Activity == activity).Sum(y => y.Duration));
                _activityCumulativeReportRepository.SaveReportEntry(new WeekReportEntry(engineer, activity, _firstDayOfWeek, timeForActivityPerDay));
            });

            differentLabels.ToList().ForEach(label =>
            {
                var timeForLabelPerDay = logEntriesPerDay.ToDictionary(x => x.Key, x => x.Where(y => y.Label == label).Sum(y => y.Duration));
                _labelCumulativeReportRepository.SaveReportEntry(new WeekReportEntry(engineer, label, _firstDayOfWeek, timeForLabelPerDay));
            });

            differentIssueNumbers.ToList().ForEach(issueNumber =>
            {
                var timeForIssueNumberPerDay = logEntriesPerDay.ToDictionary(x => x.Key, x => x.Where(y => y.IssueNumber == issueNumber).Sum(y => y.Duration));
                _issueNumberCumulativeReportRepository.SaveReportEntry(new WeekReportEntry(engineer, issueNumber.ToString(), _firstDayOfWeek, timeForIssueNumberPerDay));
            });
        }
    }
}
