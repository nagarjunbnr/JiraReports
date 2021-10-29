using JiraReports.Services.Jira.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Reports.KeyPoints.Predictability
{
    public class PredictabilityBreakdown
    {
        [DisplayName("Project")]
        public string Project { get; set; }

        [DisplayName("Sprint")]
        public int Sprint { get; set; }

        [DisplayName("SprintId")]
        public string SprintId { get; set; }

        [DisplayName("Issue")]
        public string IssueName { get; set; }

        [DisplayName("IssueType")]
        public string IssueType { get; set; }

        [DisplayName("Status")]
        public string Status { get; set; }

        [DisplayName("Incomplete Reason")]
        public string IncompleteReason { get; set; }

        public bool IsPulled { get; set; }

        public JiraIssue Issue { get; set; }

    }
}
