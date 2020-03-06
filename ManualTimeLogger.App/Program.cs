using System;
using System.Windows.Forms;
using ManualTimeLogger.Domain;
using ManualTimeLogger.Persistence;

namespace ManualTimeLogger.App
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(
                new LogEntryInputForm(
                    new LogEntryInputParser(), 
                    new CsvFileRepository(Properties.Settings.Default.TimeLogsBasePath, $"{Properties.Settings.Default.Engineer}_timelog_{DateTime.Today:yyyyMM}.csv"),
                    new AutoFillListBoxController(Properties.Settings.Default.IsAutoFillFeatureEnabled, Properties.Settings.Default.LabelPresets.Split(';'), Properties.Settings.Default.ActivityPresets.Split(';'))));
        }
    }
}
