using System;
using System.Collections.Generic;
using System.Linq;

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

        protected bool Equals(ReportEntry other)
        {
            return string.Equals(Engineer, other.Engineer) && string.Equals(Description, other.Description) && NrOfHoursPerDay.SequenceEqual(other.NrOfHoursPerDay) && PeriodNrOfDays == other.PeriodNrOfDays;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ReportEntry) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Engineer != null ? Engineer.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (NrOfHoursPerDay != null ? NrOfHoursPerDay.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ PeriodNrOfDays;
                return hashCode;
            }
        }
    }
}
