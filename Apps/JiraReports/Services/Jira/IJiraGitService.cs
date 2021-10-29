using JiraReports.Services.Jira.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Jira
{
    public interface IJiraGitService
    {
        List<GitCommit> GetIssueCommits(string issueKey);
    }
}
