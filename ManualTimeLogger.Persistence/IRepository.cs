using System;
using System.Collections.Generic;
using ManualTimeLogger.Domain;

namespace ManualTimeLogger.Persistence
{
    public interface IRepository
    {
        void SaveLogEntry(LogEntry logEntry);
        float GetTotalLoggedHoursForDate(DateTime date);
        IEnumerable<LogEntry> GetAllLogEntries();
        string GetEngineerName();
    }
}
