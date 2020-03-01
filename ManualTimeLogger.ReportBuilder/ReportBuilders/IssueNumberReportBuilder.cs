using System;
using System.Collections.Generic;
using System.Linq;
using ManualTimeLogger.Domain;

namespace ManualTimeLogger.ReportBuilder.ReportBuilders
{
    class IssueNumberReportBuilder
    {
        private readonly WeekReportCsvFileRepository _repository;
        private readonly DateTime _firstDayOfReport;

        public IssueNumberReportBuilder(WeekReportCsvFileRepository repository, DateTime firstDayOfReport)
        {
            _repository = repository;
            _firstDayOfReport = firstDayOfReport;
        }

        public void Build(string engineer, IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            var differentIssueNumbers = logEntriesPerDay.SelectMany(x => x).Select(x => x.IssueNumber).Distinct();

            differentIssueNumbers.ToList().ForEach(issueNumber =>
            {
                var timeForIssueNumberPerDay = logEntriesPerDay.ToDictionary(x => x.Key, x => x.Where(y => y.IssueNumber == issueNumber).Sum(y => y.Duration));
                _repository.SaveReportEntry(new WeekReportEntry(engineer, issueNumber.ToString(), _firstDayOfReport, timeForIssueNumberPerDay));
            });
        }

        public void Build(IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            Build(null, logEntriesPerDay);
        }
    }
}
