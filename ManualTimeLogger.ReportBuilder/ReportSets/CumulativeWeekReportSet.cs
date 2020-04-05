using System;
using System.Collections.Generic;
using System.Linq;
using ManualTimeLogger.Domain;
using ManualTimeLogger.ReportBuilder.Persistence;
using ManualTimeLogger.ReportBuilder.ReportBuilders;

// ReSharper disable PossibleMultipleEnumeration

namespace ManualTimeLogger.ReportBuilder.ReportSets
{
    public class CumulativeWeekReportSet
    {
        private readonly ActivityReportBuilder _activityCumulativeReportBuilder;
        private readonly LabelReportBuilder _labelCumulativeReportBuilder;

        public CumulativeWeekReportSet(IRepositoryFactory repositoryFactory, DateTime firstDayOfWeek)
        {
            if (firstDayOfWeek.DayOfWeek != DayOfWeek.Monday)
            {
                throw new ArgumentException("First day of week should be a monday", nameof(firstDayOfWeek));
            }

            var nrOfDaysInWeek = 7;
            _activityCumulativeReportBuilder = new ActivityReportBuilder(repositoryFactory.Create($"cumulative_activity_week_report_{firstDayOfWeek:yyyyMMdd}.csv", firstDayOfWeek, nrOfDaysInWeek), firstDayOfWeek, nrOfDaysInWeek);
            _labelCumulativeReportBuilder = new LabelReportBuilder(repositoryFactory.Create($"cumulative_label_week_report_{firstDayOfWeek:yyyyMMdd}.csv", firstDayOfWeek, nrOfDaysInWeek), firstDayOfWeek, nrOfDaysInWeek);
        }

        public void Create(IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            _activityCumulativeReportBuilder.Build(logEntriesPerDay);
            _labelCumulativeReportBuilder.Build(logEntriesPerDay);
        }
    }
}
