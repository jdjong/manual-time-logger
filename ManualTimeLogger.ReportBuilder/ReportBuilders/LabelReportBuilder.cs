using System;
using System.Collections.Generic;
using System.Linq;
using ManualTimeLogger.Domain;
// ReSharper disable PossibleMultipleEnumeration

namespace ManualTimeLogger.ReportBuilder.ReportBuilders
{
    class LabelReportBuilder
    {
        private readonly WeekReportCsvFileRepository _repository;
        private readonly DateTime _firstDayOfReport;

        public LabelReportBuilder(WeekReportCsvFileRepository repository, DateTime firstDayOfReport)
        {
            _repository = repository;
            _firstDayOfReport = firstDayOfReport;
        }

        public void Build(string engineer, IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            var differentLabels = logEntriesPerDay.SelectMany(x => x).Select(x => x.Label).Distinct();

            differentLabels.ToList().ForEach(label =>
            {
                var timeForLabelPerDay = logEntriesPerDay.ToDictionary(x => x.Key, x => x.Where(y => y.Label == label).Sum(y => y.Duration));
                _repository.SaveReportEntry(new WeekReportEntry(engineer, label, _firstDayOfReport, timeForLabelPerDay));
            });
        }

        public void Build(IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            Build(null, logEntriesPerDay);
        }
    }
}
