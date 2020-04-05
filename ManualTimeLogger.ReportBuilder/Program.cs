using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ManualTimeLogger.Domain;
using ManualTimeLogger.Persistence;
using ManualTimeLogger.ReportBuilder.Commands;
using ManualTimeLogger.ReportBuilder.ReportSets;

namespace ManualTimeLogger.ReportBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            var reportingCommandProvider = new CommandProvider();
            Handle((dynamic)reportingCommandProvider.GetCommand(args));
        }

        private static void Handle(BuildWeekReportsCommand command)
        {
            var logEntriesPerEngineer = GetLogEntriesPerEngineer();
            var logEntriesPerLabel = GetLogEntriesPerLabel(logEntriesPerEngineer);
            var logEntriesPerDay = GetLogEntriesPerDay(logEntriesPerEngineer);

            var perEngineerWeekReportSet = new PerEngineerWeekReportSet(Properties.Settings.Default.ReportsBasePath, command.FromDay, command.AccountFilter);
            var perLabelWeekReportSet = new PerLabelWeekReportSet(Properties.Settings.Default.ReportsBasePath, command.FromDay, command.AccountFilter);
            var cumulativeWeekReportSet = new CumulativeWeekReportSet(Properties.Settings.Default.ReportsBasePath, command.FromDay, command.AccountFilter);
            
            perEngineerWeekReportSet.Create(logEntriesPerEngineer);
            perLabelWeekReportSet.Create(logEntriesPerLabel);
            cumulativeWeekReportSet.Create(logEntriesPerDay);
        }

        private static void Handle(BuildMonthReportsCommand command)
        {
            var logEntriesPerEngineer = GetLogEntriesPerEngineer();
            var logEntriesPerLabel = GetLogEntriesPerLabel(logEntriesPerEngineer);
            var logEntriesPerDay = GetLogEntriesPerDay(logEntriesPerEngineer);

            var perEngineerMonthReportSet = new PerEngineerMonthReportSet(Properties.Settings.Default.ReportsBasePath, command.FromDay, command.AccountFilter);
            var perLabelMonthReportSet = new PerLabelMonthReportSet(Properties.Settings.Default.ReportsBasePath, command.FromDay, command.AccountFilter);
            var cumulativeMonthReportSet = new CumulativeMonthReportSet(Properties.Settings.Default.ReportsBasePath, command.FromDay, command.AccountFilter);

            perEngineerMonthReportSet.Create(logEntriesPerEngineer);
            perLabelMonthReportSet.Create(logEntriesPerLabel);
            cumulativeMonthReportSet.Create(logEntriesPerDay);
        }

        private static Dictionary<string, IEnumerable<LogEntry>> GetLogEntriesPerLabel(Dictionary<string, IEnumerable<LogEntry>> logEntriesPerEngineer)
        {
            var logEntriesPerLabel = logEntriesPerEngineer.SelectMany(x => x.Value).GroupBy(x => x.Label);
            return logEntriesPerLabel.ToDictionary(grouping => grouping.Key, grouping => grouping.Select(logEntry => logEntry));
        }

        private static IEnumerable<IGrouping<DateTime, LogEntry>> GetLogEntriesPerDay(Dictionary<string, IEnumerable<LogEntry>> logEntriesPerEngineer)
        {
            return logEntriesPerEngineer.SelectMany(x => x.Value).GroupBy(x => x.CreateDate);
        }

        private static Dictionary<string, IEnumerable<LogEntry>> GetLogEntriesPerEngineer()
        {
            var allTimeLogRepositories = GetAllTimeLogRepositories();

            return allTimeLogRepositories
                .GroupBy(repo => repo.GetEngineerName())
                .ToDictionary(grouping => grouping.Key, grouping => grouping.SelectMany(repo => repo.GetAllLogEntries()));
        }

        private static List<CsvFileRepository> GetAllTimeLogRepositories()
        {
            var timeLogsBasePath = Properties.Settings.Default.TimeLogsBasePath;
            var allTimeLogFileNames = Directory.EnumerateFiles(timeLogsBasePath);
            var allTimeLogRepositories = allTimeLogFileNames.Select(filePaths => new CsvFileRepository(timeLogsBasePath, Path.GetFileName(filePaths))).ToList();
            return allTimeLogRepositories;
        }
    }
}
