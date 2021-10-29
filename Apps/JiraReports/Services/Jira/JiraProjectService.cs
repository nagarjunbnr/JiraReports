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
    [InstancePerRequest(typeof(IJiraProjectService))]
    public class JiraProjectService : JiraService, IJiraProjectService
    {
        public JiraProjectService(IWebClient webClient, IWebAuthentication auth, IHttpUtility httpUtility)
            : base(webClient, auth, httpUtility)
        {

        }

        public List<JiraProject> GetProjects()
        {
            return Deserialize<List<JiraProject>>(JiraGetJSON("/project"));
        }

        public List<JiraProjectCategory> GetProjectCategories()
        {
            return Deserialize<List<JiraProjectCategory>>(JiraGetJSON("/projectCategory"));
        }
    }
}
