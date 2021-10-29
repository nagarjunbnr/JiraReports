using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Reports.TimeTracking
{
    public class CapExReportData
    {
        [DisplayName("Issue Id")]
        public string IssueId { get; set; }

        [DisplayName("Epic Issue Id")]
        public string EpicIssueId { get; set; }

        [DisplayName("Summary")]
        public string Summary { get; set; }

        [DisplayName("Epic Summary")]
        public string EpicSummary { get; set; }

        [DisplayName("CapEx")]
        public bool Capex { get; set; }

        [DisplayName("Issue Type")]
        public string IssueType { get; set; }

        [DisplayName("Estimate Hours")]
        public string OriginalEstimateHours { get; set; }

        [DisplayName("Resolution Date")]
        public string ResolutionDate { get; set; }

        [DisplayName("Resolution")]
        public string Resolution { get; set; }

        [DisplayName("Logged By")]
        public string LoggedBy { get; set; }

        [DisplayName("Project Name")]
        public string ProjectName { get; set; }

        [DisplayName("Project Category")]
        public string ProjectCategory { get; set; }

        [DisplayName("Logged Hours")]
        public decimal Hours { get; set; }

        [DisplayName("Issue Created Date")]
        public string IssueCreatedDate { get; set; }

        [DisplayName("Work Started On")]
        public string StartDate { get; set; }

        [DisplayName("Logged On")]
        public string LoggedOn { get; set; }

        [DisplayName("Components")]
        public string Components { get; set; }

        [DisplayName("Fix Versions")]
        public string FixVersions { get; set; }

        [DisplayName("Tax Credit")]
        public string TaxCredit { get; set; }
    }
}
