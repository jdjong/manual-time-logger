using System;
using System.Collections.Generic;
using System.Linq;
using ManualTimeLogger.Domain;

// ReSharper disable PossibleMultipleEnumeration

namespace ManualTimeLogger.ReportBuilder.ReportBuilders
{
    public class AccountReportBuilder
    {
        private readonly DateTime _firstDayOfReport;
        private readonly int _periodNrOfDays;
        private readonly Func<LogEntry, bool> _accountFilter;
        private readonly IReportCsvFileRepository _repository;

        public AccountReportBuilder(IReportCsvFileRepository repository, DateTime firstDayOfReport, int periodNrOfDays, string accountFilter)
        {
            _firstDayOfReport = firstDayOfReport;
            _periodNrOfDays = periodNrOfDays;
            _accountFilter = string.IsNullOrEmpty(accountFilter)
                ? (Func<LogEntry, bool>) (x => true)
                : x => x.Account == accountFilter;
            _repository = repository;

            _repository.CreateHeader(new[] { $"\"Wie\"{repository.CsvSeparator}\"Klant\"{repository.CsvSeparator}\"Totaal\"{repository.CsvSeparator}{string.Join(repository.CsvSeparator.ToString(), Enumerable.Range(0, _periodNrOfDays).Select(nr => $"\"{_firstDayOfReport.AddDays(nr):yyyyMMdd}\""))}" });
        }

        public void Build(string engineer, IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            var accounts = logEntriesPerDay
                .SelectMany(x => x)
                .Where(_accountFilter)
                .OrderBy(x => x.Account)
                .Select(x => x.Account)
                .Distinct();

            accounts.ToList().ForEach(account =>
            {
                var timeForLabelPerDay = logEntriesPerDay
                    .ToDictionary(x => x.Key, x => x.Where(y => y.Account == account)
                        .Where(_accountFilter)
                        .Sum(y => y.Duration));
                _repository.SaveReportEntry(new ReportEntry(engineer, account, _firstDayOfReport, _periodNrOfDays, timeForLabelPerDay));
            });
        }
    }
}
