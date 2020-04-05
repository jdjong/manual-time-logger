namespace ManualTimeLogger.ReportBuilder.Persistence
{
    public interface IRepository
    {
        void SaveReportEntry(ReportEntry reportEntry);
    }
}