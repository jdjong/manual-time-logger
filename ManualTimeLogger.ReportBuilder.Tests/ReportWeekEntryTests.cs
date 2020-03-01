using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ManualTimeLogger.ReportBuilder.Tests
{
    [TestFixture]
    public class ReportWeekEntryTests
    {
        private string _engineer;
        private string _description;

        [SetUp]
        public void SetUp()
        {
            _engineer = "TestEngineer";
            _description = "Description";
        }
        
        [Test]
        public void given_a_non_monday_date_when_creating_report_week_entry_then_an_argument_exception_is_thrown()
        {
            Assert.Throws<ArgumentException>(() => new ReportWeekEntry(_engineer, _description, new DateTime(2020, 2, 29), new Dictionary<DateTime, float>())); // a saturday
        }

        [Test]
        public void given_no_data_when_creating_report_week_entry_then_all_week_entries_have_zero_hours()
        {
            var entry = new ReportWeekEntry(_engineer, _description, new DateTime(2020, 3, 2), new Dictionary<DateTime, float>());
            Assert.AreEqual(7, entry.NrOfHoursPerWeekDay.Count, "entries for nr of days");
            Assert.True(entry.NrOfHoursPerWeekDay.Select(x => x.Value).All(nrOfHours => Math.Abs(nrOfHours) < 0.01f), "nr of hours value");
        }

        [Test]
        public void given_partial_data_when_creating_report_week_entry_then_the_hour_data_for_the_days_equal_the_provided_hour_data_and_the_unknown_days_have_zero_hours()
        {
            var monday = new DateTime(2020, 3, 2);
            var tuesday = monday.AddDays(1);
            var wednesday = monday.AddDays(2);
            var thursday = monday.AddDays(3);
            var hoursOnTuesday = 2f;
            var hoursOnThursday = 3f;
            var nrOfHoursPerDayInputData = new Dictionary<DateTime, float>{{ tuesday, hoursOnTuesday }, { thursday, hoursOnThursday } };
            var entry = new ReportWeekEntry(_engineer, _description, monday, nrOfHoursPerDayInputData);

            Assert.AreEqual(7, entry.NrOfHoursPerWeekDay.Count, "entries for nr of days");
            Assert.True(Math.Abs(entry.NrOfHoursPerWeekDay[monday]) < 0.01f, "nr of hours value on monday");
            Assert.True(Math.Abs(entry.NrOfHoursPerWeekDay[tuesday] - nrOfHoursPerDayInputData[tuesday]) < 0.01f, "nr of hours value on tuesday");
            Assert.True(Math.Abs(entry.NrOfHoursPerWeekDay[wednesday]) < 0.01f, "nr of hours value on wednesday");
            Assert.True(Math.Abs(entry.NrOfHoursPerWeekDay[thursday] - nrOfHoursPerDayInputData[thursday]) < 0.01f, "nr of hours value on thursday");
        }
    }
}
