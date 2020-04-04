namespace ManualTimeLogger.ReportBuilder
{
    public interface IReportCsvFileRepository
    {
        void SaveReportEntry(ReportEntry reportEntry);
    }
}