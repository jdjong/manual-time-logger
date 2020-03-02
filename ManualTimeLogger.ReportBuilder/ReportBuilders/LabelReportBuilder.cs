using System;
using System.Collections.Generic;
using System.Linq;
using ManualTimeLogger.Domain;

// ReSharper disable PossibleMultipleEnumeration

namespace ManualTimeLogger.ReportBuilder.ReportBuilders
{
    class LabelReportBuilder
    {
        private readonly ReportCsvFileRepository _repository;
        private readonly DateTime _firstDayOfReport;
        private readonly int _periodNrOfDays;

        public LabelReportBuilder(ReportCsvFileRepository repository, DateTime firstDayOfReport, int periodNrOfDays)
        {
            _repository = repository;
            _firstDayOfReport = firstDayOfReport;
            _periodNrOfDays = periodNrOfDays;
        }

        public void Build(string engineer, IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            var differentLabels = logEntriesPerDay.SelectMany(x => x).Select(x => x.Label).Distinct();

            differentLabels.ToList().ForEach(label =>
            {
                var timeForLabelPerDay = logEntriesPerDay.ToDictionary(x => x.Key, x => x.Where(y => y.Label == label).Sum(y => y.Duration));
                _repository.SaveReportEntry(new ReportEntry(engineer, label, _firstDayOfReport, _periodNrOfDays, timeForLabelPerDay));
            });
        }

        public void Build(IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            Build(null, logEntriesPerDay);
        }
    }
}
