using ManualTimeLogger.ReportBuilder.Commands;

namespace ManualTimeLogger.ReportBuilder
{
    class Program
    {
        private static ICommand _buildReportCommand;

        static void Main(string[] args)
        {
            var reportingCommandProvider = new CommandProvider();
            _buildReportCommand = reportingCommandProvider.GetCommand(args);

            Handle((dynamic)_buildReportCommand);
        }

        private static void Handle(BuildWeekReportsCommand command)
        {
            var reportsBuilder = new WeekReportsBuilder(Properties.Settings.Default.TimeLogsBasePath, Properties.Settings.Default.ReportsBasePaths, command.FirstDayOfPeriod);
            reportsBuilder.Build();
        }

        private static void Handle(BuildMonthReportsCommand command)
        {
            var reportsBuilder = new MonthReportsBuilder(Properties.Settings.Default.TimeLogsBasePath, Properties.Settings.Default.ReportsBasePaths, command.FirstDayOfPeriod);
            reportsBuilder.Build();
        }
    }
}
