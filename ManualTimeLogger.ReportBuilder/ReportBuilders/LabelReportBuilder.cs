using System;
using System.Collections.Generic;
using System.Linq;
using ManualTimeLogger.Domain;

// ReSharper disable PossibleMultipleEnumeration

namespace ManualTimeLogger.ReportBuilder.ReportBuilders
{
    public class LabelReportBuilder
    {
        private readonly IRepository _repository;
        private readonly DateTime _firstDayOfReport;
        private readonly int _periodNrOfDays;

        public LabelReportBuilder(IRepository repository, DateTime firstDayOfReport, int periodNrOfDays)
        {
            _repository = repository;
            _firstDayOfReport = firstDayOfReport;
            _periodNrOfDays = periodNrOfDays;
        }

        public void Build(string groupedBy, IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            var differentLabels = logEntriesPerDay
                .SelectMany(x => x)
                .Select(logEntry => logEntry.Label)
                .Distinct()
                .OrderBy(activity => activity);

            differentLabels.ToList().ForEach(thenGroupedByLabel =>
            {
                var timeForLabelPerDay = logEntriesPerDay.ToDictionary(x => x.Key, x => x.Where(y => y.Label == thenGroupedByLabel).Sum(y => y.Duration));
                _repository.SaveReportEntry(new ReportEntry(groupedBy, thenGroupedByLabel, _firstDayOfReport, _periodNrOfDays, timeForLabelPerDay));
            });
        }

        public void Build(IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            Build(null, logEntriesPerDay);
        }
    }
}
