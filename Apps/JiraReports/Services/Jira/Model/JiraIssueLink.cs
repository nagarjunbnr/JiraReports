using JiraReports.Services.Jira.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Jira
{
    public class JiraIssueLink
    {
        public string ID { get; set; }
        public string Self { get; set; }
        public JiraIssueLinkType Type { get; set; }
        public JiraIssue OutwardIssue { get; set; }
        public JiraIssue InwardIssue { get; set; }
    }
}
