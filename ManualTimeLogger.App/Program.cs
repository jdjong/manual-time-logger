using System;
using System.Linq;
using System.Windows.Forms;
using ManualTimeLogger.App.AutoFill;
using ManualTimeLogger.App.Properties;
using ManualTimeLogger.Domain;
using ManualTimeLogger.Persistence;
using NLog;

namespace ManualTimeLogger.App
{
    static class Program
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                var accounts = Settings.Default.AccountPresets.Split(';').ToList();
                IAutoFillListBoxController autoFillListBoxController = new DoNothingAutoFillListBoxController();

                if (Settings.Default.IsAutoFillFeatureEnabled)
                {
                    autoFillListBoxController = new AutoFillListBoxController(Settings.Default.LabelPresets.Split(';'), Settings.Default.ActivityPresets.Split(';'));
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(
                    new LogEntryInputForm(
                        new CsvFileRepository(Settings.Default.TimeLogsBasePath, $"{Environment.UserName}_timelog_{DateTime.Today:yyyyMM}.csv"),
                        new LogEntryTextBoxController(new LogEntryInputParser(accounts), autoFillListBoxController, new HotKeyHelper(accounts))
                ));
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
        }
    }
}
