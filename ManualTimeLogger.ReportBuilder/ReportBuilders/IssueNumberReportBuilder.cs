using System;
using System.Collections.Generic;
using System.Linq;
using ManualTimeLogger.Domain;
// ReSharper disable PossibleMultipleEnumeration

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
