﻿using ManualTimeLogger.ReportBuilder.Commands;
using ManualTimeLogger.ReportBuilder.Persistence;

namespace ManualTimeLogger.ReportBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            var reportingCommandProvider = new CommandProvider();

            var command = (dynamic)reportingCommandProvider.GetCommand(args);
            var timeLogRepositoryFactory = new ManualTimeLogger.Persistence.CsvFileRepositoryFactory(Properties.Settings.Default.TimeLogsBasePath);
            var reportRepositoryFactory = new CsvFileRepositoryFactory(Properties.Settings.Default.ReportsBasePath, $"{command.AccountFilter ?? "all"}");

            var commandHandler = new CommandHandler(timeLogRepositoryFactory, reportRepositoryFactory);

            commandHandler.Handle(command);
        }
    }
}
