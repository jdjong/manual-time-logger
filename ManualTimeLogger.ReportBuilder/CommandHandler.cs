using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ManualTimeLogger.Domain;
using ManualTimeLogger.ReportBuilder.Commands;
using ManualTimeLogger.ReportBuilder.Persistence;
using ManualTimeLogger.ReportBuilder.ReportSets;

namespace ManualTimeLogger.ReportBuilder
{
    public class CommandHandler
    {
        private readonly ManualTimeLogger.Persistence.IRepositoryFactory _timeLogRepositoryFactory;
        private readonly IRepositoryFactory _reportRepositoryFactory;

        // TODO, add big integration test based on an example time log and generated reports which are tested ok. Test should check if time log generates expected reports.

        public CommandHandler(ManualTimeLogger.Persistence.IRepositoryFactory timeLogRepositoryFactory, IRepositoryFactory reportRepositoryFactory)
        {
            _timeLogRepositoryFactory = timeLogRepositoryFactory;
            _reportRepositoryFactory = reportRepositoryFactory;
        }

        private void Handle(BuildWeekReportsCommand command)
        {
            var logEntriesPerEngineer = string.IsNullOrEmpty(command.AccountFilter)
                ? GetLogEntriesPerEngineer()
                : GetAccountFilteredLogEntriesPerEngineer(command.AccountFilter);
            var logEntriesPerLabel = GetLogEntriesPerLabel(logEntriesPerEngineer);
            var logEntriesPerDay = GetLogEntriesPerDay(logEntriesPerEngineer);

            var perEngineerWeekReportSet = new PerEngineerWeekReportSet(_reportRepositoryFactory, command.FromDay);
            var perLabelWeekReportSet = new PerLabelWeekReportSet(_reportRepositoryFactory, command.FromDay);
            var cumulativeWeekReportSet = new CumulativeWeekReportSet(_reportRepositoryFactory, command.FromDay);

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

            var perEngineerMonthReportSet = new PerEngineerMonthReportSet(_reportRepositoryFactory, command.FromDay);
            var perLabelMonthReportSet = new PerLabelMonthReportSet(_reportRepositoryFactory, command.FromDay);
            var cumulativeMonthReportSet = new CumulativeMonthReportSet(_reportRepositoryFactory, command.FromDay);

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

        private List<ManualTimeLogger.Persistence.IRepository> GetAllTimeLogRepositories()
        {
            var allTimeLogFileNames = Directory.EnumerateFiles(_timeLogRepositoryFactory.GetFilesBasePath());
            var allTimeLogRepositories = allTimeLogFileNames.Select(filePath => _timeLogRepositoryFactory.Create(Path.GetFileName(filePath))).ToList();
            return allTimeLogRepositories;
        }
    }
}
