using JiraReports.Common;
using JiraReports.Services.Jira.Model;
using JiraReports.Services.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JiraReports.Services.Jira
{
    [InstancePerRequest(typeof(IJiraAgileService))]
    public class JiraAgileService : JiraService, IJiraAgileService
    {
        private const string PluginBaseURL = @"https://jira.affinitiv.com/rest/agile/1.0/";
        private JiraConstants jiraConstants;

        public JiraAgileService(IWebClient webClient, IWebAuthentication auth, IHttpUtility httpUtility, JiraConstants jiraConstants)
            : base(webClient, auth, httpUtility)
        {
            this.jiraConstants = jiraConstants;
        }

        public List<AgileBoard> GetAllBoards()
        {
            List<AgileBoard> boards = new List<AgileBoard>();
            int startAt = 0;
            AgileServiceResult<AgileBoard> result;

            do
            {
                result = Deserialize<AgileServiceResult<AgileBoard>>(AgilePluginGetJSON($"/board", expectedStatusCode: HttpStatusCode.OK,
                    ("startAt", startAt.ToString())));

                if (result?.Values != null)
                {
                    boards.AddRange(result.Values);
                    startAt += result.Values.Count;
                }

            } while (result?.IsLast != true);

            return boards;
        }

        public List<AgileBoard> GetDeliveryBoards()
        {
            return this.GetAllBoards()
                .Where(b => this.jiraConstants.MainBoards.Any(bn => bn == b.Name))
                .ToList();
        }

        public List<AgileSprint> GetSprintsForBoard(string boardId)
        {
            List<AgileSprint> sprints = new List<AgileSprint>();
            int startAt = 0;

            AgileServiceResult<AgileSprint> result;
            do
            {
                result = Deserialize<AgileServiceResult<AgileSprint>>(AgilePluginGetJSON($"/board/{boardId}/sprint",
                    expectedStatusCode: HttpStatusCode.OK,
                    ("startAt", startAt.ToString())
                ));

                if (result?.Values != null)
                {
                    sprints.AddRange(result.Values);
                    startAt += result.Values.Count;
                }

            } while (result?.IsLast != true);
            return sprints;

            #region For COE Commented

            /* For COE Kanban Board

            if (boardId == "313") 
            {
                string str;

                str = JiraGetJSON($"/project/COE/versions", expectedStatusCode: HttpStatusCode.OK, ("startAt", startAt.ToString()));

                string json = @"{'d':" + str + "}";

                JObject o = JObject.Parse(json);

                JArray a = (JArray)o["d"];

                sprints = a.ToObject<List<AgileSprint>>();

                return sprints;
            }
            else
            {

            } 
            */
            
            #endregion
        }

        public List<int> GetBoardSprintNumbers(IEnumerable<string> boardIds)
        {
            var sprintNumbers = new List<int>();

            foreach (string boardId in boardIds)
            {
                sprintNumbers.AddRange(GetBoardSprintNumbers(boardId));
            }

            return sprintNumbers.Distinct().ToList();
        }

        public List<int> GetBoardSprintNumbers(string boardId)
        {
            var result = new List<int>();

            List<AgileSprint> sprints = this.GetSprintsForBoard(boardId);

            foreach (AgileSprint sprint in sprints)
            {
                var parsedSprint = sprint.ParseSprintName();

                if (parsedSprint.matched)
                {
                    result.Add(parsedSprint.sprint);
                }
                #region For COE Commented

                /*
                 
                if (boardid == "313") // for coe kanban board
                {
                    int sprintnumber;
                    if (int.tryparse(sprint.name, out sprintnumber))
                    {
                        result.add(sprintnumber);
                    };
                }
                else
                {
                }
                
                */
                
                #endregion
            }

            return result;
        }

        public List<JiraIssue> GetIssuesForSprint(string sprintId, params string[] fields)
        {
            List<JiraIssue> issues = new List<JiraIssue>();
            int startAt = 0;
            JiraSearchResult result;

            List<(string, string)> fieldList = new List<(string, string)>();
            fieldList.Add(("startAt", startAt.ToString()));

            if (fields != null && fields.Length > 0)
            {
                fieldList.Add(("fields", string.Join(",", fields)));
            }

            do
            {
                result = Deserialize<JiraSearchResult>(AgilePluginGetJSON($"/sprint/{sprintId}/issue",
                    expectedStatusCode: HttpStatusCode.OK,
                    fieldList.ToArray()
                ));

                if (result?.Issues != null)
                {
                    issues.AddRange(result.Issues);
                    startAt += result.Issues.Count;
                }

            } while (result != null && (startAt + 1) < result.Total);

            return issues;
        }

        protected IWebClientResponse AgilePluginGet(string path, params (string key, string value)[] qs)
        {
            IWebClientResponse response = this.WebClient.Get(this.HTTPUtility.Combine(PluginBaseURL, path), qs);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                response.Dispose();
                throw new JiraNotAuthorizedException();
            }

            return response;
        }

        protected string AgilePluginGetJSON(string path, HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            params (string key, string value)[] qs)
        {
            using (IWebClientResponse response = AgilePluginGet(path, qs))
            {
                using (Stream responseStream = response.GetStream())
                {
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        if (response.StatusCode == expectedStatusCode)
                        {
                            return reader.ReadToEnd();
                        }

                        throw new Exception("Unhandled Exception!");
                    }
                }
            }
        }
    }
}
