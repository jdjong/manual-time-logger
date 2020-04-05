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
    public class ActivityReportBuilderTests
    {
        [Test]
        public void given_an_activity_report_builder_and_some_logged_activities_with_different_scenarios_when_creating_the_report_then_the_expected_report_entries_are_added()
        {
            var repository = Substitute.For<IRepository>();
            var firstDateOfReportPeriod = new DateTime(2020, 2, 14);
            var periodNrOfDays = 6;
            var accountFilter = "nwb";

            var builder = new ActivityReportBuilder(repository, firstDateOfReportPeriod, periodNrOfDays);

            var firstDate = new DateTime(2020, 2, 15);
            var secondDate = new DateTime(2020, 2, 17);
            var thirdDate = new DateTime(2020, 2, 19);

            var firstActivity = "activity1";
            var secondActivity = "activity2";
            var thirdActivity = "activity3";

            var logEntries = new List<LogEntry>
            {
                new LogEntry(12345, 1f, "description", "label", firstActivity, "nwb", firstDate),
                new LogEntry(12345, 2f, "description", "label", firstActivity, "nwb", secondDate),
                new LogEntry(12345, 3.25f, "description", "label", secondActivity, "nwb", secondDate),
                new LogEntry(12345, 4f, "description", "label", firstActivity, "nwb", thirdDate),
                new LogEntry(12345, 5.25f, "description", "label", thirdActivity, "nwb", thirdDate),
                new LogEntry(12345, 6f, "description", "label", thirdActivity, "nb", thirdDate),
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
                { thirdDate, 5.25f },
            };

            var engineerName = "TestEngineer";
            builder.Build(engineerName, logEntries.GroupBy(x => x.CreateDate));

            repository.Received(1).SaveReportEntry(new ReportEntry(engineerName, firstActivity, firstDateOfReportPeriod, periodNrOfDays, timeForActivity1PerDay));
            repository.Received(1).SaveReportEntry(new ReportEntry(engineerName, secondActivity, firstDateOfReportPeriod, periodNrOfDays, timeForActivity2PerDay));
            repository.Received(1).SaveReportEntry(new ReportEntry(engineerName, thirdActivity, firstDateOfReportPeriod, periodNrOfDays, timeForActivity3PerDay));
        }
    }
}
