using System;
using System.Linq;
using System.Windows.Forms;
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
                IAutoFillListBoxController autoFillListBoxController = new DoNothingAutoFillListBoxController();

                if (Settings.Default.IsAutoFillFeatureEnabled)
                {
                    autoFillListBoxController = new AutoFillListBoxController(Settings.Default.LabelPresets.Split(';'), Settings.Default.ActivityPresets.Split(';'));
                }
                
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(
                    new LogEntryInputForm(
                        new LogEntryInputParser(Settings.Default.AccountPresets.Split(';').ToList()), 
                        new CsvFileRepository(Settings.Default.TimeLogsBasePath, $"{Environment.UserName}_timelog_{DateTime.Today:yyyyMM}.csv"),
                        autoFillListBoxController,
                        Settings.Default.AccountPresets.Split(';').ToList()));
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
        }
    }
}
