﻿using System.IO;
using System.Linq;
using ManualTimeLogger.Persistence;
using ManualTimeLogger.ReportBuilder.Commands;

namespace ManualTimeLogger.ReportBuilder
{
    class Program
    {
        private static ICommand _buildReportCommand;

        static void Main(string[] args)
        {
            var reportingCommandProvider = new CommandProvider();
            _buildReportCommand = reportingCommandProvider.GetCommand(args);

            Handle((dynamic)_buildReportCommand);
        }

        private static void Handle(BuildWeekReportsCommand command)
        {
            var timeLogsBasePath = Properties.Settings.Default.TimeLogsBasePath;
            var allTimeLogFileNames = Directory.EnumerateFiles(timeLogsBasePath);
            var allTimeLogRepositories = allTimeLogFileNames.Select(filePaths => new CsvFileRepository(timeLogsBasePath, Path.GetFileName(filePaths))).ToList();

            var logEntriesPerEngineer = allTimeLogRepositories.ToDictionary(repository => repository.GetEngineerName(), repository => repository.GetAllLogEntries());
            var logEntriesPerDay = logEntriesPerEngineer.SelectMany(x => x.Value).GroupBy(x => x.CreateDate);

            var perEngineerReportsBuilder = new PerEngineerWeekReportsBuilder(Properties.Settings.Default.ReportsBasePaths, command.FirstDayOfPeriod, logEntriesPerEngineer);
            var cumulativeReportsBuilder = new CumulativeWeekReportsBuilder(Properties.Settings.Default.ReportsBasePaths, command.FirstDayOfPeriod, logEntriesPerDay);
            
            perEngineerReportsBuilder.Build();
            cumulativeReportsBuilder.Build();
        }

        private static void Handle(BuildMonthReportsCommand command)
        {
            var reportsBuilder = new MonthReportsBuilder(Properties.Settings.Default.TimeLogsBasePath, Properties.Settings.Default.ReportsBasePaths, command.FirstDayOfPeriod);
            reportsBuilder.Build();
        }
    }
}
