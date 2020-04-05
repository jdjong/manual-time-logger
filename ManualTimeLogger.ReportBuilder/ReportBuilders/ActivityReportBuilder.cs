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
        private readonly IRepository _repository;

        public ActivityReportBuilder(IRepository repository, DateTime firstDayOfReport, int periodNrOfDays)
        {
            _firstDayOfReport = firstDayOfReport;
            _periodNrOfDays = periodNrOfDays;
            _repository = repository;
        }

        public void Build(string groupBy, IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            var differentActivities = logEntriesPerDay
                .SelectMany(x => x)
                .Select(logEntry => logEntry.Activity)
                .Distinct()
                .OrderBy(activity => activity);

            differentActivities.ToList().ForEach(thenGroupedByActivity =>
            {
                var timeForActivityPerDay = logEntriesPerDay.ToDictionary(x => x.Key, x => x.Where(y => y.Activity == thenGroupedByActivity).Sum(y => y.Duration));
                _repository.SaveReportEntry(new ReportEntry(groupBy, thenGroupedByActivity, _firstDayOfReport, _periodNrOfDays, timeForActivityPerDay));
            });
        }

        public void Build(IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            Build(null, logEntriesPerDay);
        }
    }
}
