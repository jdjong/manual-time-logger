namespace ManualTimeLogger.ReportBuilder
{
    public interface IReportCsvFileRepository
    {
        char CsvSeparator { get; }
        void CreateHeader(string[] header);
        void SaveReportEntry(ReportEntry reportEntry);
    }
}