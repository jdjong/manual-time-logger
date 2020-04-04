using System;
using System.Collections.Generic;
using System.Linq;
using ManualTimeLogger.Domain;
using ManualTimeLogger.ReportBuilder.ReportBuilders;

// ReSharper disable PossibleMultipleEnumeration

namespace ManualTimeLogger.ReportBuilder.ReportsBuilders
{
    public class CumulativeMonthReportsBuilder
    {
        private readonly IEnumerable<IGrouping<DateTime, LogEntry>> _logEntriesPerDay;

        private readonly ActivityReportBuilder _activityCumulativeReportBuilder;
        private readonly LabelReportBuilder _labelCumulativeReportBuilder;

        public CumulativeMonthReportsBuilder(string reportsBasePath, DateTime firstDayOfMonth, string accountFilter, IEnumerable<IGrouping<DateTime, LogEntry>> logEntriesPerDay)
        {
            if (firstDayOfMonth.Day != 1)
            {
                throw new ArgumentException("First day of month should be 1", nameof(firstDayOfMonth));
            }

            _logEntriesPerDay = logEntriesPerDay;

            var nrOfDaysInMonth = DateTime.DaysInMonth(firstDayOfMonth.Year, firstDayOfMonth.Month);
            _activityCumulativeReportBuilder = new ActivityReportBuilder(new ReportCsvFileRepository(reportsBasePath, $"{accountFilter ?? "all"}_cumulative_activity_month_report_{firstDayOfMonth:yyyyMMdd}.csv", firstDayOfMonth, nrOfDaysInMonth), firstDayOfMonth, nrOfDaysInMonth, accountFilter);
            _labelCumulativeReportBuilder = new LabelReportBuilder(new ReportCsvFileRepository(reportsBasePath, $"{accountFilter ?? "all"}_cumulative_label_month_report_{firstDayOfMonth:yyyyMMdd}.csv", firstDayOfMonth, nrOfDaysInMonth), firstDayOfMonth, nrOfDaysInMonth, accountFilter);
        }
        
        public void Build()
        {
            _activityCumulativeReportBuilder.Build(_logEntriesPerDay);
            _labelCumulativeReportBuilder.Build(_logEntriesPerDay);
        }
    }
}
