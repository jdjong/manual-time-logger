using System.IO;
using System.Linq;
using ManualTimeLogger.Persistence;
using ManualTimeLogger.ReportBuilder.Commands;

namespace ManualTimeLogger.ReportBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            var reportingCommandProvider = new CommandProvider();
            Handle((dynamic)reportingCommandProvider.GetCommand(args));
        }

        private static void Handle(BuildWeekReportsCommand command)
        {
            var timeLogsBasePath = Properties.Settings.Default.TimeLogsBasePath;
            var allTimeLogFileNames = Directory.EnumerateFiles(timeLogsBasePath);
            var allTimeLogRepositories = allTimeLogFileNames.Select(filePaths => new CsvFileRepository(timeLogsBasePath, Path.GetFileName(filePaths))).ToList();

            var logEntriesPerEngineer = allTimeLogRepositories.ToDictionary(repository => repository.GetEngineerName(), repository => repository.GetAllLogEntries());
            var logEntriesPerDay = logEntriesPerEngineer.SelectMany(x => x.Value).GroupBy(x => x.CreateDate);

            var perEngineerReportsBuilder = new PerEngineerWeekReportsBuilder(Properties.Settings.Default.ReportsBasePaths, command.FirstDayOfPeriod, logEntriesPerEngineer);
            var cumulativeReportsBuilder = new CumulativeWeekReportsBuilder(Properties.Settings.Default.ReportsBasePaths, command.FirstDayOfPeriod, logEntriesPerDay);
            
            perEngineerReportsBuilder.Build();
            cumulativeReportsBuilder.Build();
        }

        private static void Handle(BuildMonthReportsCommand command)
        {
            var timeLogsBasePath = Properties.Settings.Default.TimeLogsBasePath;
            var allTimeLogFileNames = Directory.EnumerateFiles(timeLogsBasePath);
            var allTimeLogRepositories = allTimeLogFileNames.Select(filePaths => new CsvFileRepository(timeLogsBasePath, Path.GetFileName(filePaths))).ToList();

            var logEntriesPerEngineer = allTimeLogRepositories.ToDictionary(repository => repository.GetEngineerName(), repository => repository.GetAllLogEntries());
            var logEntriesPerDay = logEntriesPerEngineer.SelectMany(x => x.Value).GroupBy(x => x.CreateDate);

            var perEngineerReportsBuilder = new PerEngineerMonthReportsBuilder(Properties.Settings.Default.ReportsBasePaths, command.FirstDayOfPeriod, logEntriesPerEngineer);
            var cumulativeReportsBuilder = new CumulativeMonthReportsBuilder(Properties.Settings.Default.ReportsBasePaths, command.FirstDayOfPeriod, logEntriesPerDay);

            perEngineerReportsBuilder.Build();
            cumulativeReportsBuilder.Build();
        }
    }
}
