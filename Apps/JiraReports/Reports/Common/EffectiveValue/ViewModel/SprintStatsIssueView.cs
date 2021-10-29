using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Reports.Common.EffectiveValue.ViewModel
{
    public class SprintStatsIssueView
    {

        public string Issue { get; set; }

        public string IssueType { get; set; }

        public bool IsValueAdded { get; set; }

        public decimal LoggedHours { get; set; }

        public string Summary { get; set; }

        public string Sprint { get; set; }

    }

}
