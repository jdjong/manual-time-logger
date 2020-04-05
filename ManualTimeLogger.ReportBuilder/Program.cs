using ManualTimeLogger.ReportBuilder.Commands;
using ManualTimeLogger.ReportBuilder.Persistence;

namespace ManualTimeLogger.ReportBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            var reportingCommandProvider = new CommandProvider();

            var command = (dynamic)reportingCommandProvider.GetCommand(args);
            var reportRepositoryFactory = new CsvFileRepositoryFactory(Properties.Settings.Default.ReportsBasePath, command.AccountFilter);

            // TODO, introduce time log repository factory as well
            var commandHandler = new CommandHandler(Properties.Settings.Default.TimeLogsBasePath, reportRepositoryFactory);

            commandHandler.Handle(command);
        }
    }
}
