using System;
using System.Collections.Generic;
using System.Linq;
using ManualTimeLogger.Domain;

namespace ManualTimeLogger.ReportBuilder.ReportBuilders
{
    public class TwinfieldReportBuilder
    {
        private readonly DateTime _firstDayOfReport;
        private readonly int _periodNrOfDays;
        private readonly IReportCsvFileRepository _repository;

        public TwinfieldReportBuilder(IReportCsvFileRepository repository, DateTime firstDayOfReport, int periodNrOfDays)
        {
            _firstDayOfReport = firstDayOfReport;
            _periodNrOfDays = periodNrOfDays;
            _repository = repository;

            _repository.CreateHeader(new[] { $"\"Wie\"{repository.CsvSeparator}\"Klant\"{repository.CsvSeparator}\"Totaal\"{repository.CsvSeparator}{string.Join(repository.CsvSeparator.ToString(), Enumerable.Range(0, _periodNrOfDays).Select(nr => $"\"{_firstDayOfReport.AddDays(nr):yyyyMMdd}\""))}" });
        }

        public void Build(Dictionary<string, IEnumerable<LogEntry>> logEntriesPerEngineer)
        {
            logEntriesPerEngineer.ToList().ForEach(engineerLogEntries =>
            {
                foreach (var logEntriesForAccount in engineerLogEntries.Value.GroupBy(x => x.Account))
                {
                    var nrOfHoursPerDayForAccountForEngineer = logEntriesForAccount.GroupBy(x => x.CreateDate).ToDictionary(x => x.Key, x => x.Sum(y => y.Duration));
                    _repository.SaveReportEntry(new ReportEntry(engineerLogEntries.Key, logEntriesForAccount.Key, _firstDayOfReport, _periodNrOfDays, nrOfHoursPerDayForAccountForEngineer));
                }
            });
        }
    }
}
