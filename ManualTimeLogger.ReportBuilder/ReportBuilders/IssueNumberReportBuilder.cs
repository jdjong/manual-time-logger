using System;
using System.Collections.Generic;
using System.Linq;
using ManualTimeLogger.Domain;

// ReSharper disable PossibleMultipleEnumeration

// TODO, unit test along with label and activity builders
namespace ManualTimeLogger.ReportBuilder.ReportBuilders
{
    class IssueNumberReportBuilder
    {
        private readonly ReportCsvFileRepository _repository;
        private readonly DateTime _firstDayOfReport;
        private readonly int _periodNrOfDays;

        public IssueNumberReportBuilder(ReportCsvFileRepository repository, DateTime firstDayOfReport, int periodNrOfDays)
        {
            _repository = repository;
            _firstDayOfReport = firstDayOfReport;
            _periodNrOfDays = periodNrOfDays;

            _repository.CreateHeader(new[] { $"\"Wie\"{ReportCsvFileRepository.CsvSeparator}\"Issue\"{ReportCsvFileRepository.CsvSeparator}{string.Join(ReportCsvFileRepository.CsvSeparator.ToString(), Enumerable.Range(0, _periodNrOfDays).Select(nr => $"\"{_firstDayOfReport.AddDays(nr):yyyyMMdd}\""))}" });
        }

        public void Build(string engineer, IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            var differentIssueNumbers = logEntriesPerDay.SelectMany(x => x).Select(x => x.IssueNumber).Distinct();

            differentIssueNumbers.ToList().ForEach(issueNumber =>
            {
                var timeForIssueNumberPerDay = logEntriesPerDay.ToDictionary(x => x.Key, x => x.Where(y => y.IssueNumber == issueNumber).Sum(y => y.Duration));
                _repository.SaveReportEntry(new ReportEntry(engineer, issueNumber.ToString(), _firstDayOfReport, _periodNrOfDays, timeForIssueNumberPerDay));
            });
        }

        public void Build(IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            Build(null, logEntriesPerDay);
        }
    }
}
