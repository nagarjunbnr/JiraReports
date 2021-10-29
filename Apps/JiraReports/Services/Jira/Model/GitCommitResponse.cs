using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Jira.Model
{
    public class GitCommitResponse
    {
        public bool Success { get; set; }
        public List<GitCommit> Commits { get; set; }
    }
}
