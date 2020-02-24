using System;

namespace ManualTimeLogger.Domain
{
    public class LogEntry
    {
        public int IssueNumber { get; }
        /// <summary>
        /// Duration in hours
        /// </summary>
        public float Duration { get; }
        public string Description { get; }
        public string Label { get; }
        public Activity Activity { get; }
        public DateTime CreateDate { get; }

        /// <summary>
        /// Represents a log entry
        /// </summary>
        /// <param name="issueNumber"></param>
        /// <param name="duration"></param>
        /// <param name="description"></param>
        /// <param name="label"></param>
        /// <param name="activity"></param>
        /// <param name="createDate"></param>
        public LogEntry(int issueNumber, float duration, string description, string label, Activity activity, DateTime createDate)
        {
            if (issueNumber < 0)
            {
                throw new ArgumentException($"Only positive issue numbers are allowed (current: {issueNumber})", nameof(issueNumber));
            }

            if (duration <= 0)
            {
                throw new ArgumentException($"Duration should be greater than zero (current: {duration})", nameof(duration));
            }

            if (string.IsNullOrEmpty(description))
            {
                throw new ArgumentNullException(nameof(description));
            }

            if (createDate.Date == DateTime.MinValue)
            {
                throw new ArgumentException($"Create date is not set; it equals min value", nameof(createDate));
            }

            IssueNumber = issueNumber;
            Duration = duration;
            Description = description;
            Label = label;
            Activity = activity;
            CreateDate = createDate;
        }

        protected bool Equals(LogEntry other)
        {
            return IssueNumber == other.IssueNumber && Duration.Equals(other.Duration) && string.Equals(Description, other.Description) && string.Equals(Label, other.Label) && CreateDate.Equals(other.CreateDate);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LogEntry) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = IssueNumber;
                hashCode = (hashCode * 397) ^ Duration.GetHashCode();
                hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Label != null ? Label.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Activity.GetHashCode();
                hashCode = (hashCode * 397) ^ CreateDate.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(LogEntry left, LogEntry right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(LogEntry left, LogEntry right)
        {
            return !Equals(left, right);
        }
    }
}