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
                        new AutoFillListBoxController(Settings.Default.IsAutoFillFeatureEnabled, Settings.Default.LabelPresets.Split(';'), Settings.Default.ActivityPresets.Split(';'))));
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
        }
    }
}
