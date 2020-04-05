using System;
using System.Collections.Generic;
using System.Linq;
using ManualTimeLogger.ReportBuilder.Persistence;
using NUnit.Framework;

namespace ManualTimeLogger.ReportBuilder.Tests
{
    [TestFixture]
    public class ReportEntryTests
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
        public void given_no_data_when_creating_report_entry_for_a_week_then_all_entries_have_zero_hours()
        {
            var entry = new ReportEntry(_engineer, _description, new DateTime(2020, 3, 2), 7, new Dictionary<DateTime, float>());
            Assert.AreEqual(7, entry.NrOfHoursPerDay.Count, "entries for nr of days");
            Assert.True(entry.NrOfHoursPerDay.Select(x => x.Value).All(nrOfHours => Math.Abs(nrOfHours) < 0.01f), "nr of hours value");
        }

        [Test]
        public void given_partial_data_when_creating_report_entry_for_a_week_then_the_hour_data_for_the_days_equal_the_provided_hour_data_and_the_unknown_days_have_zero_hours()
        {
            var monday = new DateTime(2020, 3, 2);
            var tuesday = monday.AddDays(1);
            var wednesday = monday.AddDays(2);
            var thursday = monday.AddDays(3);
            var hoursOnTuesday = 2f;
            var hoursOnThursday = 3f;
            var nrOfHoursPerDayInputData = new Dictionary<DateTime, float>{{ tuesday, hoursOnTuesday }, { thursday, hoursOnThursday } };
            var entry = new ReportEntry(_engineer, _description, monday, 7, nrOfHoursPerDayInputData);

            Assert.AreEqual(7, entry.NrOfHoursPerDay.Count, "entries for nr of days");
            Assert.True(Math.Abs(entry.NrOfHoursPerDay[monday]) < 0.01f, "nr of hours value on monday");
            Assert.True(Math.Abs(entry.NrOfHoursPerDay[tuesday] - nrOfHoursPerDayInputData[tuesday]) < 0.01f, "nr of hours value on tuesday");
            Assert.True(Math.Abs(entry.NrOfHoursPerDay[wednesday]) < 0.01f, "nr of hours value on wednesday");
            Assert.True(Math.Abs(entry.NrOfHoursPerDay[thursday] - nrOfHoursPerDayInputData[thursday]) < 0.01f, "nr of hours value on thursday");
        }

        [Test]
        public void given_partial_data_when_creating_report_entry_for_16_days_then_the_hour_data_for_the_days_equal_the_provided_hour_data_and_the_unknown_days_have_zero_hours()
        {
            var monday = new DateTime(2020, 3, 2);
            var tuesday = monday.AddDays(1);
            var wednesday = monday.AddDays(2);
            var thursday = monday.AddDays(3);
            var hoursOnTuesday = 2f;
            var hoursOnThursday = 3f;
            var nrOfHoursPerDayInputData = new Dictionary<DateTime, float> { { tuesday, hoursOnTuesday }, { thursday, hoursOnThursday } };
            var periodNrOfDays = 16;
            var entry = new ReportEntry(_engineer, _description, monday, periodNrOfDays, nrOfHoursPerDayInputData);

            Assert.AreEqual(periodNrOfDays, entry.NrOfHoursPerDay.Count, "entries for nr of days");
            Assert.True(Math.Abs(entry.NrOfHoursPerDay[monday]) < 0.01f, "nr of hours value on monday");
            Assert.True(Math.Abs(entry.NrOfHoursPerDay[tuesday] - nrOfHoursPerDayInputData[tuesday]) < 0.01f, "nr of hours value on tuesday");
            Assert.True(Math.Abs(entry.NrOfHoursPerDay[wednesday]) < 0.01f, "nr of hours value on wednesday");
            Assert.True(Math.Abs(entry.NrOfHoursPerDay[thursday] - nrOfHoursPerDayInputData[thursday]) < 0.01f, "nr of hours value on thursday");
        }
    }
}
