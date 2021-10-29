using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Jira.Model
{
    public class GitCommit
    {
        public string Author { get; set; }
        public string CommitID { get; set; }
        public DateTime Date { get; set; }
        public string Message { get; set; }
        public GitRepository Repository { get; set; }
    }
}
