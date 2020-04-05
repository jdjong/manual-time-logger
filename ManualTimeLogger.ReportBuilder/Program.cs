using ManualTimeLogger.ReportBuilder.Commands;

namespace ManualTimeLogger.ReportBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            var commandHandler = new CommandHandler(Properties.Settings.Default.TimeLogsBasePath, Properties.Settings.Default.ReportsBasePath);
            var reportingCommandProvider = new CommandProvider();

            commandHandler.Handle((dynamic)reportingCommandProvider.GetCommand(args));
        }
    }
}
