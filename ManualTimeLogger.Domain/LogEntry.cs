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
        public DateTimeOffset CreateDate { get; }

        /// <summary>
        /// Represents a log entry
        /// </summary>
        /// <param name="issueNumber"></param>
        /// <param name="duration"></param>
        /// <param name="description"></param>
        /// <param name="createDate"></param>
        public LogEntry(int issueNumber, float duration, string description, DateTimeOffset createDate)
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

            if (createDate.Date == DateTimeOffset.MinValue)
            {
                throw new ArgumentException($"Create date is not set; it equals min value", nameof(createDate));
            }

            IssueNumber = issueNumber;
            Duration = duration;
            Description = description;
            CreateDate = createDate;
        }
    }
}