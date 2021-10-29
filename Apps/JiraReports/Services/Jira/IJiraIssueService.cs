using JiraReports.Services.Jira.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Jira
{
    public interface IJiraIssueService
    {
        JiraIssue GetIssue(string issueId, string[] fields = null, bool includeChangelog = false);
        string GetEpicCaseTag(string issueId);

        string GetEpicInvestmentCategory(string issueId);
        List<JiraIssue> GetIssues(List<string> issueIDs, string[] fields, bool includeChangelog = false);
        List<JiraIssue> GetIssuesInProject(string projectKey, string[] fields = null, bool includeChangelog = false, 
            DateTime? updatedAfter = null, DateTime? updatedBefore = null, string sprintId = null);

      
        List<JiraIssue> GetIssuesInSprint(string sprintId, string[] fields);
        JiraWorklogList GetWorklogsForIssue(string issueId);
    }
}
