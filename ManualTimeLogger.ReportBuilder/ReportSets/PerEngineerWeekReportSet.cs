using System;
using System.Collections.Generic;
using System.Linq;
using ManualTimeLogger.Domain;
using ManualTimeLogger.ReportBuilder.ReportBuilders;

// ReSharper disable PossibleMultipleEnumeration

namespace ManualTimeLogger.ReportBuilder.ReportSets
{
    public class PerEngineerWeekReportSet
    {
        private readonly ActivityReportBuilder _activityPerEngineerReportBuilder;
        private readonly LabelReportBuilder _labelPerEngineerReportBuilder;

        public PerEngineerWeekReportSet(string reportsBasePath, DateTime firstDayOfWeek, string accountFilter)
        {
            if (firstDayOfWeek.DayOfWeek != DayOfWeek.Monday)
            {
                throw new ArgumentException("First day of week should be a monday", nameof(firstDayOfWeek));
            }

            var nrOfDaysInWeek = 7;
            _activityPerEngineerReportBuilder = new ActivityReportBuilder(new ReportCsvFileRepository(reportsBasePath, $"{accountFilter ?? "all"}_engineer_activity_week_report_{firstDayOfWeek:yyyyMMdd}.csv", firstDayOfWeek, nrOfDaysInWeek), firstDayOfWeek, nrOfDaysInWeek);
            _labelPerEngineerReportBuilder = new LabelReportBuilder(new ReportCsvFileRepository(reportsBasePath, $"{accountFilter ?? "all"}_engineer_label_week_report_{firstDayOfWeek:yyyyMMdd}.csv", firstDayOfWeek, nrOfDaysInWeek), firstDayOfWeek, nrOfDaysInWeek);
        }

        public void Create(Dictionary<string, IEnumerable<LogEntry>> logEntriesPerEngineer)
        {
            logEntriesPerEngineer.Keys.ToList().ForEach(engineer =>
            {
                var logEntriesPerEngineerPerDay = logEntriesPerEngineer[engineer].GroupBy(x => x.CreateDate);

                _activityPerEngineerReportBuilder.Build(engineer, logEntriesPerEngineerPerDay);
                _labelPerEngineerReportBuilder.Build(engineer, logEntriesPerEngineerPerDay);
            });
        }
    }
}
