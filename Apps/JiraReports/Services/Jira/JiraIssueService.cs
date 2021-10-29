using JiraReports.Common;
using JiraReports.Services.Jira.Model;
using JiraReports.Services.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Jira
{
    [InstancePerRequest(typeof(IJiraIssueService))]
    public class JiraIssueService : JiraService, IJiraIssueService
    {
        public JiraIssueService(IWebClient webClient, IWebAuthentication auth, IHttpUtility httpUtility)
            : base(webClient, auth, httpUtility)
        {

        }

        public JiraIssue GetIssue(string issueId, string[] fields = null, bool includeChangelog = false)
        {
            List<(string key, string value)> qs = new List<(string key, string value)>();
            if(includeChangelog)
            {
                qs.Add(("expand", "changelog"));
                qs.Add(("maxResults", "1000"));
            }
            if(fields?.Length > 0)
            {
                qs.Add(("fields", String.Join(",", fields)));
            }

            return Deserialize<JiraIssue>(JiraGetJSON($"/issue/{issueId}", System.Net.HttpStatusCode.OK,
                qs.ToArray()));
        }

        public string GetEpicCaseTag(string issueId)
        {
            JiraEpicFieldSummary epicField = Deserialize<JiraEpicFieldSummary>(JiraGetJSON($"/issue/{issueId}?fields=customfield_10100,summary"));

            return epicField?.Fields?.EpicCaseTag;
        }

        public string GetEpicInvestmentCategory(string issueId)
        {
            JiraEpicFieldSummary epicField = Deserialize<JiraEpicFieldSummary>(JiraGetJSON($"/issue/{issueId}?fields=customfield_12509"));

            return epicField?.Fields?.InvestmentCategory?.Category;
        }

        public List<JiraIssue> GetIssues(List<string> issueIDs, string[] fields, bool includeChangelog = false)
        {
            List< (string, string) > parameters = new List<(string, string)>()
            {
                ( "jql", $"issue in ({String.Join(",", issueIDs)})" ),
                ( "startAt", "0" ),
                ( "maxResults", issueIDs.Count.ToString()),
                ( "fields", string.Join(",", fields) )
            };

            if(includeChangelog)
            {
                parameters.Add(("expand", "changelog"));
            }

            string json = JiraGetJSON($"/search", System.Net.HttpStatusCode.OK, parameters.ToArray());

            return Deserialize<JiraSearchResult>(json).Issues;
        }

        public JiraWorklogList GetWorklogsForIssue(string issueId)
        {
            string json = JiraGetJSON($"/issue/{issueId}/worklog");

            return  Deserialize<JiraWorklogList>(json);
        }

        //public List<JiraIssue> GetIssues(List<string> issueIDs, string[] fields)
        //{
        //    return Deserialize<JiraSearchResult>(JiraPostJSON($"/search", System.Net.HttpStatusCode.OK, new
        //    {
        //        jql = $"issue in ({String.Join(",", issueIDs)})",
        //        startAt = 0,
        //        maxResults = issueIDs.Count,
        //        fields = fields
        //    })).Issues;
        //}

        public List<JiraIssue> GetIssuesInProject(string projectKey, 
            string[] fields = null, bool includeChangelog = false, 
            DateTime? updatedAfter = null,
            DateTime? updatedBefore = null, string sprintNumber = "350")
        {
            List<JiraIssue> issues = new List<JiraIssue>();

            int startAt = 0;
            int increment = 100;

            JiraSearchResult searchResult = null;

            do
            {
                string jql = $"project = '{projectKey}'";

                //if (updatedAfter != null)
                //{
                //    jql += $"&updated >= '{updatedAfter.Value.ToString("yyyy/MM/dd")}'";
                //}

                //if (updatedBefore != null)
                //{
                //    jql += $"&updated <= '{updatedBefore.Value.ToString("yyyy/MM/dd")}'";
                //}

                jql += string.Format("&Sprint in ('{0}')", sprintNumber);

                List<(string, string)> parameters = new List<(string, string)>()
                {
                    ( "jql", jql),
                    ( "startAt", startAt.ToString() ),
                    ( "maxResults", increment.ToString() ),
                    ( "fields", string.Join(",", fields == null ? new string[] { "timetracking", "project", "customfield_10104", "updated" } : fields))
                };


                if (includeChangelog)
                {
                    parameters.Add(("expand", "changelog"));
                }

                string json = JiraGetJSON($"/search", System.Net.HttpStatusCode.OK, parameters.ToArray());
                searchResult = Deserialize<JiraSearchResult>(json);

                issues.AddRange(searchResult.Issues);

                startAt += increment;
            } while(searchResult.Total > startAt);

            return issues;
        }

        public List<JiraIssue> GetIssuesInSprint(string sprintId, string[] fields)
        {
            List<JiraIssue> issues = new List<JiraIssue>();

            int startAt = 0;
            int increment = 1000;

            JiraSearchResult searchResult = null;

            do
            {
                searchResult = Deserialize<JiraSearchResult>(JiraPostJSON($"/search", System.Net.HttpStatusCode.OK, new
                {
                    jql = $"sprint = {sprintId}",
                    startAt = startAt,
                    maxResults = increment,
                    fields = fields
                }));

                issues.AddRange(searchResult.Issues);

                startAt += increment;
            } while (searchResult.Total > startAt);

            return issues;
        }

        public List<JiraIssue> GetIssuesInProject(string projectKey, string[] fields = null, bool includeChangelog = false, DateTime? updatedAfter = null, DateTime? updatedBefore = null)
        {
            throw new NotImplementedException();
        }
    }
}
