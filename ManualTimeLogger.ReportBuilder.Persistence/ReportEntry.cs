using System;
using System.Collections.Generic;
using System.Linq;

namespace ManualTimeLogger.ReportBuilder.Persistence
{
    public class ReportEntry
    {
        public int PeriodNrOfDays { get; }
        public string GroupedBy { get; }
        public string ThenGroupedBy { get; }
        public Dictionary<DateTime, float> NrOfHoursPerDay { get; }

        public ReportEntry(string groupedBy, string thenGroupedBy, DateTime firstDateOfReportPeriod, int periodNrOfDays, Dictionary<DateTime, float> nrOfHoursForAllDaysPerDay)
        {
            PeriodNrOfDays = periodNrOfDays;
            GroupedBy = groupedBy;
            ThenGroupedBy = thenGroupedBy;
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
            return string.Equals(GroupedBy, other.GroupedBy) && string.Equals(ThenGroupedBy, other.ThenGroupedBy) && NrOfHoursPerDay.SequenceEqual(other.NrOfHoursPerDay) && PeriodNrOfDays == other.PeriodNrOfDays;
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
                var hashCode = (GroupedBy != null ? GroupedBy.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ThenGroupedBy != null ? ThenGroupedBy.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (NrOfHoursPerDay != null ? NrOfHoursPerDay.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ PeriodNrOfDays;
                return hashCode;
            }
        }
    }
}
