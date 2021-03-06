﻿using System;
using ManualTimeLogger.ReportBuilder.Commands;
using NUnit.Framework;

namespace ManualTimeLogger.ReportBuilder.Tests.Commands
{
    [TestFixture]
    public class CommandProviderTests
    {
        [Test]
        public void given_no_arguments_when_determining_report_command_then_the_default_command_is_returned()
        {
            var commandProvider = new CommandProvider();
            var command = commandProvider.GetCommand(null);

            Assert.IsInstanceOf(typeof(BuildWeekReportsCommand), command, "Type of command");
            Assert.IsTrue(DateTime.Today - command.FromDay < TimeSpan.FromDays(7), "Default command is for the current week");
        }

        [Test]
        public void given_wrong_number_of_arguments_when_determining_report_command_then_an_argument_exception_is_thrown()
        {
            var commandProvider = new CommandProvider();

            Assert.Throws<ArgumentException>(() => commandProvider.GetCommand(new []{"1", "2", "3"}));
        }

        [Test]
        [TestCase("19990202")]
        [TestCase("20100002")]
        [TestCase("20101302")]
        [TestCase("20100200")]
        [TestCase("20100232")]
        public void given_wrong_date_input_when_determining_report_command_then_an_argument_exception_is_thrown(string dateInput)
        {
            var commandProvider = new CommandProvider();

            Assert.Throws<ArgumentException>(() => commandProvider.GetCommand(new[] { "-w", dateInput }));
        }

        [Test]
        public void given_wrong_command_input_when_determining_report_command_then_an_argument_exception_is_thrown()
        {
            var commandProvider = new CommandProvider();

            Assert.Throws<ArgumentException>(() => commandProvider.GetCommand(new[] { "-f", "20100202" }));
        }

        [Test]
        public void given_correct_week_command_and_date_input_when_determining_report_command_then_the_week_command_with_the_correct_first_period_date_is_returned()
        {
            var commandProvider = new CommandProvider();
            var command = commandProvider.GetCommand(new[] { "-w", "20200229" }); // saturday

            Assert.IsInstanceOf(typeof(BuildWeekReportsCommand), command, "Type of command");
            Assert.AreEqual(new DateTime(2020, 2, 24), command.FromDay, "Command first date of period");
        }

        [Test]
        public void given_correct_month_command_and_date_input_when_determining_report_command_then_the_month_command_with_the_correct_first_period_date_is_returned()
        {
            var commandProvider = new CommandProvider();
            var command = commandProvider.GetCommand(new[] { "-m", "20200229" }); // saturday

            Assert.IsInstanceOf(typeof(BuildMonthReportsCommand), command, "Type of command");
            Assert.AreEqual(new DateTime(2020, 2, 1), command.FromDay, "Command first date of period");
        }

        [Test]
        [TestCase("-m", "20200229", "-c", "nwb")]
        [TestCase("-c", "nwb", "-m", "20200229")]
        public void given_correct_account_filter_command_when_determining_report_command_then_the_account_filter_is_added(params string[] args)
        {
            var commandProvider = new CommandProvider();
            var command = commandProvider.GetCommand(args); // saturday

            Assert.IsInstanceOf(typeof(BuildMonthReportsCommand), command, "Type of command");
            Assert.AreEqual("nwb", command.AccountFilter, "Command account filter");
        }
    }
}
