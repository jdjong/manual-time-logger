using System;
using System.Collections.Generic;
using System.Linq;
using ManualTimeLogger.Domain;
using ManualTimeLogger.ReportBuilder.Persistence;
using ManualTimeLogger.ReportBuilder.ReportBuilders;

// ReSharper disable PossibleMultipleEnumeration

namespace ManualTimeLogger.ReportBuilder.ReportSets
{
    public class PerLabelWeekReportSet
    {
        private readonly ActivityReportBuilder _labelPerActivityReportBuilder;

        public PerLabelWeekReportSet(IRepositoryFactory repositoryFactory, DateTime firstDayOfWeek)
        {
            if (firstDayOfWeek.DayOfWeek != DayOfWeek.Monday)
            {
                throw new ArgumentException("First day of week should be a monday", nameof(firstDayOfWeek));
            }

            var nrOfDaysInWeek = 7;
            _labelPerActivityReportBuilder = new ActivityReportBuilder(repositoryFactory.Create($"label_activity_week_report_{firstDayOfWeek:yyyyMMdd}.csv", firstDayOfWeek, nrOfDaysInWeek), firstDayOfWeek, nrOfDaysInWeek);
        }

        public void Create(Dictionary<string, IEnumerable<LogEntry>> logEntriesPerLabel)
        {
            logEntriesPerLabel.Keys.ToList().ForEach(label =>
            {
                var logEntriesPerLabelPerDay = logEntriesPerLabel[label].GroupBy(x => x.CreateDate);

                _labelPerActivityReportBuilder.Build(label, logEntriesPerLabelPerDay);
            });
        }
    }
}
