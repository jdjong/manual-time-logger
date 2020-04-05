using System;

namespace ManualTimeLogger.ReportBuilder.Persistence
{
    public interface IRepositoryFactory
    {
        IRepository Create(string fileName, DateTime reportStartDate, int reportNrOfDays);
    }
}