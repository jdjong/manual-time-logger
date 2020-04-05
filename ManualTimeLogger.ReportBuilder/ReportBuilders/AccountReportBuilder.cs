using System;
using System.Collections.Generic;
using System.Linq;
using ManualTimeLogger.Domain;
using ManualTimeLogger.ReportBuilder.Persistence;

// ReSharper disable PossibleMultipleEnumeration

namespace ManualTimeLogger.ReportBuilder.ReportBuilders
{
    public class AccountReportBuilder
    {
        private readonly DateTime _firstDayOfReport;
        private readonly int _periodNrOfDays;
        private readonly IRepository _repository;

        public AccountReportBuilder(IRepository repository, DateTime firstDayOfReport, int periodNrOfDays)
        {
            _firstDayOfReport = firstDayOfReport;
            _periodNrOfDays = periodNrOfDays;
            _repository = repository;
        }

        public void Build(string groupedBy, IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            var differentAccounts = logEntriesPerDay
                .SelectMany(x => x)
                .Select(logEntry => logEntry.Account)
                .Distinct()
                .OrderBy(activity => activity);

            differentAccounts.ToList().ForEach(thenGroupedByAccount =>
            {
                var timeForLabelPerDay = logEntriesPerDay.ToDictionary(x => x.Key, x => x.Where(y => y.Account == thenGroupedByAccount).Sum(y => y.Duration));
                _repository.SaveReportEntry(new ReportEntry(groupedBy, thenGroupedByAccount, _firstDayOfReport, _periodNrOfDays, timeForLabelPerDay));
            });
        }
    }
}
