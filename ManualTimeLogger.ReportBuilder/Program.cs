using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ManualTimeLogger.Domain;
using ManualTimeLogger.Persistence;
// ReSharper disable PossibleMultipleEnumeration

namespace ManualTimeLogger.ReportBuilder
{
    class Program
    {
        private static ReportWeekCsvFileRepository _reportActivityRepository;
        private static ReportWeekCsvFileRepository _reportLabelRepository;
        private static ReportWeekCsvFileRepository _reportIssueNumberRepository;
        private static DateTime _generateForWeekWhichContainsDate;
        private static string _reportsBasePath;

        static void Main(string[] args)
        {
            _generateForWeekWhichContainsDate = DateTime.Today;
            
            // TODO, make configurable
            var timeLogBasePath = @"C:\temp\timelogs";
            _reportsBasePath = @"C:\temp\timelogs\reports";

            var allTimeLogFileNames = Directory.EnumerateFiles(timeLogBasePath);
            var allTimeLogRepositories = allTimeLogFileNames.Select(filePaths => new CsvFileRepository(timeLogBasePath, Path.GetFileName(filePaths))).ToList();

            var logEntriesPerEngineer = allTimeLogRepositories.ToDictionary(repository => repository.GetEngineerName(), repository => repository.GetAllLogEntries());
            var logEntriesPerDay = logEntriesPerEngineer.SelectMany(x => x.Value).GroupBy(x => x.CreateDate);

            // TODO, refactor, rename, test, polish
            GenerateCumulativePerEngineerReport(logEntriesPerEngineer);
            GenerateCumulativeOverallReport(null, logEntriesPerDay);
        }

        private static void GenerateCumulativePerEngineerReport(Dictionary<string, IEnumerable<LogEntry>> logEntriesPerEngineer)
        {
            _reportActivityRepository = new ReportWeekCsvFileRepository(_reportsBasePath, $"engineer_activity_week_report_{_generateForWeekWhichContainsDate:yyyyMMdd}.csv", _generateForWeekWhichContainsDate);
            _reportLabelRepository = new ReportWeekCsvFileRepository(_reportsBasePath, $"engineer_label_week_report_{_generateForWeekWhichContainsDate:yyyyMMdd}.csv", _generateForWeekWhichContainsDate);
            _reportIssueNumberRepository = new ReportWeekCsvFileRepository(_reportsBasePath, $"engineer_issue_week_report_{_generateForWeekWhichContainsDate:yyyyMMdd}.csv", _generateForWeekWhichContainsDate);
            
            logEntriesPerEngineer.Keys.ToList().ForEach(engineer =>
            {
                var logEntriesPerEngineerPerDay = logEntriesPerEngineer[engineer].GroupBy(x => x.CreateDate);
                GenerateReport(engineer, logEntriesPerEngineerPerDay);
            });
        }

        private static void GenerateCumulativeOverallReport(string engineer, IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            _reportActivityRepository = new ReportWeekCsvFileRepository(_reportsBasePath, $"cumulative_activity_week_report_{_generateForWeekWhichContainsDate:yyyyMMdd}.csv", _generateForWeekWhichContainsDate);
            _reportLabelRepository = new ReportWeekCsvFileRepository(_reportsBasePath, $"cumulative_label_week_report_{_generateForWeekWhichContainsDate:yyyyMMdd}.csv", _generateForWeekWhichContainsDate);
            _reportIssueNumberRepository = new ReportWeekCsvFileRepository(_reportsBasePath, $"cumulative_issue_week_report_{_generateForWeekWhichContainsDate:yyyyMMdd}.csv", _generateForWeekWhichContainsDate);
            
            GenerateReport(engineer, logEntriesPerDay);
        }

        private static void GenerateReport(string engineer, IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            var differentActivities = logEntriesPerDay.SelectMany(x => x).Select(x => x.Activity).Distinct();
            var differentLabels = logEntriesPerDay.SelectMany(x => x).Select(x => x.Label).Distinct();
            var differentIssueNumbers = logEntriesPerDay.SelectMany(x => x).Select(x => x.IssueNumber).Distinct();

            differentActivities.ToList().ForEach(activity =>
            {
                var timeForActivityPerDay =
                    logEntriesPerDay.ToDictionary(x => x.Key, x => x.Where(y => y.Activity == activity).Sum(y => y.Duration));
                WriteReportActivityEntry(engineer, activity, timeForActivityPerDay);
            });

            differentLabels.ToList().ForEach(label =>
            {
                var timeForLabelPerDay =
                    logEntriesPerDay.ToDictionary(x => x.Key, x => x.Where(y => y.Label == label).Sum(y => y.Duration));
                WriteReportLabelEntry(engineer, label, timeForLabelPerDay);
            });

            differentIssueNumbers.ToList().ForEach(issueNumber =>
            {
                var timeForIssueNumberPerDay = logEntriesPerDay.ToDictionary(x => x.Key,
                    x => x.Where(y => y.IssueNumber == issueNumber).Sum(y => y.Duration));
                WriteReportIssueNumberEntry(engineer, issueNumber, timeForIssueNumberPerDay);
            });
        }

        // TODO, to one method
        private static void WriteReportIssueNumberEntry(string engineer, int issueNumber, Dictionary<DateTime, float> timeForIssueNumberPerDay)
        {
            _reportIssueNumberRepository.SaveReportEntry(new ReportWeekEntry(engineer, issueNumber.ToString(), _generateForWeekWhichContainsDate, timeForIssueNumberPerDay));
        }

        private static void WriteReportLabelEntry(string engineer, string label, Dictionary<DateTime, float> timeForLabelPerDay)
        {
            _reportLabelRepository.SaveReportEntry(new ReportWeekEntry(engineer, label, _generateForWeekWhichContainsDate, timeForLabelPerDay));
        }

        private static void WriteReportActivityEntry(string engineer, string activity, Dictionary<DateTime, float> timeForActivityPerDay)
        {
            _reportActivityRepository.SaveReportEntry(new ReportWeekEntry(engineer, activity, _generateForWeekWhichContainsDate, timeForActivityPerDay));
        }
    }
}
