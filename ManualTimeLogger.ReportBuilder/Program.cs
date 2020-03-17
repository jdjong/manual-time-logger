using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ManualTimeLogger.Domain;
using ManualTimeLogger.Persistence;
using ManualTimeLogger.ReportBuilder.Commands;

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
            var logEntriesPerDay = GetLogEntriesPerDay(logEntriesPerEngineer);

            var perEngineerWeekReportsBuilder = new PerEngineerWeekReportsBuilder(Properties.Settings.Default.ReportsBasePath, command.FromDay, logEntriesPerEngineer);
            var cumulativeWeekReportsBuilder = new CumulativeWeekReportsBuilder(Properties.Settings.Default.ReportsBasePath, command.FromDay, logEntriesPerDay);
            
            perEngineerWeekReportsBuilder.Build();
            cumulativeWeekReportsBuilder.Build();
        }

        private static void Handle(BuildMonthReportsCommand command)
        {
            var logEntriesPerEngineer = GetLogEntriesPerEngineer();
            var logEntriesPerDay = GetLogEntriesPerDay(logEntriesPerEngineer);

            var perEngineerMonthReportsBuilder = new PerEngineerMonthReportsBuilder(Properties.Settings.Default.ReportsBasePath, command.FromDay, logEntriesPerEngineer);
            var cumulativeMonthReportsBuilder = new CumulativeMonthReportsBuilder(Properties.Settings.Default.ReportsBasePath, command.FromDay, logEntriesPerDay);

            perEngineerMonthReportsBuilder.Build();
            cumulativeMonthReportsBuilder.Build();
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
