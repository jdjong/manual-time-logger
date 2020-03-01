using System;
using ManualTimeLogger.ReportBuilder.Commands;
using NUnit.Framework;

// ReSharper disable ObjectCreationAsStatement

namespace ManualTimeLogger.ReportBuilder.Tests.Commands
{
    [TestFixture]
    public class BuildReportsCommandsTests
    {
        [Test]
        public void given_a_non_monday_date_when_initializing_week_command_then_an_argument_exception_is_thrown()
        {
            Assert.Throws<ArgumentException>(() => new BuildWeekReportsCommand(new DateTime(2020,2,29))); // a saturday
        }

        [Test]
        public void given_a_non_first_day_of_the_month_date_when_initializing_month_command_then_an_argument_exception_is_thrown()
        {
            Assert.Throws<ArgumentException>(() => new BuildMonthReportsCommand(new DateTime(2020, 2, 29)));
        }

        [Test]
        public void given_a_monday_date_when_initializing_week_command_then_week_command_is_created()
        {
            new BuildWeekReportsCommand(new DateTime(2020, 3, 2)); // a monday
        }

        [Test]
        public void given_a_first_day_of_the_month_date_when_initializing_month_command_then_month_command_is_created()
        {
            new BuildMonthReportsCommand(new DateTime(2020, 3, 1));
        }
    }
}
