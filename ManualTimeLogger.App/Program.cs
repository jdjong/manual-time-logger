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

            // TODO, make basepath and name for timelog configurable
            Application.Run(new LogEntryInputForm(new LogEntryInputParser(), new CsvFileRepository(@"C:\temp\timelogs", $"joost_timelog_{DateTime.Today:yyyyMM}.csv")));
        }
    }
}
