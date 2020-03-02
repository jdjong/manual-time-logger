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
        private readonly ReportCsvFileRepository _repository;

        public ActivityReportBuilder(ReportCsvFileRepository repository, DateTime firstDayOfReport, int periodNrOfDays)
        {
            _firstDayOfReport = firstDayOfReport;
            _periodNrOfDays = periodNrOfDays;
            _repository = repository;

            _repository.CreateHeader(new[] { $"\"Wie\"{ReportCsvFileRepository.CsvSeparator}\"Activiteit\"{ReportCsvFileRepository.CsvSeparator}{string.Join(ReportCsvFileRepository.CsvSeparator.ToString(), Enumerable.Range(0, _periodNrOfDays).Select(nr => $"\"{_firstDayOfReport.AddDays(nr):yyyyMMdd}\""))}" });
        }

        public void Build(string engineer, IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            var differentActivities = logEntriesPerDay.SelectMany(x => x).Select(x => x.Activity).Distinct();

            differentActivities.ToList().ForEach(activity =>
            {
                var timeForActivityPerDay = logEntriesPerDay.ToDictionary(x => x.Key, x => x.Where(y => y.Activity == activity).Sum(y => y.Duration));
                _repository.SaveReportEntry(new ReportEntry(engineer, activity, _firstDayOfReport, _periodNrOfDays, timeForActivityPerDay));
            });
        }

        public void Build(IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            Build(null, logEntriesPerDay);
        }
    }
}
