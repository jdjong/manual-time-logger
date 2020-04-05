using System;
using System.Collections.Generic;
using System.Linq;
using ManualTimeLogger.Domain;
using ManualTimeLogger.ReportBuilder.Persistence;
using ManualTimeLogger.ReportBuilder.ReportBuilders;

// ReSharper disable PossibleMultipleEnumeration

namespace ManualTimeLogger.ReportBuilder.ReportSets
{
    public class PerLabelMonthReportSet
    {
        private readonly ActivityReportBuilder _labelPerActivityReportBuilder;

        public PerLabelMonthReportSet(IRepositoryFactory repositoryFactory, DateTime firstDayOfMonth)
        {
            if (firstDayOfMonth.Day != 1)
            {
                throw new ArgumentException("First day of month should be 1", nameof(firstDayOfMonth));
            }

            var nrOfDaysInMonth = DateTime.DaysInMonth(firstDayOfMonth.Year, firstDayOfMonth.Month);
            _labelPerActivityReportBuilder = new ActivityReportBuilder(repositoryFactory.Create($"label_activity_month_report_{firstDayOfMonth:yyyyMMdd}.csv", firstDayOfMonth, nrOfDaysInMonth), firstDayOfMonth, nrOfDaysInMonth);
        }

        public void Create(Dictionary<string, IEnumerable<LogEntry>> logEntriesPerLabel)
        {
            logEntriesPerLabel.Keys.ToList().ForEach(label =>
            {
                var logEntriesPerActivityPerDay = logEntriesPerLabel[label].GroupBy(x => x.CreateDate);
                _labelPerActivityReportBuilder.Build(label, logEntriesPerActivityPerDay);
            });
        }
    }
}
