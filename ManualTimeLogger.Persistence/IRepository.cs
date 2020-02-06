﻿using System;
using ManualTimeLogger.Domain;

namespace ManualTimeLogger.Persistence
{
    public interface IRepository
    {
        void SaveLogEntry(LogEntry logEntry);
        float GetTotalLoggedHoursForDate(DateTime date);
    }
}
