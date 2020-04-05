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
    public class AccountReportBuilderTests
    {
        [Test]
        public void given_a_account_report_builder_and_some_log_entries_with_different_scenarios_when_creating_the_report_then_the_expected_report_entries_are_added()
        {
            var repository = Substitute.For<IRepository>();
            var firstDateOfReportPeriod = new DateTime(2020, 2, 14);
            var periodNrOfDays = 6;

            var builder = new AccountReportBuilder(repository, firstDateOfReportPeriod, periodNrOfDays);

            var firstDate = new DateTime(2020, 2, 15);
            var secondDate = new DateTime(2020, 2, 17);
            var thirdDate = new DateTime(2020, 2, 19);

            var account1 = "nwb";

            var logEntries = new List<LogEntry>
            {
                new LogEntry(12345, 1f, "description", "label1", "activity", account1, firstDate),
                new LogEntry(12345, 2f, "description", "label1", "activity", account1, secondDate),
                new LogEntry(12345, 3.25f, "description", "label2", "activity", account1, secondDate),
                new LogEntry(12345, 4f, "description", "label1", "activity", account1, thirdDate),
                new LogEntry(12345, 5.25f, "description", "label3", "activity", account1, thirdDate),
                new LogEntry(12345, 6f, "description", "label3", "activity", account1, thirdDate),
            };
            var timeForAccount1PerDay = new Dictionary<DateTime, float>
            {
                { firstDate, 1f },
                { secondDate, 5.25f },
                { thirdDate, 15.25f },
            };

            var engineerName = "TestEngineer";
            builder.Build(engineerName, logEntries.GroupBy(x => x.CreateDate));

            repository.Received(1).SaveReportEntry(new ReportEntry(engineerName, account1, firstDateOfReportPeriod, periodNrOfDays, timeForAccount1PerDay));
        }
    }
}
