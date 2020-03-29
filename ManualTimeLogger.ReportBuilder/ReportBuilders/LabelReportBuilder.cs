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
            _repository.CreateHeader(new[] { $"\"Wie\"{repository.CsvSeparator}\"Thema\"{repository.CsvSeparator}{string.Join(repository.CsvSeparator.ToString(), Enumerable.Range(0, _periodNrOfDays).Select(nr => $"\"{_firstDayOfReport.AddDays(nr):yyyyMMdd}\""))}" });
        }

        public void Build(string engineer, IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            var differentLabels = logEntriesPerDay
                .SelectMany(x => x)
                .Where(_accountFilter)
                .OrderBy(x => x.Label)
                .Select(x => x.Label)
                .Distinct();

            differentLabels.ToList().ForEach(label =>
            {
                var timeForLabelPerDay = logEntriesPerDay
                    .ToDictionary(x => x.Key, x => x.Where(y => y.Label == label)
                                                    .Where(_accountFilter)
                                                    .Sum(y => y.Duration));
                _repository.SaveReportEntry(new ReportEntry(engineer, label, _firstDayOfReport, _periodNrOfDays, timeForLabelPerDay));
            });
        }

        public void Build(IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            Build(null, logEntriesPerDay);
        }
    }
}
