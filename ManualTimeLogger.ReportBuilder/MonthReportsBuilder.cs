using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManualTimeLogger.ReportBuilder
{
    public class MonthReportsBuilder
    {
        private readonly string _timeLogsBasePath;
        private readonly string _reportsBasePath;
        private readonly DateTime _firstDayOfMonth;

        public MonthReportsBuilder(string timeLogsBasePath, string reportsBasePath, DateTime firstDayOfMonth)
        {
            if (firstDayOfMonth.Day != 1)
            {
                throw new ArgumentException("First day of the month should have day number 1", nameof(firstDayOfMonth));
            }

            _timeLogsBasePath = timeLogsBasePath;
            _reportsBasePath = reportsBasePath;
            _firstDayOfMonth = firstDayOfMonth;
        }

        public void Build()
        {
            throw new NotImplementedException();
        }
    }
}
