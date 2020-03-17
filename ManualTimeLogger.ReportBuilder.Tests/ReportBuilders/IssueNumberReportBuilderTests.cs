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
    public class IssueNumberReportBuilderTests
    {
        [Test]
        public void given_an_issue_number_report_builder_and_some_logged_issue_numbers_with_different_scenarios_when_creating_the_report_then_the_expected_report_entries_are_added()
        {
            var repository = Substitute.For<IReportCsvFileRepository>();
            var firstDateOfReportPeriod = new DateTime(2020, 2, 14);
            var periodNrOfDays = 6;
            var accountFilter = "nwb";

            var builder = new IssueNumberReportBuilder(repository, firstDateOfReportPeriod, periodNrOfDays, accountFilter);

            var firstDate = new DateTime(2020, 2, 15);
            var secondDate = new DateTime(2020, 2, 17);
            var thirdDate = new DateTime(2020, 2, 19);

            var firstIssueNumber = 12345;
            var secondIssueNumber = 123456;
            var thirdIssueNumber = 12345678;

            var logEntries = new List<LogEntry>
            {
                new LogEntry(firstIssueNumber, 1f, "description", "label", "activity", "nwb", firstDate),
                new LogEntry(firstIssueNumber, 2f, "description", "label", "activity", "nwb", secondDate),
                new LogEntry(secondIssueNumber, 3.25f, "description", "label", "activity", "nwb", secondDate),
                new LogEntry(firstIssueNumber, 4f, "description", "label", "activity", "nwb", thirdDate),
                new LogEntry(thirdIssueNumber, 5.25f, "description", "label", "activity", "nwb", thirdDate),
                new LogEntry(thirdIssueNumber, 6f, "description", "label", "activity", "nb", thirdDate),
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

            repository.Received(1).SaveReportEntry(new ReportEntry(engineerName, firstIssueNumber.ToString(), firstDateOfReportPeriod, periodNrOfDays, timeForActivity1PerDay));
            repository.Received(1).SaveReportEntry(new ReportEntry(engineerName, secondIssueNumber.ToString(), firstDateOfReportPeriod, periodNrOfDays, timeForActivity2PerDay));
            repository.Received(1).SaveReportEntry(new ReportEntry(engineerName, thirdIssueNumber.ToString(), firstDateOfReportPeriod, periodNrOfDays, timeForActivity3PerDay));
        }
    }
}
