using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Reports.Common.EffectiveValue
{
    public class FailureRateIssue
    {

        public string Issue { get; set; }

        public string IssueType { get; set; }

        public int? ZenDeskTicketCount { get; set; }

        public decimal? EstimatedHours { get; set; }

        public string EstimatedStoryPoints { get; set; }

    }
}
