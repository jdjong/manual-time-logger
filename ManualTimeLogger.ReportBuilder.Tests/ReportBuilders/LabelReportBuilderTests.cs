using System;
using System.Collections.Generic;
using System.Linq;
using ManualTimeLogger.Domain;
using ManualTimeLogger.ReportBuilder.ReportBuilders;
using NSubstitute;
using NUnit.Framework;

namespace ManualTimeLogger.ReportBuilder.Tests.ReportBuilders
{
    [TestFixture]
    public class LabelReportBuilderTests
    {
        [Test]
        public void given_a_label_report_builder_and_some_logged_labels_with_different_scenarios_when_creating_the_report_then_the_expected_report_entries_are_added()
        {
            var repository = Substitute.For<IReportCsvFileRepository>();
            var firstDateOfReportPeriod = new DateTime(2020, 2, 14);
            var periodNrOfDays = 6;

            var builder = new LabelReportBuilder(repository, firstDateOfReportPeriod, periodNrOfDays);

            var firstDate = new DateTime(2020, 2, 15);
            var secondDate = new DateTime(2020, 2, 17);
            var thirdDate = new DateTime(2020, 2, 20);

            var firstLabel = "label1";
            var secondLabel = "label2";
            var thirdLabel = "label3";

            var logEntries = new List<LogEntry>
            {
                new LogEntry(12345, 1f, "description", firstLabel, "activity", firstDate),
                new LogEntry(12345, 2f, "description", firstLabel, "activity", secondDate),
                new LogEntry(12345, 3.25f, "description", secondLabel, "activity", secondDate),
                new LogEntry(12345, 4f, "description", firstLabel, "activity", thirdDate),
                new LogEntry(12345, 5.25f, "description", thirdLabel, "activity", thirdDate),
                new LogEntry(12345, 6f, "description", thirdLabel, "activity", thirdDate),
            };
            var timeForActivity1PerDay = new Dictionary<DateTime, float>
            {
                { firstDate, 1f },
                { secondDate, 2f },
                { thirdDate, 4f },
            };
            var timeForActivity2PerDay = new Dictionary<DateTime, float>
            {
                { secondDate, 3.25f },
            };
            var timeForActivity3PerDay = new Dictionary<DateTime, float>
            {
                { thirdDate, 11.25f },
            };

            var engineerName = "TestEngineer";
            builder.Build(engineerName, logEntries.GroupBy(x => x.CreateDate));

            repository.Received(1).SaveReportEntry(new ReportEntry(engineerName, firstLabel, firstDateOfReportPeriod, periodNrOfDays, timeForActivity1PerDay));
            repository.Received(1).SaveReportEntry(new ReportEntry(engineerName, secondLabel, firstDateOfReportPeriod, periodNrOfDays, timeForActivity2PerDay));
            repository.Received(1).SaveReportEntry(new ReportEntry(engineerName, thirdLabel, firstDateOfReportPeriod, periodNrOfDays, timeForActivity3PerDay));
        }
    }
}
