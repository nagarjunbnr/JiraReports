using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Jira.Model
{
    public class JiraTimeTrackingData
    {
        public string OriginalEstimate { get; set; }
        public string RemainingEstimate { get; set; }
        public string TimeSpent { get; set; }
        public int OriginalEstimateSeconds { get; set; }
        public int RemainingEstimateSeconds { get; set; }
        public int TimeSpentSeconds { get; set; }
    }
}
