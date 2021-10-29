using JiraReports.Common;
using JiraReports.Services.Jira.Model;
using JiraReports.Services.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Jira
{
    [InstancePerRequest(typeof(IJiraGitService))]
    public class JiraGitService : JiraService, IJiraGitService
    {
        private const string PluginBaseURL = @"https://jira.affinitiv.com/rest/gitplugin/1.0/";


        public JiraGitService(IWebClient webClient, IWebAuthentication auth, IHttpUtility httpUtility)
            : base(webClient, auth, httpUtility)
        {

        }

        public List<GitCommit> GetIssueCommits(string issueKey)
        {
            return Deserialize<GitCommitResponse>(GitPluginGetJSON($"/issues/{issueKey}/commits"))?.Commits ?? new List<GitCommit>();
        }

        protected IWebClientResponse GitPluginGet(string path, params (string key, string value)[] qs)
        {
            IWebClientResponse response = this.WebClient.Get(this.HTTPUtility.Combine(PluginBaseURL, path), qs);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                response.Dispose();
                throw new JiraNotAuthorizedException();
            }

            return response;
        }

        protected string GitPluginGetJSON(string path, HttpStatusCode expectedStatusCode = HttpStatusCode.OK,
            params (string key, string value)[] qs)
        {
            using (IWebClientResponse response = GitPluginGet(path, qs))
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
