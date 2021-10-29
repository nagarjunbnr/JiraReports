using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Reports.TimeTracking
{
    public class TimeTrackingReportData
    {
        [DisplayName("Worklog Id")]
        public string WorklogId { get; set; }

        [DisplayName("Issue Id")]
        public string IssueId { get; set; }

        //[DisplayName("Epic Issue Id")]
        //public string EpicIssueId { get; set; }

        [DisplayName("Summary")]
        public string Summary { get; set; }

        //[DisplayName("Epic Summary")]
        //public string EpicSummary { get; set; }

        //[DisplayName("CapEx")]
        //public bool Capex { get; set; }

        //[DisplayName("Issue Type")]
        //public string IssueType { get; set; }

        //[DisplayName("Estimate Hours")]
        //public string OriginalEstimateHours { get; set; }

        //[DisplayName("Resolution Date")]
        //public string ResolutionDate { get; set; }

        //[DisplayName("Resolution")]
        //public string Resolution { get; set; }

        //[DisplayName("Name")]
        //public string LoggedBy { get; set; }

        [DisplayName("Display Name")]
        public string DisplayName { get; set; }

        [DisplayName("Project Name")]
        public string ProjectName { get; set; }

        //[DisplayName("Project Category")]
        //public string ProjectCategory { get; set; }

        [DisplayName("Logged Hours")]
        public decimal Hours { get; set; }

        //[DisplayName("Updated")]
        //public DateTime Updated { get; set; }

        [DisplayName("Work Started On")]
        public string StartDate { get; set; }

        [DisplayName("Logged On")]
        public string LoggedOn { get; set; }

        [DisplayName("Logged On Year")]
        public string LoggedOnYear { get; set; }

        [DisplayName("Logged On Month")]
        public string LoggedOnMonth { get; set; }

        [DisplayName("Logged On Day")]
        public string LoggedOnDay { get; set; }

        [DisplayName("Logged On Hour")]
        public string LoggedOnHour { get; set; }

        [DisplayName("Logged On Minute")]
        public string LoggedOnMinute { get; set; }

        [DisplayName("Logged On AM PM")]
        public string LoggedOnAMPM { get; set; }

        [DisplayName("Components")]
        public string Components { get; set; }

        //[DisplayName("Fix Versions")]
        //public string FixVersions { get; set; }

        //[DisplayName("Tax Credit")]
        //public string TaxCredit { get; set; }
    }
}
