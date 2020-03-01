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
        private readonly WeekReportCsvFileRepository _repository;

        // TODO, still week specific. Should this class work for week and month builds? If so, refactor. Also label and issue number report builders.
        public ActivityReportBuilder(WeekReportCsvFileRepository repository, DateTime firstDayOfReport)
        {
            _firstDayOfReport = firstDayOfReport;
            _repository = repository;
        }

        public void Build(string engineer, IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            var differentActivities = logEntriesPerDay.SelectMany(x => x).Select(x => x.Activity).Distinct();

            differentActivities.ToList().ForEach(activity =>
            {
                var timeForActivityPerDay = logEntriesPerDay.ToDictionary(x => x.Key, x => x.Where(y => y.Activity == activity).Sum(y => y.Duration));
                _repository.SaveReportEntry(new WeekReportEntry(engineer, activity, _firstDayOfReport, timeForActivityPerDay));
            });
        }

        public void Build(IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            Build(null, logEntriesPerDay);
        }
    }
}
