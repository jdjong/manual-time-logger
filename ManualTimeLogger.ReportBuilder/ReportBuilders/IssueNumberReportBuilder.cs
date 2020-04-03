using System;
using System.Collections.Generic;
using System.Linq;
using ManualTimeLogger.Domain;

// ReSharper disable PossibleMultipleEnumeration

// TODO, unused in reports, so remove class?
namespace ManualTimeLogger.ReportBuilder.ReportBuilders
{
    public class IssueNumberReportBuilder
    {
        private readonly IReportCsvFileRepository _repository;
        private readonly DateTime _firstDayOfReport;
        private readonly int _periodNrOfDays;
        private readonly Func<LogEntry, bool> _accountFilter;

        public IssueNumberReportBuilder(IReportCsvFileRepository repository, DateTime firstDayOfReport, int periodNrOfDays, string accountFilter)
        {
            _repository = repository;
            _firstDayOfReport = firstDayOfReport;
            _periodNrOfDays = periodNrOfDays;
            _accountFilter = string.IsNullOrEmpty(accountFilter)
                ? (Func<LogEntry, bool>) (x => true)
                : x => x.Account == accountFilter;
            _repository.CreateHeader(new[] { $"\"Wie\"{repository.CsvSeparator}\"Issue\"{repository.CsvSeparator}\"Totaal\"{repository.CsvSeparator}{string.Join(repository.CsvSeparator.ToString(), Enumerable.Range(0, _periodNrOfDays).Select(nr => $"\"{_firstDayOfReport.AddDays(nr):yyyyMMdd}\""))}" });
        }

        public void Build(string groupedBy, IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            var accountLogEntriesPerDay = logEntriesPerDay
                .SelectMany(x => x)
                .Where(_accountFilter)
                .GroupBy(x => x.CreateDate)
                .ToList();

            var differentIssueNumbers = accountLogEntriesPerDay
                .SelectMany(x => x)
                .Select(logEntry => logEntry.IssueNumber)
                .Distinct()
                .OrderBy(activity => activity);

            differentIssueNumbers.ToList().ForEach(thenGroupedByIssueNumber =>
            {
                var timeForIssueNumberPerDay = accountLogEntriesPerDay.ToDictionary(x => x.Key, x => x.Where(y => y.IssueNumber == thenGroupedByIssueNumber).Sum(y => y.Duration));
                _repository.SaveReportEntry(new ReportEntry(groupedBy, thenGroupedByIssueNumber.ToString(), _firstDayOfReport, _periodNrOfDays, timeForIssueNumberPerDay));
            });
        }

        public void Build(IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            Build(null, logEntriesPerDay);
        }
    }
}
