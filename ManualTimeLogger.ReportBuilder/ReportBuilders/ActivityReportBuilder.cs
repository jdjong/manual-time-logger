using System;
using System.Collections.Generic;
using System.Linq;
using ManualTimeLogger.Domain;

// ReSharper disable PossibleMultipleEnumeration

namespace ManualTimeLogger.ReportBuilder.ReportBuilders
{
    public class ActivityReportBuilder
    {
        private readonly DateTime _firstDayOfReport;
        private readonly int _periodNrOfDays;
        private readonly Func<LogEntry, bool> _accountFilter;
        private readonly IReportCsvFileRepository _repository;

        public ActivityReportBuilder(IReportCsvFileRepository repository, DateTime firstDayOfReport, int periodNrOfDays, string accountFilter)
        {
            _firstDayOfReport = firstDayOfReport;
            _periodNrOfDays = periodNrOfDays;
            _accountFilter = string.IsNullOrEmpty(accountFilter)
                ? (Func<LogEntry, bool>) (x => true)
                : x => x.Account == accountFilter;
            _repository = repository;
        }

        public void Build(string groupBy, IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            var accountLogEntriesPerDay = logEntriesPerDay
                .SelectMany(x => x)
                .Where(_accountFilter)
                .GroupBy(x => x.CreateDate)
                .ToList();

            var differentActivities = accountLogEntriesPerDay
                .SelectMany(x => x)
                .Select(logEntry => logEntry.Activity)
                .Distinct()
                .OrderBy(activity => activity);

            differentActivities.ToList().ForEach(thenGroupedByActivity =>
            {
                var timeForActivityPerDay = accountLogEntriesPerDay.ToDictionary(x => x.Key, x => x.Where(y => y.Activity == thenGroupedByActivity).Sum(y => y.Duration));
                _repository.SaveReportEntry(new ReportEntry(groupBy, thenGroupedByActivity, _firstDayOfReport, _periodNrOfDays, timeForActivityPerDay));
            });
        }

        public void Build(IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            Build(null, logEntriesPerDay);
        }
    }
}
