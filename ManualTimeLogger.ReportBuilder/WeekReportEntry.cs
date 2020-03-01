﻿using System;
using System.Collections.Generic;

namespace ManualTimeLogger.ReportBuilder
{
    // TODO, to random number of days report entry
    public class WeekReportEntry
    {
        public string Engineer;
        public string Description;
        public Dictionary<DateTime, float> NrOfHoursPerWeekDay;

        public WeekReportEntry(string engineer, string description, DateTime dateOfMondayOfRequestedWeek, Dictionary<DateTime, float> nrOfHoursForAllDaysPerDay)
        {
            if (dateOfMondayOfRequestedWeek.DayOfWeek != DayOfWeek.Monday)
            {
                throw new ArgumentException("Date needs to be a monday", nameof(dateOfMondayOfRequestedWeek));
            }
            
            Engineer = engineer;
            Description = description;
            NrOfHoursPerWeekDay = new Dictionary<DateTime, float>();

            for (var numberOfDays = 0; numberOfDays < 7; numberOfDays++)
            {
                var currentDay = dateOfMondayOfRequestedWeek.AddDays(numberOfDays);
                NrOfHoursPerWeekDay.Add(currentDay,
                    nrOfHoursForAllDaysPerDay?.ContainsKey(currentDay) ?? false
                        ? nrOfHoursForAllDaysPerDay[currentDay]
                        : 0f);
            }
        }
    }
}
