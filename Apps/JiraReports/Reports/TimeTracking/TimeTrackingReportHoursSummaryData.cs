using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Reports.TimeTracking
{
    public class TimeTrackingReportHoursSummaryData
    {
        [DisplayName("Display Name")]
        public string DisplayName { get; set; }

        [DisplayName("Total Hours")]
        public decimal TotalHours { get; set; }

        [DisplayName("Average Per Day")]
        public decimal AveragePerDay { get; set; }

        [DisplayName("Last Logged Hours On")]
        public string LastLoggedOn { get; set; }

    }
}
