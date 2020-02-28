using System;
using System.Collections.Generic;

namespace ManualTimeLogger.ReportBuilder
{
    public class ReportWeekEntry
    {
        public string Engineer;
        public string Description;
        public Dictionary<DateTime, float> NrOfHoursPerWeekDay;

        // TODO, unit test
        public ReportWeekEntry(string engineer, string description, DateTime aDateForTheRequestedWeek, Dictionary<DateTime, float> nrOfHoursForAllDaysPerDay)
        {
            var dateOfMondayForRequestedWeek = GetMondayDateForRequestedWeek(aDateForTheRequestedWeek);

            Engineer = engineer;
            Description = description;
            NrOfHoursPerWeekDay = new Dictionary<DateTime, float>();

            for (var numberOfDays = 0; numberOfDays < 7; numberOfDays++)
            {
                var currentDay = dateOfMondayForRequestedWeek.AddDays(numberOfDays);
                NrOfHoursPerWeekDay.Add(currentDay,
                    nrOfHoursForAllDaysPerDay?.ContainsKey(currentDay) ?? false
                        ? nrOfHoursForAllDaysPerDay[currentDay]
                        : 0f);
            }
        }

        private DateTime GetMondayDateForRequestedWeek(DateTime aDateForTheRequestedWeek)
        {
            var dateIterator = aDateForTheRequestedWeek;
            while (dateIterator.DayOfWeek != DayOfWeek.Monday)
            {
                dateIterator = dateIterator.AddDays(-1);
            }

            return dateIterator;
        }
    }
}
