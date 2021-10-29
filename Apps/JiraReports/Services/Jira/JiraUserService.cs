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
    [InstancePerRequest(typeof(IJiraUserService))]
    public class JiraUserService : JiraService, IJiraUserService
    {
        public JiraUserService(IWebClient webClient, IWebAuthentication auth, IHttpUtility httpUtility)
            : base(webClient, auth, httpUtility)
        {

        }

        public JiraUser GetMe()
        {
            using (IWebClientResponse response = JiraGet("/myself"))
            {
                using (Stream responseStream = response.GetStream())
                {
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        string json = reader.ReadToEnd();

                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            return Deserialize<JiraUser>(json);
                        }

                        throw new Exception("Unhandled Exception!");
                    }
                }
            }
        }
    }
}
