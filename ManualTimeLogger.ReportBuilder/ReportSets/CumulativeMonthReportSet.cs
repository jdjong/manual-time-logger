using System;
using System.Collections.Generic;
using System.Linq;
using ManualTimeLogger.Domain;
using ManualTimeLogger.ReportBuilder.Persistence;
using ManualTimeLogger.ReportBuilder.ReportBuilders;

// ReSharper disable PossibleMultipleEnumeration

namespace ManualTimeLogger.ReportBuilder.ReportSets
{
    public class CumulativeMonthReportSet
    {
        private readonly ActivityReportBuilder _activityCumulativeReportBuilder;
        private readonly LabelReportBuilder _labelCumulativeReportBuilder;

        public CumulativeMonthReportSet(IRepositoryFactory repositoryFactory, DateTime firstDayOfMonth)
        {
            if (firstDayOfMonth.Day != 1)
            {
                throw new ArgumentException("First day of month should be 1", nameof(firstDayOfMonth));
            }

            var nrOfDaysInMonth = DateTime.DaysInMonth(firstDayOfMonth.Year, firstDayOfMonth.Month);
            _activityCumulativeReportBuilder = new ActivityReportBuilder(repositoryFactory.Create($"cumulative_activity_month_report_{firstDayOfMonth:yyyyMMdd}.csv", firstDayOfMonth, nrOfDaysInMonth), firstDayOfMonth, nrOfDaysInMonth);
            _labelCumulativeReportBuilder = new LabelReportBuilder(repositoryFactory.Create($"cumulative_label_month_report_{firstDayOfMonth:yyyyMMdd}.csv", firstDayOfMonth, nrOfDaysInMonth), firstDayOfMonth, nrOfDaysInMonth);
        }
        
        public void Create(IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            _activityCumulativeReportBuilder.Build(logEntriesPerDay);
            _labelCumulativeReportBuilder.Build(logEntriesPerDay);
        }
    }
}
