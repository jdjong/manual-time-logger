namespace ManualTimeLogger.ReportBuilder
{
    public interface IRepository
    {
        void SaveReportEntry(ReportEntry reportEntry);
    }
}