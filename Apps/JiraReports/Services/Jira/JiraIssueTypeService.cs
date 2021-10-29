using JiraReports.Common;
using JiraReports.Services.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraReports.Services.Jira
{
    [InstancePerRequest(typeof(IJiraIssueTypeService))]
    public class JiraIssueTypeService : JiraService, IJiraIssueTypeService
    {
        public JiraIssueTypeService(IWebClient webClient, IWebAuthentication auth, IHttpUtility httpUtility)
            : base(webClient, auth, httpUtility)
        {

        }

        public List<JiraIssueType> GetIssueTypes()
        {
            return Deserialize<List<JiraIssueType>>(JiraGetJSON("/issuetype"));
        }
    }
}
