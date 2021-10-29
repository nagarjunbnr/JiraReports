using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Jira
{
    public class JiraIssueStatus
    {
        public string Self { get; set; }
        public string ID { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string IconUrl { get; set; }
        public JiraIssueStatusCategory StatusCategory { get; set; }
    }
}
