using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Jira.Model
{
    public class JiraIssueChangelog
    {
        public int StartAt { get; set; }
        public int MaxResults { get; set; }
        public int Total { get; set; }

        public List<JiraIssueChangelogItem> Histories { get; set; }
    }
}
