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
    public class CommandHandler
    {
        private readonly string _timeLogsBasePath;
        private readonly string _reportsBasePath;

        // TODO, add big integration test based on an example time log and generated reports which are tested ok. Test should check if time log generates expected reports.

        public CommandHandler(string timeLogsBasePath, string reportsBasePath)
        {
            _timeLogsBasePath = timeLogsBasePath;
            _reportsBasePath = reportsBasePath;
        }

        private void Handle(BuildWeekReportsCommand command)
        {
            var logEntriesPerEngineer = string.IsNullOrEmpty(command.AccountFilter)
                ? GetLogEntriesPerEngineer()
                : GetAccountFilteredLogEntriesPerEngineer(command.AccountFilter);
            var logEntriesPerLabel = GetLogEntriesPerLabel(logEntriesPerEngineer);
            var logEntriesPerDay = GetLogEntriesPerDay(logEntriesPerEngineer);

            var perEngineerWeekReportSet = new PerEngineerWeekReportSet(_reportsBasePath, command.FromDay, command.AccountFilter);
            var perLabelWeekReportSet = new PerLabelWeekReportSet(_reportsBasePath, command.FromDay, command.AccountFilter);
            var cumulativeWeekReportSet = new CumulativeWeekReportSet(_reportsBasePath, command.FromDay, command.AccountFilter);

            perEngineerWeekReportSet.Create(logEntriesPerEngineer);
            perLabelWeekReportSet.Create(logEntriesPerLabel);
            cumulativeWeekReportSet.Create(logEntriesPerDay);
        }

        public void Handle(BuildMonthReportsCommand command)
        {
            var logEntriesPerEngineer = string.IsNullOrEmpty(command.AccountFilter) 
                ? GetLogEntriesPerEngineer() 
                : GetAccountFilteredLogEntriesPerEngineer(command.AccountFilter);
            var logEntriesPerLabel = GetLogEntriesPerLabel(logEntriesPerEngineer);
            var logEntriesPerDay = GetLogEntriesPerDay(logEntriesPerEngineer);

            var perEngineerMonthReportSet = new PerEngineerMonthReportSet(_reportsBasePath, command.FromDay, command.AccountFilter);
            var perLabelMonthReportSet = new PerLabelMonthReportSet(_reportsBasePath, command.FromDay, command.AccountFilter);
            var cumulativeMonthReportSet = new CumulativeMonthReportSet(_reportsBasePath, command.FromDay, command.AccountFilter);

            perEngineerMonthReportSet.Create(logEntriesPerEngineer);
            perLabelMonthReportSet.Create(logEntriesPerLabel);
            cumulativeMonthReportSet.Create(logEntriesPerDay);
        }

        private Dictionary<string, IEnumerable<LogEntry>> GetLogEntriesPerLabel(Dictionary<string, IEnumerable<LogEntry>> logEntriesPerEngineer)
        {
            var logEntriesPerLabel = logEntriesPerEngineer.SelectMany(x => x.Value).GroupBy(x => x.Label);
            return logEntriesPerLabel.ToDictionary(grouping => grouping.Key, grouping => grouping.Select(logEntry => logEntry));
        }

        private IEnumerable<IGrouping<DateTime, LogEntry>> GetLogEntriesPerDay(Dictionary<string, IEnumerable<LogEntry>> logEntriesPerEngineer)
        {
            return logEntriesPerEngineer.SelectMany(x => x.Value).GroupBy(x => x.CreateDate);
        }

        private Dictionary<string, IEnumerable<LogEntry>> GetLogEntriesPerEngineer()
        {
            var allTimeLogRepositories = GetAllTimeLogRepositories();

            return allTimeLogRepositories
                .GroupBy(repo => repo.GetEngineerName())
                .ToDictionary(grouping => grouping.Key, grouping => grouping.SelectMany(repo => repo.GetAllLogEntries()));
        }

        private Dictionary<string, IEnumerable<LogEntry>> GetAccountFilteredLogEntriesPerEngineer(string accountFilter)
        {
            var accountFilterFunc = string.IsNullOrEmpty(accountFilter)
                ? (Func<LogEntry, bool>)(x => true)
                : x => x.Account == accountFilter;
            var allTimeLogRepositories = GetAllTimeLogRepositories();

            var filteredResult = allTimeLogRepositories
                .GroupBy(repo => repo.GetEngineerName())
                .ToDictionary(grouping => grouping.Key, grouping => grouping.SelectMany(repo => repo.GetAllLogEntries().Where(accountFilterFunc)));
            
            var keysWithoutLogEntries = filteredResult.Where(x => !x.Value.Any()).Select(x => x.Key);
            
            foreach (var keyWithoutLogEntries in keysWithoutLogEntries)
            {
                filteredResult.Remove(keyWithoutLogEntries);
            }

            return filteredResult;
        }

        private List<CsvFileRepository> GetAllTimeLogRepositories()
        {
            var allTimeLogFileNames = Directory.EnumerateFiles(_timeLogsBasePath);
            var allTimeLogRepositories = allTimeLogFileNames.Select(filePath => new CsvFileRepository(_timeLogsBasePath, Path.GetFileName(filePath))).ToList();
            return allTimeLogRepositories;
        }
    }
}
