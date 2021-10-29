using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Jira.Model
{
    public class JiraIssueChangelogItem
    {
        public string ID { get; set; }
        public JiraUser Author { get; set; }
        public DateTime Created { get; set; }
        public List<JiraChangelogItemDetails> Items { get; set; }
    }
}
