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
        }

        public void Build(string groupedBy, IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            var accountLogEntriesPerDay = logEntriesPerDay
                .SelectMany(x => x)
                .Where(_accountFilter)
                .GroupBy(x => x.CreateDate)
                .ToList();
            
            var differentAccounts = accountLogEntriesPerDay
                .SelectMany(x => x)
                .Select(logEntry => logEntry.Account)
                .Distinct()
                .OrderBy(activity => activity);

            differentAccounts.ToList().ForEach(thenGroupedByAccount =>
            {
                var timeForLabelPerDay = accountLogEntriesPerDay.ToDictionary(x => x.Key, x => x.Where(y => y.Account == thenGroupedByAccount).Sum(y => y.Duration));
                _repository.SaveReportEntry(new ReportEntry(groupedBy, thenGroupedByAccount, _firstDayOfReport, _periodNrOfDays, timeForLabelPerDay));
            });
        }
    }
}
