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
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(
                    new LogEntryInputForm(
                        new LogEntryInputParser(Settings.Default.AccountPresets.Split(';').ToList()), 
                        new CsvFileRepository(Settings.Default.TimeLogsBasePath, $"{Settings.Default.Engineer}_timelog_{DateTime.Today:yyyyMM}.csv"),
                        // TODO, add interface and do nothing controller. Inject proper one depending on feature IsAutoFillFeatureEnabled.
                        new AutoFillListBoxController(Settings.Default.IsAutoFillFeatureEnabled, Settings.Default.LabelPresets.Split(';'), Settings.Default.ActivityPresets.Split(';')),
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
