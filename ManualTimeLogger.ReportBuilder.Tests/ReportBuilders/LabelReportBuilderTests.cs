using System;
using System.Collections.Generic;
using System.Linq;
using ManualTimeLogger.Domain;
using ManualTimeLogger.ReportBuilder.Persistence;
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
            var repository = Substitute.For<IRepository>();
            var firstDateOfReportPeriod = new DateTime(2020, 2, 14);
            var periodNrOfDays = 6;

            var builder = new LabelReportBuilder(repository, firstDateOfReportPeriod, periodNrOfDays);

            var firstDate = new DateTime(2020, 2, 15);
            var secondDate = new DateTime(2020, 2, 17);
            var thirdDate = new DateTime(2020, 2, 19);

            var firstLabel = "label1";
            var secondLabel = "label2";
            var thirdLabel = "label3";

            var logEntries = new List<LogEntry>
            {
                new LogEntry(12345, 1f, "description", firstLabel, "activity", "nwb", firstDate),
                new LogEntry(12345, 2f, "description", firstLabel, "activity", "nwb", secondDate),
                new LogEntry(12345, 3.25f, "description", secondLabel, "activity", "nwb", secondDate),
                new LogEntry(12345, 4f, "description", firstLabel, "activity", "nwb", thirdDate),
                new LogEntry(12345, 5.25f, "description", thirdLabel, "activity", "nwb", thirdDate),
                new LogEntry(12345, 6f, "description", thirdLabel, "activity", "nb", thirdDate),
            };
            var timeForLabel1PerDay = new Dictionary<DateTime, float>
            {
                { firstDate, 1f },
                { secondDate, 2f },
                { thirdDate, 4f },
            };
            var timeForLabel2PerDay = new Dictionary<DateTime, float>
            {
                { secondDate, 3.25f },
            };
            var timeForLabel3PerDay = new Dictionary<DateTime, float>
            {
                { thirdDate, 11.25f },
            };

            var engineerName = "TestEngineer";
            builder.Build(engineerName, logEntries.GroupBy(x => x.CreateDate));

            repository.Received(1).SaveReportEntry(new ReportEntry(engineerName, firstLabel, firstDateOfReportPeriod, periodNrOfDays, timeForLabel1PerDay));
            repository.Received(1).SaveReportEntry(new ReportEntry(engineerName, secondLabel, firstDateOfReportPeriod, periodNrOfDays, timeForLabel2PerDay));
            repository.Received(1).SaveReportEntry(new ReportEntry(engineerName, thirdLabel, firstDateOfReportPeriod, periodNrOfDays, timeForLabel3PerDay));
        }

        [Test]
        public void given_a_label_report_builder_and_some_logged_labels_with_different_scenarios_and_account_filter_is_set_when_creating_the_report_then_the_expected_report_entries_are_added()
        {
            var repository = Substitute.For<IRepository>();
            var firstDateOfReportPeriod = new DateTime(2020, 2, 14);
            var periodNrOfDays = 6;
            var accountFilter = "nwb";

            var builder = new LabelReportBuilder(repository, firstDateOfReportPeriod, periodNrOfDays);

            var firstDate = new DateTime(2020, 2, 15);
            var secondDate = new DateTime(2020, 2, 17);
            var thirdDate = new DateTime(2020, 2, 19);

            var firstLabel = "label1";
            var secondLabel = "label2";
            var thirdLabel = "label3";

            var logEntries = new List<LogEntry>
            {
                new LogEntry(12345, 1f, "description", firstLabel, "activity", "nwb", firstDate),
                new LogEntry(12345, 2f, "description", firstLabel, "activity", "nwb", secondDate),
                new LogEntry(12345, 3.25f, "description", secondLabel, "activity", "nwb", secondDate),
                new LogEntry(12345, 4f, "description", firstLabel, "activity", "nwb", thirdDate),
                new LogEntry(12345, 5.25f, "description", thirdLabel, "activity", "nwb", thirdDate),
                new LogEntry(12345, 6f, "description", thirdLabel, "activity", "nb", thirdDate),
            };
            var timeForLabel1PerDay = new Dictionary<DateTime, float>
            {
                { firstDate, 1f },
                { secondDate, 2f },
                { thirdDate, 4f },
            };
            var timeForLabel2PerDay = new Dictionary<DateTime, float>
            {
                { secondDate, 3.25f },
            };
            var timeForLabel3PerDay = new Dictionary<DateTime, float>
            {
                { thirdDate, 5.25f },
            };

            var engineerName = "TestEngineer";
            builder.Build(engineerName, logEntries.GroupBy(x => x.CreateDate));

            repository.Received(1).SaveReportEntry(new ReportEntry(engineerName, thirdLabel, firstDateOfReportPeriod, periodNrOfDays, timeForLabel3PerDay));
        }
    }
}
