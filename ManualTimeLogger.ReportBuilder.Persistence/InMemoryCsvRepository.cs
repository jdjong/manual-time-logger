using System;
using System.Collections.Generic;
using System.Linq;

namespace ManualTimeLogger.ReportBuilder.Persistence
{
    public class InMemoryCsvRepository : IRepository
    {
        public string FileName { get; private set; }
        public char CsvSeparator => ';';
        public List<string> SavedReportEntries;

        // TODO, remove duplication for strings with CsvFileRepository class

        public InMemoryCsvRepository(string fileName, DateTime firstDayOfReport, int periodNrOfDays)
        {
            FileName = fileName;
            SavedReportEntries = new List<string>();
            var headerString = $"\"GroupedBy\"{CsvSeparator}\"ThenGroupedBy\"{CsvSeparator}\"Totaal\"{CsvSeparator}{string.Join(CsvSeparator.ToString(), Enumerable.Range(0, periodNrOfDays).Select(nr => $"\"{firstDayOfReport.AddDays(nr):yyyyMMdd}\""))}";
            SavedReportEntries.Add(headerString);
        }

        public void SaveReportEntry(ReportEntry reportEntry)
        {
            var reportEntryString = $"\"{reportEntry.GroupedBy}\"{CsvSeparator}\"{reportEntry.ThenGroupedBy}\"{CsvSeparator}\"{Enumerable.Sum<KeyValuePair<DateTime, float>>(reportEntry.NrOfHoursPerDay, x => x.Value)}\"{CsvSeparator}{string.Join(CsvSeparator.ToString(), Enumerable.Select<KeyValuePair<DateTime, float>, string>(reportEntry.NrOfHoursPerDay, hoursForDay => $"\"{GetHoursForDayString(hoursForDay)}\""))}";
            SavedReportEntries.Add(reportEntryString);
        }

        private string GetHoursForDayString(KeyValuePair<DateTime, float> hoursForDay)
        {
            return hoursForDay.Value < 0.01f
                ? string.Empty
                : Math.Round(hoursForDay.Value, 2, MidpointRounding.AwayFromZero).ToString("0.00");
        }
    }
}
