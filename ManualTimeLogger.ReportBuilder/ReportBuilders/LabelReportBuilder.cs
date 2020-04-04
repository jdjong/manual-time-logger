using System;
using System.Collections.Generic;
using System.Linq;
using ManualTimeLogger.Domain;

// ReSharper disable PossibleMultipleEnumeration

namespace ManualTimeLogger.ReportBuilder.ReportBuilders
{
    public class LabelReportBuilder
    {
        private readonly IReportCsvFileRepository _repository;
        private readonly DateTime _firstDayOfReport;
        private readonly int _periodNrOfDays;
        private readonly Func<LogEntry, bool> _accountFilter;

        public LabelReportBuilder(IReportCsvFileRepository repository, DateTime firstDayOfReport, int periodNrOfDays, string accountFilter)
        {
            _repository = repository;
            _firstDayOfReport = firstDayOfReport;
            _periodNrOfDays = periodNrOfDays;
            _accountFilter = string.IsNullOrEmpty(accountFilter)
                ? (Func<LogEntry, bool>) (x => true)
                : x => x.Account == accountFilter;
        }

        public void Build(string groupedBy, IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            var accountLogEntriesPerDay = logEntriesPerDay
                .SelectMany(x => x)
                .Where(_accountFilter)
                .GroupBy(x => x.CreateDate)
                .ToList();
            
            var differentLabels = accountLogEntriesPerDay
                .SelectMany(x => x)
                .Select(logEntry => logEntry.Label)
                .Distinct()
                .OrderBy(activity => activity);

            differentLabels.ToList().ForEach(thenGroupedByLabel =>
            {
                var timeForLabelPerDay = accountLogEntriesPerDay.ToDictionary(x => x.Key, x => x.Where(y => y.Label == thenGroupedByLabel).Sum(y => y.Duration));
                _repository.SaveReportEntry(new ReportEntry(groupedBy, thenGroupedByLabel, _firstDayOfReport, _periodNrOfDays, timeForLabelPerDay));
            });
        }

        public void Build(IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            Build(null, logEntriesPerDay);
        }
    }
}
