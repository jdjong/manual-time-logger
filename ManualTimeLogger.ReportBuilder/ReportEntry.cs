using System;
using System.Collections.Generic;

namespace ManualTimeLogger.ReportBuilder
{
    public class ReportEntry
    {
        public int PeriodNrOfDays { get; }
        public string Engineer;
        public string Description;
        public Dictionary<DateTime, float> NrOfHoursPerDay;

        public ReportEntry(string engineer, string description, DateTime firstDateOfReportPeriod, int periodNrOfDays, Dictionary<DateTime, float> nrOfHoursForAllDaysPerDay)
        {
            PeriodNrOfDays = periodNrOfDays;
            Engineer = engineer;
            Description = description;
            NrOfHoursPerDay = new Dictionary<DateTime, float>();

            for (var numberOfDays = 0; numberOfDays < periodNrOfDays; numberOfDays++)
            {
                var currentDay = firstDateOfReportPeriod.AddDays(numberOfDays);
                NrOfHoursPerDay.Add(currentDay,
                    nrOfHoursForAllDaysPerDay?.ContainsKey(currentDay) ?? false
                        ? nrOfHoursForAllDaysPerDay[currentDay]
                        : 0f);
            }
        }
    }
}
