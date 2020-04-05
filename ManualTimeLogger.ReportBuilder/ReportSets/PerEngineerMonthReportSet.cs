using System;
using System.Collections.Generic;
using System.Linq;
using ManualTimeLogger.Domain;
using ManualTimeLogger.ReportBuilder.ReportBuilders;

// ReSharper disable PossibleMultipleEnumeration

namespace ManualTimeLogger.ReportBuilder.ReportSets
{
    public class PerEngineerMonthReportSet
    {
        private readonly ActivityReportBuilder _activityPerEngineerReportBuilder;
        private readonly LabelReportBuilder _labelPerEngineerReportBuilder;
        private readonly AccountReportBuilder _accountReportBuilder;

        public PerEngineerMonthReportSet(string reportsBasePath, DateTime firstDayOfMonth, string accountFilter)
        {
            if (firstDayOfMonth.Day != 1)
            {
                throw new ArgumentException("First day of month should be 1", nameof(firstDayOfMonth));
            }

            var nrOfDaysInMonth = DateTime.DaysInMonth(firstDayOfMonth.Year, firstDayOfMonth.Month);
            _activityPerEngineerReportBuilder = new ActivityReportBuilder(new ReportCsvFileRepository(reportsBasePath, $"{accountFilter ?? "all"}_engineer_activity_month_report_{firstDayOfMonth:yyyyMMdd}.csv", firstDayOfMonth, nrOfDaysInMonth), firstDayOfMonth, nrOfDaysInMonth);
            _labelPerEngineerReportBuilder = new LabelReportBuilder(new ReportCsvFileRepository(reportsBasePath, $"{accountFilter ?? "all"}_engineer_label_month_report_{firstDayOfMonth:yyyyMMdd}.csv", firstDayOfMonth, nrOfDaysInMonth), firstDayOfMonth, nrOfDaysInMonth);
            _accountReportBuilder = new AccountReportBuilder(new ReportCsvFileRepository(reportsBasePath, $"{accountFilter ?? "all"}_engineer_customer_month_report_{firstDayOfMonth:yyyyMMdd}.csv", firstDayOfMonth, nrOfDaysInMonth), firstDayOfMonth, nrOfDaysInMonth);
        }

        public void Create(Dictionary<string, IEnumerable<LogEntry>> logEntriesPerEngineer)
        {
            logEntriesPerEngineer.Keys.ToList().ForEach(engineer =>
            {
                var logEntriesPerEngineerPerDay = logEntriesPerEngineer[engineer].GroupBy(x => x.CreateDate);

                _activityPerEngineerReportBuilder.Build(engineer, logEntriesPerEngineerPerDay);
                _labelPerEngineerReportBuilder.Build(engineer, logEntriesPerEngineerPerDay);
                _accountReportBuilder.Build(engineer, logEntriesPerEngineerPerDay);
            });
        }
    }
}
