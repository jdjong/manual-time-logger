using System;
using System.Collections.Generic;
using System.Linq;
using ManualTimeLogger.Domain;

// ReSharper disable PossibleMultipleEnumeration

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
            _repository.CreateHeader(new[] { $"\"Wie\"{repository.CsvSeparator}\"Issue\"{repository.CsvSeparator}{string.Join(repository.CsvSeparator.ToString(), Enumerable.Range(0, _periodNrOfDays).Select(nr => $"\"{_firstDayOfReport.AddDays(nr):yyyyMMdd}\""))}" });
        }

        public void Build(string engineer, IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            var differentIssueNumbers = logEntriesPerDay
                .SelectMany(x => x)
                .Where(_accountFilter)
                .Select(x => x.IssueNumber)
                .Distinct();

            differentIssueNumbers.ToList().ForEach(issueNumber =>
            {
                var timeForIssueNumberPerDay = logEntriesPerDay
                    .ToDictionary(x => x.Key, x => x.Where(y => y.IssueNumber == issueNumber)
                                                    .Where(_accountFilter)
                                                    .Sum(y => y.Duration));
                _repository.SaveReportEntry(new ReportEntry(engineer, issueNumber.ToString(), _firstDayOfReport, _periodNrOfDays, timeForIssueNumberPerDay));
            });
        }

        public void Build(IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            Build(null, logEntriesPerDay);
        }
    }
}
