using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Jira
{
    public class JiraWorklog
    {
        public string Self { get; set; }
        public JiraUserBasic Author { get; set; }
        public JiraUserBasic UpdateAuthor { get; set; }
        public string Comment { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public DateTime Started { get; set; }
        public string TimeSpent { get; set; }
        public int TimeSpentSeconds { get; set; }
        public string ID { get; set; }
        public string IssueID { get; set; }

        public decimal Hours
        {
            get
            {
                return this.TimeSpentSeconds / 3600m;
            }
        }


    }
}
